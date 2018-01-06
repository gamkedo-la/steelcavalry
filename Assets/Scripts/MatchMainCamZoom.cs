using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchMainCamZoom : MonoBehaviour {
	Camera mainCam;
	Camera thisCam;
	// Use this for initialization
	void Start () {
		mainCam = Camera.main;
		thisCam = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
		thisCam.orthographicSize = mainCam.orthographicSize;
	}
}
