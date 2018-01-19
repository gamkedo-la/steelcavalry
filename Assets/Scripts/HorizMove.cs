using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HorizMove : MonoBehaviour {

	public float xMin, xMax; //leave blank for auto-generated based on range
	public float range;

	public float moveSpeed;

	private bool facingRight = true;

	// Use this for initialization
	void Start () {

		if (xMin == 0 && xMax == 0){

			//we use range instead
			if (range == 0) Debug.Log("Your HorizMove script isn't doing anything! :O");

			xMin = transform.position.x - range;
			xMax = transform.position.x + range;
		}
	}

	// Update is called once per frame
	void FixedUpdate () {
		CheckAndFlip();
		transform.position += Vector3.right*moveSpeed*Time.deltaTime*(facingRight ? 1 : -1);
	}

	void CheckAndFlip(){
		if ((transform.position.x > xMax && facingRight) ||
		(transform.position.x < xMin && !facingRight)){

			facingRight = !facingRight; //flip it!
			//Debug.Log("Flipped!");
		}
	}
	//Change direction if we see hit wall
	void OnCollisionEnter2D(Collision2D bumpFacts){
		for(int i = 0; i < bumpFacts.contacts.Length; i++) {
			if(Mathf.Abs(bumpFacts.contacts[i].normal.x) >= 0.9f) {
				facingRight = !facingRight;
				return;
			}
		}
	}
}
