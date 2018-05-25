﻿using System.Collections;
using UnityEngine;
//using UnityStandardAssets.ImageEffects;

public class GameCameraController : MonoBehaviour {
	// Camera
	[SerializeField] private Camera primaryCamera;
	// Properties
	private float orthoSizeNeutral;
	private float zoomAmount = 1; // UNUSED currently. Stays at 1. It's here for if/when we need it.
	private Rect viewRect;
	private float screenShakeVolume;
	private float screenShakeVolumeVel;
	// References
	[SerializeField] private FullScrim fullScrim;

	// Getters / Setters
	private float rotation {
		get { return this.transform.localEulerAngles.z; }
		set { this.transform.localEulerAngles = new Vector3 (0, 0, value); }
	}
	private Rect GetViewRect (Vector2 _rectCenter, float _zoomAmount) {
		Vector2 rectSize = GetViewRectSizeFromZoomAmount (_zoomAmount);
		return new Rect (_rectCenter-rectSize*0.5f, rectSize); // Note: Convert from center to bottom-left pos.
	}
	private Vector2 GetViewRectSizeFromZoomAmount (float zoomAmount) {
		return ScreenHandler.RelativeScreenSize / zoomAmount;
	}
	private float GetZoomAmountForViewRect (Rect rect) {
		return Mathf.Min (ScreenHandler.RelativeScreenSize.x/(float)rect.width, ScreenHandler.RelativeScreenSize.y/(float)rect.height);
	}
	private float ZoomAmount { get { return orthoSizeNeutral / primaryCamera.orthographicSize; } }

	// Debug
	private void OnDrawGizmos() {
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireCube (viewRect.center*GameVisualProperties.WORLD_SCALE, new Vector3(viewRect.size.x,viewRect.size.y, 10)*GameVisualProperties.WORLD_SCALE);
		Gizmos.color = Color.blue;
		Gizmos.DrawWireCube (viewRect.center*GameVisualProperties.WORLD_SCALE, new Vector3(ScreenHandler.RelativeScreenSize.x+11,ScreenHandler.RelativeScreenSize.y+11, 10)*GameVisualProperties.WORLD_SCALE);//+11 for bloat so we can still see it if there's overlap.
	}



	// ----------------------------------------------------------------
	//  Start / Destroy
	// ----------------------------------------------------------------
	private void Awake () {
		// Add event listeners!
		GameManagers.Instance.EventManager.PlayerStompPlayerAction += OnPlayerStompPlayer;
		GameManagers.Instance.EventManager.ScreenSizeChangedEvent += OnScreenSizeChanged;
	}
	private void OnDestroy () {
		// Remove event listeners!
		GameManagers.Instance.EventManager.PlayerStompPlayerAction -= OnPlayerStompPlayer;
		GameManagers.Instance.EventManager.ScreenSizeChangedEvent -= OnScreenSizeChanged;
	}

	// ----------------------------------------------------------------
	//  Events
	// ----------------------------------------------------------------
	private void OnScreenSizeChanged () {
		// Go ahead and totally reset me completely when the screen size changes, just to be safe.
		Reset ();
	}

	private void OnPlayerStompPlayer(Player p0, Player p1) {
		// Screen shake!
		screenShakeVolume = 4f;
	}



	// ----------------------------------------------------------------
	//  Update
	// ----------------------------------------------------------------
	private void FixedUpdate() {
		UpdateScreenShake ();
	}

	private void UpdateScreenShake () {
		if (screenShakeVolume==0 && screenShakeVolumeVel==0) {
			return;
		}
		screenShakeVolume += screenShakeVolumeVel;
		screenShakeVolumeVel += (0-screenShakeVolume) / 6f;
		screenShakeVolumeVel *= 0.94f;
		if (screenShakeVolume != 0) {
			if (Mathf.Abs (screenShakeVolume) < 0.001f && Mathf.Abs (screenShakeVolumeVel) < 0.001f) {
				screenShakeVolume = 0;
				screenShakeVolumeVel = 0;
			}
		}

		float rotation = screenShakeVolume;
		if (transform.localEulerAngles.z != rotation) {
			transform.localEulerAngles = new Vector3 (transform.localEulerAngles.x, transform.localEulerAngles.y, rotation);
		}
	}


	// ----------------------------------------------------------------
	//  Doers
	// ----------------------------------------------------------------
	public void Reset () {
		viewRect = new Rect ();
		viewRect.size = GetViewRectSizeFromZoomAmount (1);

		UpdateOrthoSizeNeutral ();
//		ApplyViewRect ();HACK disabled 'cause... it's not doing what I expect and this is a game jam

		screenShakeVolume = 0;
		screenShakeVolumeVel = 0;
	}

	private void UpdateOrthoSizeNeutral () {
		orthoSizeNeutral = ScreenHandler.OriginalScreenSize.y / 2f * GameVisualProperties.WORLD_SCALE;
	}

	private void ApplyViewRect () {
		this.transform.localPosition = new Vector3 (viewRect.center.x, viewRect.center.y, -10); // lock z-pos to -10.
		ApplyZoomAmountToCameraOrthographicSize ();
	}
	private void ApplyZoomAmountToCameraOrthographicSize () {
		float zoomAmount = GetZoomAmountForViewRect (viewRect);
		float targetOrthoSize = orthoSizeNeutral / zoomAmount;
		primaryCamera.orthographicSize = targetOrthoSize;
	}

//	private void UpdateViewRectActual () {
//		viewRect_actual = GetViewRect (this.transform.localPosition, ZoomAmount);
//	}

	public void DarkenScreenForSceneTransition () {
		fullScrim.Show (0.5f);
	}



}


