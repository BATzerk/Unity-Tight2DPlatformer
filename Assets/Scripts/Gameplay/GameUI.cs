using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour {
	// Components
	[SerializeField] private Image i_scoreBackingP0;
	[SerializeField] private Image i_scoreBackingP1;
	[SerializeField] private Text t_scoreP0;
	[SerializeField] private Text t_scoreP1;
	[SerializeField] private Text t_winnerName;
	[SerializeField] private GameObject go_scoreP0;
	[SerializeField] private GameObject go_scoreP1;
	[SerializeField] private GameController gameControllerRef;
	[SerializeField] private GameObject go_preGameOverlay;
	[SerializeField] private GameObject go_postGameOverlay;
	// Properties
	private float scoreP0Scale,scoreP0ScaleVel;
	private float scoreP1Scale,scoreP1ScaleVel;

	// Getters / Setters
//	private Vector3 go_scoreP0Scale {
//		get { return go_scoreP0.transform.localScale; }
//		set { go_scoreP0.transform.localScale = value; }
//	}
//	private Vector3 go_scoreP1Scale {
//		get { return go_scoreP1.transform.localScale; }
//		set { go_scoreP1.transform.localScale = value; }
//	}


	// ----------------------------------------------------------------
	//  Start
	// ----------------------------------------------------------------
	private void Start() {
		i_scoreBackingP0.color = GameProperties.PlayerColor(0);
		i_scoreBackingP1.color = GameProperties.PlayerColor(1);

		UpdateScoreTexts();
	}

	public void OnGameControllerSetGameState(GameStates gameState) {
		// Do things
		switch(gameState) {
			case GameStates.PreGame:
				SetScoreGOsActive(false);
				go_preGameOverlay.SetActive(true);
				go_postGameOverlay.SetActive(false);
				break;

			case GameStates.Playing:
				SetScoreGOsActive(true);
				scoreP0Scale = scoreP1Scale = 1;
				scoreP0ScaleVel = Random.Range(0.05f, 0.1f);
				scoreP1ScaleVel = Random.Range(0.05f, 0.1f);
				go_preGameOverlay.SetActive(false);
				go_postGameOverlay.SetActive(false);
				break;

			case GameStates.PostGame:
				SetScoreGOsActive(true);
				go_preGameOverlay.SetActive(false);
				go_postGameOverlay.SetActive(true);
				t_winnerName.color = GameProperties.PlayerColor(gameControllerRef.WinningPlayerIndex);
				t_winnerName.text = GameProperties.PlayerName(gameControllerRef.WinningPlayerIndex).ToUpper();
				break;
		}
	}

	private void SetScoreGOsActive(bool isActive) {
		go_scoreP0.SetActive(isActive);
		go_scoreP1.SetActive(isActive);
	}

	// ----------------------------------------------------------------
	//  Updating HUD
	// ----------------------------------------------------------------
	public void UpdateScoreTexts() {
		string stringP0 = gameControllerRef.ScoreP0.ToString();
		string stringP1 = gameControllerRef.ScoreP1.ToString();
		if (t_scoreP0.text != stringP0) {
			t_scoreP0.text = stringP0;
			scoreP0Scale = 3f;
		}
		if (t_scoreP1.text != stringP1) {
			t_scoreP1.text = stringP1;
			scoreP1Scale = 3f;
		}
	}


	// ----------------------------------------------------------------
	//  Update
	// ----------------------------------------------------------------
	private void Update() {
		UpdateScoreTextsScale();
		ApplyScoreTextsScale();
	}
	private void UpdateScoreTextsScale() {
		float elasticStrength = 20f; // higher is weaker.
		float friction = 0.9f;
		// Elastic physics!
		scoreP0ScaleVel += (1-scoreP0Scale) / elasticStrength;
		scoreP0Scale += scoreP0ScaleVel;
		scoreP0ScaleVel *= friction;

		scoreP1ScaleVel += (1-scoreP1Scale) / elasticStrength;
		scoreP1Scale += scoreP1ScaleVel;
		scoreP1ScaleVel *= friction;
	}
	private void ApplyScoreTextsScale() {
		go_scoreP0.transform.localScale = Vector3.one * scoreP0Scale;
		go_scoreP1.transform.localScale = Vector3.one * scoreP1Scale;
	}

}
