using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	// Constants
	private const float inputScaleX = 6f;
	public  const float BOUNDSX = 25f; // FUDGEing this to match the camera orthographicSize. :P
	private const float MAX_VEL_X = 18;
	private const float MAX_VEL_Y_UP = 1000;
	private const float MAX_VEL_Y_DOWN = -200;
	private const float MIN_Y_BOUNCE = 24;// 14
	private const float DOWN_FORCE = -3f;// 1.8
	// Properties
	[SerializeField] private int playerIndex;
	private Color bodyColorNeutral, bodyColorDownForce;
	private float timeWhenCanAcceptDownForce; // kinda fudgy: don't accept down input for a moment after we bounced. Feels better for new players.
	private Vector2 pinputAxis; // previous inputAxis
	private Vector2 currentSize;
	private Vector2 sizeDownward;
	private Vector2 sizeUpward;
	// Components
	[SerializeField] private PlayerHat myHat;
	[SerializeField] private PlayerFeet myFeet;
	[SerializeField] private BoxCollider2D bodyCollider;
	[SerializeField] private Rigidbody2D myRigidbody;
	[SerializeField] private SpriteRenderer s_body;
	[SerializeField] private ParticleSystem ps_downForce;

	// Getters/Setters
	public int PlayerIndex { get { return playerIndex; } }
	private Vector3 pos {
		get { return this.transform.localPosition; }
		set { this.transform.localPosition = value; }
	}
	private Vector2 vel {
		get { return myRigidbody.velocity; }
		set { myRigidbody.velocity = value; }
	}
	private bool isGrounded; // TEMP
	public bool IsGrounded {
		get { return isGrounded; }
		set {
			isGrounded = value;
		}
	}


	// ----------------------------------------------------------------
	//  Start
	// ----------------------------------------------------------------
	private void Start () {
		// Color me impressed!
		bodyColorNeutral = GameProperties.PlayerColor(playerIndex);
		bodyColorDownForce = Color.Lerp(bodyColorNeutral, Color.black, 0.5f);
		s_body.color = bodyColorNeutral;
		// Size me, queen!
		Vector2 sizeNeutral = new Vector2(2.5f, 2f); // NOTE: I don't understand why we gotta cut it by 100x. :P
		sizeDownward = new Vector2(sizeNeutral.x*0.8f, sizeNeutral.y*1.3f);
		sizeUpward = new Vector2(sizeNeutral.x*1.3f, sizeNeutral.y*0.8f);
		currentSize = sizeNeutral;
		ApplyCurrentSize();
		GameUtils.SetParticleSystemColor(ps_downForce, bodyColorNeutral);

		ResetVel();
	}
	private void ApplyCurrentSize() {
		bodyCollider.size = currentSize;
		GameUtils.SizeSpriteRenderer(s_body, currentSize);
		myFeet.OnSetBodySize(currentSize);
		myHat.OnSetBodySize(currentSize);
	}


	// ----------------------------------------------------------------
	//  Update
	// ----------------------------------------------------------------
	private void FixedUpdate () {
//		if (Time.timeScale == 0) { return; } // No time? No dice.

		AcceptInput();
		ApplyAirFriction();
		ApplyTerminalVel();
		UpdateSize();
		ApplyBounds();
	}


	private void AcceptInput() {
		if (InputController.Instance==null) { return; } // for building at runtime.
		Vector2 inputAxis = InputController.Instance.PlayerInput(playerIndex);

		// Horizontal!
		if (inputAxis.x != 0) {
			if (GameMathUtils.AreSameSign(inputAxis.x, vel.x)) {
				vel += new Vector2(inputAxis.x*inputScaleX, 0);
			}
			else {
				vel = new Vector2(0, vel.y);
			}
		}

		// Vertical!
		bool isDownForce = false; // I'll say otherwise in a moment.
		if (Time.unscaledTime >= timeWhenCanAcceptDownForce) { // If it's time to accept dat force...!
			if (inputAxis.y < -0.01f) {
				isDownForce = true;
				if (pinputAxis.y >= 0) { // If we JUST started pushing down, then really hush our y vel!
//					vel = new Vector2(vel.x, vel.y*0.3f);
					vel += new Vector2(0, DOWN_FORCE*5);
				}

//				if (vel.y <= 0) {
					vel += new Vector2(0, DOWN_FORCE);
//				}
//				else {
//					vel = new Vector2(vel.x, vel.y*0.4f);
//				}
				// Also add x friction.
				float frictionX = 0.94f;
				vel = new Vector2(vel.x*frictionX, vel.y);
			}
		}
		// Update particleSystem!
		GameUtils.SetParticleSystemEmissionEnabled(ps_downForce, isDownForce);
		s_body.color = isDownForce ? bodyColorDownForce : bodyColorNeutral;

		pinputAxis = inputAxis;
	}
	private void ApplyAirFriction() {
		vel = new Vector2(vel.x*0.99f, vel.y);
	}
	private void ApplyTerminalVel() {
		vel = new Vector2(Mathf.Clamp(vel.x, -MAX_VEL_X,MAX_VEL_X), Mathf.Clamp(vel.y, MAX_VEL_Y_DOWN,MAX_VEL_Y_UP));
	}
	private void ApplyBounds() {
		// Loop me horizontally!
		if (pos.x<-BOUNDSX) { pos += new Vector3(BOUNDSX*2,0,0); }
		if (pos.x> BOUNDSX) { pos -= new Vector3(BOUNDSX*2,0,0); }
	}

	private void UpdateSize() {
		float sizeLoc = Mathf.InverseLerp(70,-70, vel.y);
		Vector2 targetSize = Vector2.Lerp(sizeUpward, sizeDownward, sizeLoc);
		currentSize = Vector2.Lerp(currentSize, targetSize, 0.8f); // ease!
		ApplyCurrentSize();
	}


	// ----------------------------------------------------------------
	//  Doers
	// ----------------------------------------------------------------
	public void ResetPos(Vector3 _pos) {
		pos = _pos;
		ResetVel();
	}
	private void ResetVel() {
		vel = Vector2.zero;
		timeWhenCanAcceptDownForce = 0; // allow it nooowwww.
	}

	// ----------------------------------------------------------------
	//  Events
	// ----------------------------------------------------------------
	private void OnCollisionEnter2D(Collision2D collision) {
		GameObject collisionGO = collision.collider.gameObject;
		if (LayerMask.LayerToName(collisionGO.layer) == LayerNames.Ground) {
			OnTouchGround(collisionGO);
		}
	}
	private void OnTouchGround(GameObject groundGO) {
		// Are we doing a legit bounce?
		BoxCollider2D groundCollider = groundGO.GetComponent<BoxCollider2D>();
		float myYBottom = pos.y + bodyCollider.offset.y - bodyCollider.size.y*0.5f;
		float groundYTop = groundGO.transform.localPosition.y + groundCollider.offset.y + groundCollider.size.y*0.5f;
		if (myYBottom+0.2f >= groundYTop) { // am I almost ABOVE the ground??
			// Give me a MINIMUM bounce vel!
			float frictionX = 0.5f;
			vel = new Vector2(vel.x*frictionX, Mathf.Max(Mathf.Abs(vel.y), MIN_Y_BOUNCE));
			timeWhenCanAcceptDownForce = Time.unscaledTime + 0.2f;
		}
	}


}
