using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
	// Constants
	private const int POINTS_TO_WIN = 10;
	// Properties
	private GameStates gameState;
	private bool isPaused = false;
	private int scoreP0,scoreP1;
	private float unscaledTimeSinceGameEnded;
	private float timeWhenAddSmashBall;
	// Components
	[SerializeField] private GameObject go_structure; // the physical level layout
	// References
	[SerializeField] private GameUI gameUI;
	[SerializeField] private GameObject prefabGO_smashBall;
	private int winningPlayerIndex; // set when they win (duh)

	// Getters / Setters
	public int WinningPlayerIndex { get { return winningPlayerIndex; } }//return winningPlayer==null ? -1 : winningPlayer.PlayerIndex; } }
	private InputController inputController { get { return InputController.Instance; } }
	public int ScoreP0 { get { return scoreP0; } }
	public int ScoreP1 { get { return scoreP1; } }

	private Vector3 GetPlayerResetPos(Player otherPlayer) {
		float spawnX = (otherPlayer.transform.localPosition.x + 25);
		return new Vector3(spawnX, 16);
	}



	// ----------------------------------------------------------------
	//  Start / Destroy
	// ----------------------------------------------------------------
	private void Start () {
		SetGameState(GameStates.PreGame);

		// Add event listeners!
		GameManagers.Instance.EventManager.PlayerOpenSmashBallAction += OnPlayerOpenSmashBall;
		GameManagers.Instance.EventManager.PlayerStompPlayerAction += OnPlayerStompPlayer;
	}
	private void OnDestroy() {
		// Add event listeners!
		GameManagers.Instance.EventManager.PlayerOpenSmashBallAction -= OnPlayerOpenSmashBall;
		GameManagers.Instance.EventManager.PlayerStompPlayerAction -= OnPlayerStompPlayer;
	}



	// ----------------------------------------------------------------
	//  Game Flow
	// ----------------------------------------------------------------
	private void SetGameState(GameStates _state) {
		gameState = _state;
		gameUI.OnGameControllerSetGameState(gameState);
		UpdateTimeScale();
		// Do things!
		switch(gameState) {
			case GameStates.PreGame:
				go_structure.SetActive(false);
				winningPlayerIndex = -1;
				break;

			case GameStates.Playing:
				go_structure.SetActive(true);
				winningPlayerIndex = -1;
				SetTimeWhenAddSmashBall();
				break;

			case GameStates.PostGame:
				go_structure.SetActive(true);
				unscaledTimeSinceGameEnded = Time.unscaledTime;
				break;
		}
	}
	private void OnPlayerWin(int _winningPlayerIndex) {
		winningPlayerIndex = _winningPlayerIndex;
		SetGameState(GameStates.PostGame);
	}


	// ----------------------------------------------------------------
	//  Doers - Loading Level
	// ----------------------------------------------------------------
	private void ReloadScene () { OpenScene (SceneNames.Gameplay); }
	private void OpenScene (string sceneName) { StartCoroutine (OpenSceneCoroutine (sceneName)); }
	private IEnumerator OpenSceneCoroutine (string sceneName) {
//		// First frame: Blur it up.
//		cameraController.DarkenScreenForSceneTransition ();
//		yield return null;

		// Second frame: Load up that business.
		UnityEngine.SceneManagement.SceneManager.LoadScene (sceneName);
		yield return null;
	}


	private void OnPlayerStompPlayer(Player p0, Player p1) {
		if (gameState != GameStates.Playing) { return; } // Only care if we're playing.
		// Score!
		IncreasePlayerScore(p0.PlayerIndex, 1);
		// If we're still playing, teleport the stomped player!
		if (gameState == GameStates.Playing) {
			p1.ResetPos(GetPlayerResetPos(p0));
		}
		// Otherwise, cast them into obvlivion.
		else {
			p1.ResetPos(new Vector3(9999,9999));
		}
	}
	private void OnPlayerOpenSmashBall(Player player) {
		IncreasePlayerScore(player.PlayerIndex, 2);
	}
	private void IncreasePlayerScore(int playerIndex, int increaseAmount) {
		if (playerIndex == 0) {
			scoreP0 += increaseAmount;
		}
		else if (playerIndex == 1) {
			scoreP1 += increaseAmount;
		}
		// Update HUD!
		gameUI.UpdateScoreTexts();
		// Did someone win??
		if (scoreP0 >= POINTS_TO_WIN) { OnPlayerWin(0); }
		else if (scoreP1 >= POINTS_TO_WIN) { OnPlayerWin(1); }
	}



	// ----------------------------------------------------------------
	//  Doers - Gameplay
	// ----------------------------------------------------------------
	private void TogglePause () {
		isPaused = !isPaused;
		UpdateTimeScale ();
	}
	private void UpdateTimeScale () {
		if (isPaused || gameState==GameStates.PreGame) { Time.timeScale = 0; }
		else { Time.timeScale = 1; }
	}
	private void SetTimeWhenAddSmashBall() {
		timeWhenAddSmashBall = Time.time+Random.Range(10,30); // add it every 15-ish seconds.
	}
	private void AddSmashBall() {
		GameObject newBall = Instantiate(prefabGO_smashBall);
		newBall.transform.localPosition = new Vector3(Random.Range(-20,20), Random.Range(-10,10), 0);
		SetTimeWhenAddSmashBall();
	}


	// ----------------------------------------------------------------
	//  Update
	// ----------------------------------------------------------------
	private void Update () {
		if (gameState == GameStates.Playing) {
			UpdateSmashBallSpawn();
		}
		RegisterButtonInput ();
	}
	private void UpdateSmashBallSpawn() {
		if (Time.time > timeWhenAddSmashBall) {
			print(Time.time + " " + timeWhenAddSmashBall);
			AddSmashBall();
		}
	}

	private void RegisterButtonInput () {
		bool isKey_alt = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
		bool isKey_control = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
		bool isKey_shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

		// Game Flow
		if (Input.anyKeyDown) {
			if (gameState==GameStates.PreGame) {
				SetGameState(GameStates.Playing);
			}
			else if (gameState==GameStates.PostGame && Time.unscaledTime>unscaledTimeSinceGameEnded+2f) { // 2 second delay to start again.
				ReloadScene();
			}
		}

		if (Input.GetKeyDown(KeyCode.Escape)) {
			TogglePause();
		}

		// ~~~~ DEBUG ~~~~
		if (Input.GetKeyDown(KeyCode.Return)) {
			ReloadScene();
		}
			
		// ALT + ___
		if (isKey_alt) {
			
		}
		// CONTROL + ___
		if (isKey_control) {
		}
		// SHIFT + ___
		if (isKey_shift) {
		}
	}







}






