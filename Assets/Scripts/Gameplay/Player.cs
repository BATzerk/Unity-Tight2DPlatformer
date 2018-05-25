using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	// Constants
	private const float InputScaleX = 6f;
	private const float MaxVelX = 18;
	private const float MaxVelYUp = 500;
	private const float MaxVelYDown = -100;
	private const float JUMP_FORCE = 26f;// 1.8
	private const float DELAYED_JUMP_WINDOW = 0.15f; // in SECONDS. The time window where we can press jump just BEFORE landing, and still jump when we land.
	// Properties
	private bool onGround;
	private Color bodyColorNeutral;
	private float timeWhenDelayedJump; // set when we're in the air and press Jump. If we touch ground before this time, we'll do a delayed jump!
	// Components
	[SerializeField] private PlayerHat myHat;
	[SerializeField] private PlayerFeet myFeet;
	[SerializeField] private BoxCollider2D bodyCollider;
	[SerializeField] private Rigidbody2D myRigidbody;
	[SerializeField] private SpriteRenderer s_body;

	// Getters/Setters
	private Vector3 pos {
		get { return this.transform.localPosition; }
		set { this.transform.localPosition = value; }
	}
	private Vector2 vel {
		get { return myRigidbody.velocity; }
		set { myRigidbody.velocity = value; }
	}
	private Vector2 inputAxis { get { return InputController.Instance.PlayerInput; } }



	// ----------------------------------------------------------------
	//  Start
	// ----------------------------------------------------------------
	private void Start () {
		// Color me impressed!
		bodyColorNeutral = new ColorHSB(0.5f, 0.5f, 1f).ToColor();
		s_body.color = bodyColorNeutral;
		// Size me, queen!
		SetSize (new Vector2(2.5f, 2.5f)); // NOTE: I don't understand why we gotta cut it by 100x. :P

		ResetVel();
	}
	private void SetSize(Vector2 _size) {
		GameUtils.SizeSpriteRenderer(s_body, _size);
		bodyCollider.size = _size;
		myFeet.OnSetBodySize(_size);
		myHat.OnSetBodySize(_size);
	}


	// ----------------------------------------------------------------
	//  Resetting
	// ----------------------------------------------------------------
	public void ResetPos(Vector3 _pos) {
		pos = _pos;
		ResetVel();
	}
	private void ResetVel() {
		vel = Vector2.zero;
	}



	// ----------------------------------------------------------------
	//  Update
	// ----------------------------------------------------------------
	private void FixedUpdate () {
//		if (Time.timeScale == 0) { return; } // No time? No dice.

		ApplyFriction();
		AcceptInput();
		ApplyTerminalVel();

		// QQQ test
		s_body.color = onGround ? Color.green : Color.yellow;
	}

	private const float RunAccel = 1000f;
	private const float RunReduce = 400f;

	private void AcceptInput() {
		if (InputController.Instance==null) { return; } // for building at runtime.

		// Horizontal!
		if (inputAxis.x != 0) {
			float moveX = MathUtils.Sign(inputAxis.x);
			float mult = onGround ? 1 : 0.65f;

			float velXDelta = moveX*InputScaleX * mult;
			vel += new Vector2(velXDelta, 0);
		}
		else {
			vel = new Vector2(vel.x*0.8f, vel.y); // TEST
		}

		// Jump!
		if (Input.GetKeyDown(KeyCode.Space)) { // TEMP hardcoded
			OnJumpPressed();
		}
	}

//	float moveX = inputAxis.x;
//	float velXTarget = MaxVelX*moveX;
//	float mult = onGround ? 1 : 0.65f;
//	bool isPushingOppositeVelX = !MathUtils.IsSameSign(moveX, vel.x);
//	if (Mathf.Abs(vel.x)>MaxVelX && isPushingOppositeVelX) {
//		velXDelta = 
//			inputAxis.x*INPUT_SCALE_X
//			vel.x = Calc.Approach(vel.x, MaxVelX*moveX, RunReduce*mult); // Reduce back from beyond the max
//	}
//	else {
//		vel.x = Calc.Approach(vel.x, MaxVelX*moveX, RunAccel*mult); // Approach max speed
//	}
	private void ApplyFriction() {
		if (onGround) {
			vel = new Vector2(vel.x*0.8f, vel.y);
		}
		else {
			vel = new Vector2(vel.x*0.99f, vel.y);
		}
	}
	private void ApplyTerminalVel() {
		vel = new Vector2(Mathf.Clamp(vel.x, -MaxVelX,MaxVelX), Mathf.Clamp(vel.y, MaxVelYDown,MaxVelYUp));
	}

//	private void UpdateSize() {
//		float sizeLoc = Mathf.InverseLerp(70,-70, vel.y);
//		Vector2 targetSize = Vector2.Lerp(sizeUpward, sizeDownward, sizeLoc);
//		currentSize = Vector2.Lerp(currentSize, targetSize, 0.8f); // ease!
//		ApplyCurrentSize();
//	}

	// ----------------------------------------------------------------
	//  Doers
	// ----------------------------------------------------------------
	private void Jump() {
		vel += new Vector2(0, JUMP_FORCE);
		timeWhenDelayedJump = -1; // reset this just in case.
	}

	// ----------------------------------------------------------------
	//  Events
	// ----------------------------------------------------------------
	private void OnJumpPressed() {
		if (onGround) {
			Jump();
		}
		else {
			timeWhenDelayedJump = Time.time + DELAYED_JUMP_WINDOW;
		}
	}
//	private void OnCollisionEnter2D(Collision2D collision) {
//		GameObject collisionGO = collision.collider.gameObject;
//		if (LayerMask.LayerToName(collisionGO.layer) == LayerNames.Ground) {
//			OnTouchGround(collisionGO);
//		}
	//	}
	public void OnFeetTouchGround() {
		onGround = true;
		if (Time.time <= timeWhenDelayedJump) {
			Jump();
		}
	}
	public void OnFeetLeaveGround() {
		onGround = false;
	}
	private void OnTouchGround(GameObject groundGO) {
//		// Are we doing a legit bounce?
//		BoxCollider2D groundCollider = groundGO.GetComponent<BoxCollider2D>();
//		float myYBottom = pos.y + bodyCollider.offset.y - bodyCollider.size.y*0.5f;
//		float groundYTop = groundGO.transform.localPosition.y + groundCollider.offset.y + groundCollider.size.y*0.5f;
//		if (myYBottom+0.2f >= groundYTop) { // am I almost ABOVE the ground??
//			// Give me a MINIMUM bounce vel!
//			float frictionX = 0.5f;
//			vel = new Vector2(vel.x*frictionX, Mathf.Max(Mathf.Abs(vel.y), MIN_Y_BOUNCE));
//		}
	}


}
