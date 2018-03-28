using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveOnTrigger : MonoBehaviour {

	public Transform initialPos;
	public Transform endPos;
	private Transform target;

	public float moveSpeed;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
		float delta = moveSpeed * Time.deltaTime;
		transform.position = Vector3.MoveTowards(transform.position, target.position, delta);
	}

	void OnTriggerEnter2D(Collider2D other){

		Debug.Log("Triggered");
		target = endPos;

	}

	void OnTriggerExit2D(){
		Debug.Log("Exit");
		target = initialPos;
	}
}
