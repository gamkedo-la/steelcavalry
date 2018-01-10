using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour {

	private float targetCamZoomSize, mechCamZoomSize;
	private Camera mainCam;
	private float camZoomK = 0.95f;
	//
	public GameObject myPlayer;
	

	// Use this for initialization
	void Start () {
		mainCam = Camera.main;
		targetCamZoomSize = 4;
		mechCamZoomSize = 6;
	}
	
	// Update is called once per frame
	void FixedUpdate() {
		//Updates the zoom
		
		mainCam.orthographicSize = mainCam.orthographicSize * camZoomK + targetCamZoomSize * (1.0f - camZoomK);

	    transform.position = new Vector3 (myPlayer.transform.position.x, myPlayer.transform.position.y, -10);
	}

	//Adaptive zoom based on optional mech passed; uses default otherwise
	public void MechZoom(Mech mech =  null) {
		if (mech == null) targetCamZoomSize = 4;
	
		else targetCamZoomSize = Mathf.Max(mechCamZoomSize, mech.transform.lossyScale.y);
	}
}
