using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour {
	private Camera mainCam;
	private float cameraMarginSizePercX = 0.3f;
	private float cameraMarginSizePercY = 0.2f;
	private float cameraMarginChaseSpeed = 50.0f;
	// Use this for initialization
	void Start () {
		mainCam = Camera.main;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		Vector3 onCameraPos = mainCam.WorldToViewportPoint(transform.position);
		float diffToMove;
		Debug.Log(onCameraPos);
		if(onCameraPos.x < cameraMarginSizePercX) {
			diffToMove = cameraMarginSizePercX - onCameraPos.x;
			mainCam.transform.position += diffToMove * cameraMarginChaseSpeed * Time.deltaTime * Vector3.left;
		}
		float rightSide = 1.0f - cameraMarginSizePercX;
		if(onCameraPos.x > rightSide) {
			diffToMove = onCameraPos.x - rightSide;
			mainCam.transform.position += diffToMove * cameraMarginChaseSpeed * Time.deltaTime * Vector3.right;
		}

		float topSide = 1.0f - cameraMarginSizePercY;
		if(onCameraPos.y > topSide) {
			diffToMove = topSide - onCameraPos.y;
			mainCam.transform.position += diffToMove * cameraMarginChaseSpeed * Time.deltaTime * Vector3.down;
		}
		if(onCameraPos.y < cameraMarginSizePercY) {
			diffToMove = onCameraPos.y - cameraMarginSizePercY;
			mainCam.transform.position += diffToMove * cameraMarginChaseSpeed * Time.deltaTime * Vector3.up;
		}

	}
}
