using UnityEngine;

public class UICamera : MonoBehaviour
{
	Camera mainCam;
	Camera thisCam;

	void Start ()
	{
		mainCam = Camera.main;
		thisCam = GetComponent<Camera>();
	}

	void Update ()
	{
		thisCam.orthographicSize = mainCam.orthographicSize;
		thisCam.fieldOfView = mainCam.fieldOfView;
	}
}
