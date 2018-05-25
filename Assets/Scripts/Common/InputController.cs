using UnityEngine;
using System.Collections;

public class InputController : MonoBehaviour {
	// Constants
	private const float QUICK_CLICK_TIME_WINDOW = 0.3f; // quick-click is used for double-clicks (but could be extended to 3 or more).
	// Instance
	static private InputController instance;
	// Properties
	private int numQuickClicks=0; // for double-clicks (or maybe even more).
	private bool isDoubleClick; // reset every frame.
	private float timeWhenNullifyDoubleClick;
	private Vector2 mousePosDown;
	private Vector2 playerInput0;
	private Vector2 playerInput1;

	// Getters
	static public InputController Instance {
		get {
//			if (instance==null) { return this; } // Note: This is only here to prevent errors when recompiling code during runtime.
			return instance;
		}
	}
	public bool IsDoubleClick { get { return isDoubleClick; } }
	public Vector3 MousePosScreen { get { return (Input.mousePosition - new Vector3(Screen.width,Screen.height,0)*0.5f) / ScreenHandler.ScreenScale; } }

	static public int GetMouseButtonDown() {
		if (Input.GetMouseButtonDown(0)) return 0;
		if (Input.GetMouseButtonDown(1)) return 1;
		if (Input.GetMouseButtonDown(2)) return 2;
		return -1;
	}
	static public int GetMouseButtonUp() {
		if (Input.GetMouseButtonUp(0)) return 0;
		if (Input.GetMouseButtonUp(1)) return 1;
		if (Input.GetMouseButtonUp(2)) return 2;
		return -1;
	}
	static public bool IsMouseButtonDown () {
		return GetMouseButtonDown() != -1;
	}
	static public bool IsMouseButtonUp () {
		return GetMouseButtonUp() != -1;
	}
	public Vector2 PlayerInput(int playerIndex) {
		switch(playerIndex) {
			case 0: return playerInput0;
			case 1: return playerInput1;
			default: Debug.LogError("Oops; asking for playerInput for player " + playerIndex + ", but there aren't that many player inputs!"); return Vector2.zero;
		}
	}




	// ----------------------------------------------------------------
	//  Awake
	// ----------------------------------------------------------------
	private void Awake () {
		// There can only be one (instance)!!
		if (instance != null) {
			Destroy (this.gameObject);
			return;
		}
		instance = this;
	}

	// ----------------------------------------------------------------
	//  Update
	// ----------------------------------------------------------------
	private void Update () {
		// Hack, only here to prevent errors after compiling during runtime. NOTE: DOESN'T ACTUALLY WORK.
		if (instance == null) {
			instance = this;
		}
		RegisterButtonInputs ();
		RegisterMouseInputs ();
	}

	private void RegisterButtonInputs () {
		// HACK TEMP switched these. :P (Better way to do this would be to switch in the Input settings)
		playerInput1 = new Vector2(Input.GetAxis("Player0_Horz"), Input.GetAxis("Player0_Vert"));
		playerInput0 = new Vector2(Input.GetAxis("Player1_Horz"), Input.GetAxis("Player1_Vert"));
	}

	private void RegisterMouseInputs () {
		isDoubleClick = false; // I'll say otherwise in a moment.

		if (Input.GetMouseButtonDown(0)) {
			// Up how many clicks we got.
			numQuickClicks ++;
			// This is the SECOND click??
			if (numQuickClicks == 2) { // to-do: Discount if mouse pos is too far from first down pos.
				isDoubleClick = true;
			}
			// Reset the timer to count another quick-click!
			timeWhenNullifyDoubleClick = Time.time + QUICK_CLICK_TIME_WINDOW;
		}
		// Have we nullified our double-click by waiting too long?
		if (Time.time >= timeWhenNullifyDoubleClick) {
			numQuickClicks = 0;
		}
	}

}


