using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targeting : MonoBehaviour {

	//private CircleCollider2D fov;

	public GameObject currentTarget; 

	// Use this for initialization
	void Start () {
		//fov = GetComponent<CircleCollider2D>();
	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnTriggerEnter2D(Collider2D other){
		if (other.gameObject.CompareTag("Player")){
			Debug.Log("Found the player!!!!");
			currentTarget = other.gameObject; //locked on the player
		}
	}

	void OnTriggerExit2D(Collider2D other){
		if (other.gameObject == currentTarget){
			Debug.Log("I lost'em....");
			currentTarget = null;
		}
	}
}
