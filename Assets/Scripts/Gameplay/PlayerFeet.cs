﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFeet : MonoBehaviour {
	// Components
	[SerializeField] private BoxCollider2D myCollider;
	// References
	[SerializeField] private Player myPlayer;

	// Getters
	public Player MyPlayer { get { return myPlayer; } }


	// ----------------------------------------------------------------
	//  Start
	// ----------------------------------------------------------------
	private void Start () {
		
	}

	// ----------------------------------------------------------------
	//  Doers
	// ----------------------------------------------------------------
	public void OnSetBodySize(Vector2 bodySize) {
		myCollider.size = new Vector2(bodySize.x*0.99f, 0.5f);
		this.transform.localPosition = new Vector3(0, -bodySize.y*0.5f + myCollider.size.y*0.5f - 0.1f);
	}


	// ----------------------------------------------------------------
	//  Events
	// ----------------------------------------------------------------
	private void OnTriggerEnter2D(Collider2D otherCol) {
		// Ground??
		if (LayerMask.LayerToName(otherCol.gameObject.layer) == LayerNames.Ground) {
			myPlayer.IsGrounded = true;
		}
	}
	private void OnTriggerExit2D(Collider2D otherCol) {
		// Ground??
		if (LayerMask.LayerToName(otherCol.gameObject.layer) == LayerNames.Ground) {
			myPlayer.IsGrounded = false;
		}
	}


}

