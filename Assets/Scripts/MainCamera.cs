using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour {

	private float targetCamZoomSize, defaultCamZoomSize;
	private Camera mainCam;
	private float camZoomK = 0.95f;

	// Use this for initialization
	void Start () {
		mainCam = Camera.main;
		targetCamZoomSize = defaultCamZoomSize = mainCam.orthographicSize;
		//defaultCamZoomSize = mainCam.orthographicSize;
	}
	
	// Update is called once per frame
	void FixedUpdate() {
		//Updates the zoom
		mainCam.orthographicSize = mainCam.orthographicSize * camZoomK +
			targetCamZoomSize * (1.0f - camZoomK);
	}

	//Adaptive zoom based on optional mech passed; uses default otherwise
	public void MechZoom(Mech mech =  null) {
		if (mech == null) targetCamZoomSize = defaultCamZoomSize; //the 5 could be replaced :/
	
		else targetCamZoomSize = Mathf.Max(defaultCamZoomSize, mech.transform.lossyScale.y);
	}
}
