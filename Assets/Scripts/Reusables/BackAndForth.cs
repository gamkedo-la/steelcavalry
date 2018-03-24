using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BackAndForth : MonoBehaviour {

	private float xMin, xMax; 
	private float yMin, yMax;
	private float xStart, yStart;


	public float range;

	public float moveSpeed;

	public float angle;
	private float sin, cos;

	private bool facingRight = true;
	private bool facingUp = true;

	void OnEnable () {

			if (range == 0) Debug.Log("Your BackAndForth script isn't doing anything! :O");

			xStart = transform.position.x;
			yStart = transform.position.y;

			sin = Mathf.Sin(angle*Mathf.Deg2Rad);
			cos = Mathf.Cos(angle*Mathf.Deg2Rad);

			xMin = xStart - Mathf.Abs(range*cos/2);
			xMax = xStart +Mathf.Abs(range*cos/2);
			yMin = yStart - Mathf.Abs(range*sin/2);
			yMax = yStart + Mathf.Abs(range*sin/2);

	}

	void FixedUpdate () {

		CheckAndFlip();
		if (xMin != xMax) transform.position += Vector3.right*cos*moveSpeed*Time.deltaTime*(facingRight ? 1 : -1);
		if (yMin != yMax) transform.position += Vector3.up*sin*moveSpeed*Time.deltaTime*(facingUp ? 1 : -1);
	}

	void CheckAndFlip(){
		if
		(((transform.position.x > xMax && facingRight) ||
		(transform.position.x < xMin && !facingRight))
		||
		((transform.position.y > yMax && facingRight) ||
		(transform.position.y < yMin && !facingRight)))
		{
			facingRight = !facingRight; //flip it!
			facingUp = !facingUp;
		}
	}

	//Change direction if we hit wall
	void OnCollisionEnter2D(Collision2D bumpFacts){

		if (bumpFacts.gameObject.CompareTag("Ground")){

			for(int i = 0; i < bumpFacts.contacts.Length; i++) {

				if(Mathf.Abs(bumpFacts.contacts[i].normal.x) >= 0.9f) {
					facingRight = !facingRight;
					return;
				}
				if(Mathf.Abs(bumpFacts.contacts[i].normal.y) >= 0.9f) {
					facingUp = !facingUp;
					return;
				}
			}
		}
	}
}
