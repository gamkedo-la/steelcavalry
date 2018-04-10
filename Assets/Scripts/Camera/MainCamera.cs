using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour {
    public float outMechCamZoomSize = 45;
    public float inMechCamZoomSize = 60;
    public float zoomSpeed = 3;
    public float followSpeed = 3;
    public float shakePower = 0;
    public bool canShake = true;
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

        // Follows player around        
        Vector3 targetPosition = new Vector3(myPlayer.transform.position.x, myPlayer.transform.position.y, -10);
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

        if (shakePower > 0) {
            shakePower -= 50f * Time.deltaTime;
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
        if (canShake) {
            if (shakePower <= 0) {
                originalCamPosition = mainCam.transform.position;
                shakePower = shakeValue;
                InvokeRepeating("CameraShaker", wait, rate);
            }
            else {
                CameraShakerStopper();
            }
        }
    }

    public void StopShaking (float time = 0.1f) {
        if (shakePower > 0) {
            shakePower = 0;
            Invoke("CameraShakerStopper", time);
        }
    }

    void CameraShaker() {
        if (shakePower > 0) {
            float quakePower = (Random.value > 0.5f ? -1 : 1) * shakePower - shakePower;
            Vector3 displacedPosition = mainCam.transform.position;

            Random.InitState(Mathf.RoundToInt(10000 * Time.deltaTime));                                    
            displacedPosition.x += 0.1f * quakePower * (Random.value > 0.5f ? -1 : 1);

            Random.InitState(Mathf.RoundToInt(10000 * Time.deltaTime));
            displacedPosition.y += 0.1f * quakePower * (Random.value > 0.5f ? -1 : 1);          

            mainCam.transform.position = displacedPosition;
        }
    }

    void CameraShakerStopper() {
        CancelInvoke("CameraShaker");
        mainCam.transform.position = originalCamPosition;
    }
}
