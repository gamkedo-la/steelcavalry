using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveOnTrigger : MonoBehaviour {

	public GameObject toMove;

	public Transform initialPos;
	public Transform endPos;
	private Transform target;

	public float moveSpeed;


	// Use this for initialization
	void Start () {
		
		target = initialPos;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
		float delta = moveSpeed * Time.deltaTime;
		toMove.transform.position = Vector3.MoveTowards(toMove.transform.position, target.position, delta);
	}

	void OnTriggerEnter2D(Collider2D other){

		target = endPos;

	}

	void OnTriggerExit2D(){

		target = initialPos;
		
	}
}
