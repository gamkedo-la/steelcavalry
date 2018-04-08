using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour {
    public float outMechCamZoomSize = 45;
    public float inMechCamZoomSize = 60;
    public float zoomSpeed = 3;
    public float followSpeed = 3;
    public float shakePower = 0;

    private float targetCamZoomSize;
	private Camera mainCam;
    private Vector3 originalCamPosition;

    public GameObject myPlayer;    

	// Use this for initialization
	void Awake () {
		mainCam = Camera.main;
		targetCamZoomSize = outMechCamZoomSize; // sets default zoom size at start		
	}

	// Update is called once per frame
	void LateUpdate() {
        // Updates the zoom
        if (Mathf.Abs(mainCam.fieldOfView - targetCamZoomSize) > 0.1f) {
            mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, targetCamZoomSize, zoomSpeed * Time.deltaTime);
        }

        if (shakePower <= 0) {
            // Follows player around
            Vector3 targetPosition = new Vector3(myPlayer.transform.position.x, myPlayer.transform.position.y, -10);
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }

        if (shakePower > 0) {
            shakePower -= 100f * Time.deltaTime;
        }
	}

	//Adaptive zoom based on optional mech passed; uses default otherwise
	public void MechZoom(Mech mech =  null) {
        if (mech == null) {
            targetCamZoomSize = outMechCamZoomSize;
        }
        else {
            targetCamZoomSize = inMechCamZoomSize;
        }
	}

    public void ShakeTheCam(float shakeValue, int wait = 0, float rate = 0.001f) {
        if (shakePower <= 0) {
            originalCamPosition = mainCam.transform.position;
            shakePower = shakeValue;
            InvokeRepeating("CameraShaker", wait, rate);            
        }
        else {
            CameraShakerStopper();
        }
    }

    public void StopShaking (float time = 0.5f) {
        if (shakePower > 0) {
            shakePower = 0;
            Invoke("CameraShakerStopper", time);
        }
    }

    void CameraShaker() {
        if (shakePower > 0) {
            Debug.Log("Shaking Cam!!");
            float quakePower = 2f * Random.value * shakePower - shakePower;
            Vector3 displacedPosition = mainCam.transform.position;
            displacedPosition.x += quakePower * 0.5f * (Random.value > 0.5f ? -1 : 1);
            displacedPosition.y += quakePower * 0.5f * (Random.value > 0.5f ? -1 : 1);          
            mainCam.transform.position = displacedPosition;
        }
    }

    void CameraShakerStopper() {
        Debug.Log("Stopping Cam Shake!!");
        CancelInvoke("CameraShaker");
        mainCam.transform.position = originalCamPosition;
    }

}
