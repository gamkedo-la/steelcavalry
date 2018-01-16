using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This AI class is like a "dumb terminal" in a good way;
// it only interfaces with the game the same way the player does:
// input booleans! up down left right and the fire buttons

[RequireComponent(typeof(PlayerMovement))]

public class AI : MonoBehaviour {

	public float minTimePerThink = 0.5f;
	public float maxTimePerThink = 1.0f;
	public float chanceItMoves = 0.5f;
	public float chanceItFires = 0.3f;
	public float chanceItEnters = 0.2f;

	PlayerMovement myMovement;

	// Use this for initialization
	void Start () {
		myMovement = GetComponent<PlayerMovement>();
		StartCoroutine("aiThink");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	IEnumerator aiThink() {

		float randy;

		for(;;) {
			// reset
			myMovement.inputUp = false;
			myMovement.inputDown = false;
			myMovement.inputLeft = false;
			myMovement.inputRight = false;
			myMovement.inputFire = false;
			myMovement.inputAltFire = false;
			myMovement.inputAltFire2 = false;

			//Debug.Log("aiThink");

			if (Random.value < chanceItMoves) 
			{
				randy = Random.value;
				if (randy < 0.25f) {
					myMovement.inputUp = true;
				} else if (randy < 0.5f) {
					myMovement.inputDown = true;
				} else if (randy < 0.75f) {
					myMovement.inputLeft = true;
				} else {
					myMovement.inputRight = true;
				} // other values: do nothing
			}

			if (Random.value < chanceItFires) {
				randy = Random.value; // fire?
				if (randy < 0.333f) {
					myMovement.inputFire = true;
				} else if (randy < 0.666f) {
					myMovement.inputAltFire = true;
				} else {
					myMovement.inputAltFire2 = true;
				}
			}

			if (Random.value < chanceItEnters) {		
				myMovement.inputEnter = true;
			}

			yield return new WaitForSeconds(minTimePerThink + Random.value * (maxTimePerThink - minTimePerThink) );
		}
	}
}
