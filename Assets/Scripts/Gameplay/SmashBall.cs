using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SmashBall : MonoBehaviour {
	// Components
//	[SerializeField] private Text t_countdown;
	[SerializeField] private ParticleSystem ps_hitBurst;
	// Properties
	private int numHitsLeft; // smash me this many times!

	private Vector3 pos {
		get { return this.transform.localPosition; }
		set { this.transform.localPosition = value; }
	}

	// ----------------------------------------------------------------
	//  Start
	// ----------------------------------------------------------------
	private void Start () {
		float radius = 2;
		numHitsLeft = Random.Range(3,5);

		SpriteRenderer s_body = this.GetComponentInChildren<SpriteRenderer>();
		CircleCollider2D circleCollider = this.GetComponentInChildren<CircleCollider2D>();
		GameUtils.SizeSpriteRenderer(s_body, radius*2,radius*2);
		circleCollider.radius = radius;

		UpdateCountdownText();
	}


	// ----------------------------------------------------------------
	//  FixedUpdate
	// ----------------------------------------------------------------
	private void FixedUpdate() {
		ApplyBounds();
	}
	private void ApplyBounds() {
		// Loop me horizontally!
		if (pos.x<-Player.BOUNDSX) { pos += new Vector3(Player.BOUNDSX*2,0,0); }
		if (pos.x> Player.BOUNDSX) { pos -= new Vector3(Player.BOUNDSX*2,0,0); }
	}


	// ----------------------------------------------------------------
	//  Events
	// ----------------------------------------------------------------
	private void OnCollisionEnter2D(Collision2D collision) {
		GameObject collisionGO = collision.collider.gameObject;
		if (LayerMask.LayerToName(collisionGO.layer) == LayerNames.Player) {
			if (collision.relativeVelocity.y<-2) { // Player's moving downward? Then it's landed on me!
				OnPlayerHitMe(collision);
			}
		}
	}
	private void OnPlayerHitMe(Collision2D collision) {
		ps_hitBurst.Emit(10);
		numHitsLeft --;
		UpdateCountdownText();
		if (numHitsLeft <= 0) {
			GameManagers.Instance.EventManager.OnPlayerOpenSmashBall(collision.collider.gameObject.GetComponent<Player>());
			GameObject.Destroy(this.gameObject); // Destroy me!
		}
	}

	private void UpdateCountdownText() {
//		t_countdown.text = numHitsLeft.ToString();
	}
}
