using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mech : MonoBehaviour {
	public float mechSpeed = 2.0f;
 	public float jumpPower = 10.0f;
	public bool isOnGround;
	private Rigidbody2D mechRB;

	// Use this for initialization
	void Start () {
		mechRB = GetComponent<Rigidbody2D>();
		
	}
	
	// Update is called once per frame
	public void MechUpdate () {
		transform.position += Vector3.right * Input.GetAxisRaw("Horizontal") * Time.deltaTime * mechSpeed;

		if(Input.GetAxisRaw("Vertical") > 0.0f && isOnGround) {
			mechRB.AddForce(Vector2.up * Input.GetAxisRaw("Vertical") * jumpPower);
			isOnGround = false;
		}
	}

	void OnCollisionEnter2D(Collision2D bumpFacts) {
		for(int i = 0; i < bumpFacts.contacts.Length; i++) {
			if(bumpFacts.contacts[i].normal.y >= 0.9f) {
				isOnGround = true;
				return;
			}
		}
	}
}
