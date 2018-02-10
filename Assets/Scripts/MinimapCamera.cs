using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    public float followSpeed = 3;

    public GameObject followTarget;
    
    private Camera minimapCam;

    // Use this for initialization
    void Start() {
        minimapCam = GetComponent<Camera>();        	
    }

    // Update is called once per frame
    void LateUpdate() {
        // Follows player around
        Vector3 targetPosition = new Vector3(followTarget.transform.position.x, followTarget.transform.position.y, -10);
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }
}
