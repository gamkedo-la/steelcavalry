using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinAndDisableRenderer : MonoBehaviour {
	public float rotationSpeed;

	// Use this for initialization
	void Start () {
		Renderer rend = GetComponent<Renderer>();
		if(rend) {
			rend.enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate (Vector3.up * Time.deltaTime * rotationSpeed);
	}
}
