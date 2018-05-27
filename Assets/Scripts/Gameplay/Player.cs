using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	// Constants
	private readonly Vector2 DirL = new Vector2(-1, 0);
	private readonly Vector2 DirR = new Vector2( 1, 0);
	private readonly Vector2 DirB = new Vector2( 0,-1);
	private readonly Vector2 DirT = new Vector2( 0, 1);
	private readonly Vector2 DirBL = new Vector2(-1,-1).normalized;
	private readonly Vector2 DirBR = new Vector2( 1,-1).normalized;
	private readonly Vector2 DirTL = new Vector2(-1, 1).normalized;
	private readonly Vector2 DirTR = new Vector2( 1, 1).normalized;
	// Controls
	private const float AimDirDeadZone = 0.1f; // we ignore any input of magnitude less than this.
	// Movement
	private const float MaxVelX = 18;
	private const float MaxVelYUp = 500;
	private const float MaxVelYDown = -100;
	private const float DashForce = 26f;
	private const int MaxDashes = 2; // How many times we can dash until we have to touch the ground again.
	private const float DashDuration = 0.2f; // how long each dash lasts.
	private const float DashCooldown = 0.1f; // Note: disabled. in SECONDS. Don't allow dashing twice this quickly.
	// Properties
	private bool onGround;
	private bool isDashing;
	private float timeWhenCanDash; // set to Time.time + DashCooldown when we dash.
	private float timeWhenDashEnd; // set to Time.time + DashDuration when we dash.
	private int numDashesSinceGround;
	private Vector2 aimDir; // the direction we're facing. We dash in this direction.
	private Vector2 dashDir; // the direction of the current dash. We keep going in this dir for the dash.
	// Components
	[SerializeField] private PlayerHat myHat=null;
	[SerializeField] private PlayerFeet myFeet=null;
	[SerializeField] private BoxCollider2D bodyCollider=null;
	[SerializeField] private Rigidbody2D myRigidbody=null;
	[SerializeField] private PlayerBody body=null;

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

	public Vector2 AimDir { get { return aimDir; } }
	public bool IsDashing { get { return isDashing; } }
	public int NumDashesSinceGround { get { return numDashesSinceGround; } }



	// ----------------------------------------------------------------
	//  Start
	// ----------------------------------------------------------------
	private void Start () {
		// Size me, queen!
		SetSize (new Vector2(2.5f, 2.5f)); // NOTE: I don't understand why we gotta cut it by 100x. :P

		ResetVel();
	}
	private void SetSize(Vector2 _size) {
		body.SetSize(_size);
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
		aimDir = DirR;
	}



	// ----------------------------------------------------------------
	//  Update
	// ----------------------------------------------------------------
	private void Update () {
		if (Time.timeScale == 0) { return; } // No time? No dice.

		UpdateAimDir();
		AcceptDashInput();

//		// TEMP test
//		s_body.color = onGround ? Color.green : Color.yellow;
	}
	private void UpdateAimDir() {
		if (inputAxis.magnitude > AimDirDeadZone) {
			aimDir = inputAxis.normalized; // TODO: lock to 8 directions.
		}
	}
	private void AcceptDashInput() {
		if (Input.GetKeyDown(KeyCode.Space)) { // TEMP hardcoded
			OnDashPressed();
		}
	}

	private void FixedUpdate () {
		if (Time.timeScale == 0) { return; } // No time? No dice.

		UpdateDash();
		if (!isDashing) {
			ApplyGravity();
			ApplyFriction();
		}
		ApplyTerminalVel();
	}


	private void UpdateDash() {
		if (isDashing && Time.time>=timeWhenDashEnd) {
			EndDash();
		}
	}
	private void ApplyGravity() {
		vel += new Vector2(0,-2f);//TEMP HACK Physics2D.gravity;
	}
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


	// ----------------------------------------------------------------
	//  Doers
	// ----------------------------------------------------------------
	private void Dash() {
		isDashing = true;
		dashDir = aimDir.normalized;
		vel = dashDir * DashForce;
		timeWhenCanDash = Time.time + DashCooldown;
		timeWhenDashEnd = Time.time + DashDuration;
		numDashesSinceGround ++;
		body.OnDash();
		GameManagers.Instance.EventManager.OnPlayerDash(this);
	}
	private void EndDash() {
		isDashing = false;
		vel = dashDir * DashForce; // Make sure we end with the dashing vel.
		body.OnDashEnd();
		GameManagers.Instance.EventManager.OnPlayerDashEnd(this);
	}

	// ----------------------------------------------------------------
	//  Events
	// ----------------------------------------------------------------
	private void OnDashPressed() {
		// We're on the ground and NOT timed out of dashing! Go!
		if (numDashesSinceGround<MaxDashes && Time.time>=timeWhenCanDash) {
			Dash();
		}
	}
	public void OnFeetTouchGround() {
		onGround = true;
		numDashesSinceGround = 0;
		body.OnFeetTouchGround();
	}
	public void OnFeetLeaveGround() {
		onGround = false;
	}
	/** We also do a constant check, which is way more reliable. */
	public void OnFeetTouchingGround() {
		onGround = true;
	}


}

/*
public class Player : MonoBehaviour {
	// Constants
	private const float InputScaleX = 6f;
	private const float MaxVelX = 18;
	private const float MaxVelYUp = 500;
	private const float MaxVelYDown = -100;
	private const float JumpForce = 26f;
	private const float DELAYED_JUMP_WINDOW = 0.15f; // in SECONDS. The time window where we can press jump just BEFORE landing, and still jump when we land.
	private const float JUMP_TIMEOUT_WINDOW = 0.0f; // Note: disabled. in SECONDS. Don't allow jumping twice this quickly.
	// Properties
	private bool onGround;
	private Color bodyColorNeutral;
	private float timeWhenDelayedJump; // set when we're in the air and press Jump. If we touch ground before this time, we'll do a delayed jump!
	private float timeWhenCanJump; // set to Time.time + JUMP_TIMEOUT_WINDOW when we jump.
	private int numJumpsSinceGround;
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
		//		bodyColorNeutral = new ColorHSB(0.5f, 0.5f, 1f).ToColor();
		//		s_body.color = bodyColorNeutral;
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
	private void Update () {
		if (Time.timeScale == 0) { return; } // No time? No dice.

		AcceptJumpInput();

		//		// TEMP test
		//		s_body.color = onGround ? Color.green : Color.yellow;
	}
	private void AcceptJumpInput() {
		if (Input.GetKeyDown(KeyCode.Space)) { // TEMP hardcoded
			OnJumpPressed();
		}
	}

	private void FixedUpdate () {
		if (Time.timeScale == 0) { return; } // No time? No dice.

		ApplyFriction();
		AcceptMoveInput();
		ApplyTerminalVel();
	}


	private void AcceptMoveInput() {
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
	}

	//	private const float RunAccel = 1000f;
	//	private const float RunReduce = 400f;
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
		vel = new Vector2(vel.x, JumpForce);
		timeWhenDelayedJump = -1; // reset this just in case.
		timeWhenCanJump = Time.time + JUMP_TIMEOUT_WINDOW;
		numJumpsSinceGround ++;
		GameManagers.Instance.EventManager.OnPlayerJump(this);
	}

	// ----------------------------------------------------------------
	//  Events
	// ----------------------------------------------------------------
	private void OnJumpPressed() {
		// We're on the ground and NOT timed out of jumping! Go!
		if (numJumpsSinceGround<2 && Time.time>=timeWhenCanJump) {//onGround 
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
		numJumpsSinceGround = 0;
		if (Time.time <= timeWhenDelayedJump) {
			Jump();
		}
	}
	public void OnFeetLeaveGround() {
		onGround = false;
	}
	/** We also do a constant check, which is way more reliable. * /
	public void OnFeetTouchingGround() {
		onGround = true;
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
*/
