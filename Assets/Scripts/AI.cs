using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This AI class is like a "dumb terminal" in a good way;
// it only interfaces with the game the same way the player does:
// input booleans! up down left right and the fire buttons

[RequireComponent(typeof(PlayerMovement))]

public class AI : MonoBehaviour {

	public float minTimePerThink = 0.5f;
	public float maxTimePerThink = 1.5f;
	public float chanceItMoves = 0.75f;
	public float chanceItFires = 0.3f;
	public float chanceItEnters = 0.2f;
	public float chanceItExits = 0.01f;
	public float distanceTolerance = 0.5f; // close enough in world units
	public float unitsAboveTarget = 1.0f; // try to move "above" the target y (good for getting on top of mech)

	public GameObject seekTargetOutside;
	public GameObject seekTargetInMech;
	private GameObject seekTarget;

	PlayerMovement myMovement;

	// Use this for initialization
	void Start () {
		myMovement = GetComponent<PlayerMovement>();
		StartCoroutine("aiThink");
	}
	
	// Update is called once per frame
	void Update () {

		if (!myMovement)
			return;
		
		if (myMovement._state == PlayerMovement.PlayerState.outOfMech)
			seekTarget = seekTargetOutside;

		if (myMovement._state == PlayerMovement.PlayerState.inMech)
			seekTarget = seekTargetInMech;

		// debug lines
		Debug.DrawLine(this.transform.position, new Vector3(seekTarget.transform.position.x,seekTarget.transform.position.y+unitsAboveTarget,seekTarget.transform.position.z), Color.red);

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
			myMovement.inputEnter = false;

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

			// fixme: we also need to choose where to aim: not the mouse cursor!
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

			if (myMovement._state == PlayerMovement.PlayerState.outOfMech) {
				myMovement.inputEnter = (Random.value < chanceItEnters); // maybe hop into a mech
			}

			if (myMovement._state == PlayerMovement.PlayerState.inMech) {
				myMovement.inputEnter = (Random.value < chanceItExits); // rarely exit the mech we're in
			}

			// simple "hack"
			// if we're seeking something,
			// and happen to have decided to move horizontally
			// be sure to choose moving towards it
			// fixme: could be fear-dependent etc
			if (seekTarget) {
				if (myMovement.inputLeft || myMovement.inputRight || myMovement.inputUp) {
					
					if (seekTarget.transform.position.x < this.transform.position.x-distanceTolerance) { // is the target left of me?
						myMovement.inputLeft = true;
						myMovement.inputRight = false;
					} else if (seekTarget.transform.position.x > this.transform.position.x+distanceTolerance) { // target is to the right of me
						myMovement.inputLeft = false;
						myMovement.inputRight = true;
					}
					else // it is nearby
					{
					}

					// let's try gaining altitude when required as well (but now the movement is barely random at all)
					if ((seekTarget.transform.position.y+unitsAboveTarget) > this.transform.position.y) { // is the target above me?
						myMovement.inputUp = true;
					}

				}
			}

			yield return new WaitForSeconds(minTimePerThink + Random.value * (maxTimePerThink - minTimePerThink) );
		}
	}
}
