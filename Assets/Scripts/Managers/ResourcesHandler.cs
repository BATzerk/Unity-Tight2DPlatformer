using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesHandler : MonoBehaviour {
	// References!
	[SerializeField] public GameObject prefabGO_characterView;
	[SerializeField] public GameObject prefabGO_imageLine;
	[SerializeField] public GameObject prefabGO_rhythmHitCharTrack;
	[SerializeField] public GameObject prefabGO_rhythmHitGhostButton;
	[SerializeField] public GameObject prefabGO_rhythmHitNote;
	[SerializeField] public GameObject prefabGO_rhythmHitNoteBody_button;
	[SerializeField] public GameObject prefabGO_rhythmHitNoteBody_text;
	[SerializeField] public GameObject prefabGO_stongTile;
//	[SerializeField] public GameObject prefabGO_songBitTile;
//	[SerializeField] public GameObject prefabGO_songBitNodeView;


	// Instance
	static private ResourcesHandler instance;
	static public ResourcesHandler Instance { get { return instance; } }

	// Awake
	private void Awake () {
		// There can only be one (instance)!
		if (instance == null) {
			instance = this;
		}
		else {
			GameObject.Destroy (this);
		}
	}
}
