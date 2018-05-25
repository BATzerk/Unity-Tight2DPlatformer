using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleGround : MonoBehaviour {
	// Components
	[SerializeField] private SpriteRenderer sr_fill;
//	[SerializeField] private SpriteRenderer sr_stroke;
	[SerializeField] private BoxCollider2D myCollider;
	// Properties
	[SerializeField] private bool startsOn;
	private bool isOn;
	private Color bodyColorOn, bodyColorOff;


	// ----------------------------------------------------------------
	//  Start / Destroy
	// ----------------------------------------------------------------
	private void Start () {
		bodyColorOn = startsOn ? new Color(3/255f, 170/255f, 204/255f) : new Color(217/255f, 74/255f, 136/255f);
		bodyColorOff = new Color(bodyColorOn.r,bodyColorOn.g,bodyColorOn.b, bodyColorOn.a*0.2f);

		// Size our sliced sprite properly!
//		sr_stroke.transform.localScale = new Vector3(1/this.transform.localScale.x, 1/this.transform.localScale.y, 1);
//		sr_stroke.sprite.

		SetIsOn(startsOn);

		// Add event listeners!
		GameManagers.Instance.EventManager.PlayerJumpEvent += OnPlayerJump;
	}
	private void OnDestroy() {
		// Remove event listeners!
		GameManagers.Instance.EventManager.PlayerJumpEvent -= OnPlayerJump;
	}


	// ----------------------------------------------------------------
	//  Events
	// ----------------------------------------------------------------
	private void OnPlayerJump(Player player) {
		ToggleIsOn();
	}

	// ----------------------------------------------------------------
	//  Doers
	// ----------------------------------------------------------------
	private void ToggleIsOn() {
		SetIsOn (!isOn);
	}
	private void SetIsOn(bool _isOn) {
		isOn = _isOn;
		myCollider.enabled = isOn;
		sr_fill.color = isOn ? bodyColorOn : bodyColorOff;
	}


}
