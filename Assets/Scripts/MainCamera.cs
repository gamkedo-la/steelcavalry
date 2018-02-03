using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour {
    public float outMechCamZoomSize = 20;
    public float inMechCamZoomSize = 60;
    public float zoomSpeed = 3;
    public float followSpeed = 3;
  
    private float targetCamZoomSize;
	private Camera mainCam;	
    
	public GameObject myPlayer;    

	// Use this for initialization
	void Start () {
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
}
