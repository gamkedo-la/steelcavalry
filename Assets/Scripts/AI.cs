﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This AI class is like a "dumb terminal" in a good way;
// it only interfaces with the game the same way the player does:
// Player.cs input booleans! up down left right and the fire buttons

[RequireComponent(typeof(Player))]

public class AI : MonoBehaviour {

[Header("Emotional State")]
	[Tooltip("Fluctuating values based on game events")]
	[Range(0.0f, 1.0f)]
	public float confidence = 0.0f;
	[Range(0.0f, 1.0f)]
	public float boredom = 0.0f;
	[Range(0.0f, 1.0f)]
	public float anger = 0.0f;
	[Range(0.0f, 1.0f)]
	public float fear = 0.0f;

[Header("Personality Modifiers")]
	[Range(0.0f, 1.0f)]
	public float chanceItMoves = 0.75f;
	[Range(0.0f, 1.0f)]
	public float chanceItFires = 0.3f;
	[Range(0.0f, 1.0f)]
	public float chanceItEnters = 0.2f;
	[Range(0.0f, 1.0f)]
	public float chanceItExits = 0.01f;
	[Range(0.0f, 1.0f)]
	public float emotionalInstability = 0.002f;
	[Range(0.0f, 1.0f)]
	public float boredomInstabilityBoost = 0.25f;

[Header("Recouperation Over Time")]
	[Range(-1.0f, 1.0f)]
	public float confidencePerSec = 0.1f;
	[Range(-1.0f, 1.0f)]
	public float boredomPerSec = 0.25f;
	[Range(-1.0f, 1.0f)]
	public float angerPerSec = -0.1f;
	[Range(-1.0f, 1.0f)]
	public float fearPerSec = -0.1f;

[Header("Emotional Reactions")]
	[Range(-1.0f, 1.0f)]
	public float confidenceWhenHit = -0.5f;
	[Range(-1.0f, 1.0f)]
	public float boredomWhenHit = -1.0f;
	[Range(-1.0f, 1.0f)]
	public float angerWhenHit = 0.1f;
	[Range(-1.0f, 1.0f)]
	public float fearWhenHit = 0.3f;

[Header("Times and Distances")]
	public float minTimePerThink = 0.5f; // in seconds
	public float maxTimePerThink = 1.5f;
	public float distanceTolerance = 0.5f; // close enough in world units
	public float unitsAboveTarget = 1.0f; // try to move "above" the target y (good for getting on top of mech)

[Header("Vision Scanner")]
    public float scannerLength = 5.0f;
    public float scannerMaxRotation = 60.0f;
    public float scannerRotateSpeed = 10.0f;
    public int scannedCount = 5;
    public LayerMask visibleToMe;    
    private GameObject scanner;
    private GameObject pointInArc;
    private RaycastHit2D[] seenByMe;

[Header("Target GameObjects")]    
    public GameObject seekTargetOutside;
	public GameObject seekTargetInMech;
	private GameObject seekTarget;

	private Player myMovement; // so we can access the input boolean flags    

    // Use this for initialization
    void Start () {
		myMovement = GetComponent<Player>();
		StartCoroutine("aiThink");

        // Vision Scanner, this rotates
        scanner = new GameObject("Vision Scanner");
        Transform thisTransform = this.gameObject.transform;
        scanner.transform.position = thisTransform.position;
        scanner.transform.parent = thisTransform;        

        // A point in the arc of the vision scanner, parented under Vision Scanner to rotate along the arc
        pointInArc = new GameObject("Point in Arc");
        Transform scannerTransform = scanner.gameObject.transform;        
        float leftOrRight = myMovement.isFacingRight ? 1 : -1;
        pointInArc.transform.position = new Vector3(scannerTransform.position.x + leftOrRight * scannerLength, 
                                                    scanner.transform.position.y);        
        pointInArc.transform.parent = scannerTransform;        
    }

    public RaycastHit2D[] GetScannedGameObjects() {
        Vector2 rayDirection = (pointInArc.transform.position - scanner.transform.position).normalized;

		RaycastHit2D[] results = new RaycastHit2D[scannedCount]; // TODO: can we avoid this new() every frame?

        int hit = Physics2D.RaycastNonAlloc(scanner.transform.position,
                                            rayDirection,
                                            results,
                                            scannerLength,
                                            visibleToMe);

        for (int i = 1; i < hit; i++) {  // i starts at 1 to ignore self
            Collider2D resultCollider = results[i].collider;
            if (resultCollider != null) {                
                Debug.Log(gameObject.name + " can see " + resultCollider.name + "!");                
            }
        }
        
        return results;
    }

    public bool hasLineOfSightTo(GameObject toThis)	{
		bool result = false;

        RaycastHit2D[] visible = seenByMe;

        for (int i = 1; i < visible.Length; i++) {  // i starts at 1 to ignore self
            Collider2D visibleCollider = visible[i].collider;
            if (visibleCollider != null && visibleCollider.gameObject == toThis) {                
                Debug.Log("Ah ha!! " + gameObject.name + " is going to do something to " + visibleCollider.name + "!");
                return true;               
            }
        }

        return result;
	}

	float wobble(float timestamp, float offset)
	{
		float amount = (Mathf.PerlinNoise(timestamp+offset,timestamp+offset)*2f-0.5f) // take a timed random wobble from -1 to +1
			* emotionalInstability // how much we should change per second
			* (1f + (boredom * boredomInstabilityBoost)); // and affect emotions a bit (or a lot of we're bored)
		//Debug.Log("wobble="+amount);
		return amount;
	}

	// Update is called once per frame
	void Update () {

		if (!myMovement)
			return;

		// FIXME  - this is called too often - only call every few ms?
		// set the position of the point in arc depending on the facing direction
        Vector3 PIALocalPosition = pointInArc.transform.localPosition;
        PIALocalPosition.x = myMovement.isFacingRight ? Mathf.Abs(PIALocalPosition.x) : -Mathf.Abs(PIALocalPosition.x);        
        pointInArc.transform.localPosition = new Vector3(PIALocalPosition.x, PIALocalPosition.y);        
        // rotate the vision scanner
        scanner.transform.rotation = Quaternion.Euler(0f, 0f, scannerMaxRotation * Mathf.Sin(Time.time * scannerRotateSpeed));
        seenByMe = GetScannedGameObjects();

        Debug.DrawLine(scanner.transform.position, pointInArc.transform.position, Color.red);

		if (myMovement._state == Player.PlayerState.outOfMech)
			seekTarget = seekTargetOutside;

		if (myMovement._state == Player.PlayerState.inMech)
			seekTarget = seekTargetInMech;

		// recouperation over time
		confidence += confidencePerSec * Time.deltaTime;
		boredom += boredomPerSec * Time.deltaTime;
		anger += angerPerSec * Time.deltaTime;
		fear += fearPerSec * Time.deltaTime;

		// emotional instability
		confidence += wobble(Time.time,0f);
		boredom += wobble(Time.time,0.2f);
		anger += wobble(Time.time,0.333f);
		fear += wobble(Time.time,42f);

		// keep in a range
		confidence = Mathf.Clamp(confidence,0f,1f);
		boredom = Mathf.Clamp(boredom,0f,1f);
		anger = Mathf.Clamp(anger,0f,1f);
		fear = Mathf.Clamp(fear,0f,1f);
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

			if (myMovement._state == Player.PlayerState.outOfMech) {
				myMovement.inputEnter = (Random.value < chanceItEnters); // maybe hop into a mech
			}

			if (myMovement._state == Player.PlayerState.inMech) {
				myMovement.inputEnter = (Random.value < chanceItExits); // rarely exit the mech we're in
			}

			// simple "hack"
			// if we're seeking something,
			// and happen to have decided to move horizontally
			// be sure to choose moving towards it
			// fixme: could be fear-dependent etc
			if (seekTarget) {

				bool canSeeTarget = hasLineOfSightTo(seekTarget); // TODO: unimplemented

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

					// are we really scared? then run away instead
					if (fear > 0.5) {
						Debug.Log("Too scared! Running away from target!");
						myMovement.inputLeft = !myMovement.inputLeft;
						myMovement.inputRight = !myMovement.inputRight;
					}

				}
			}

			yield return new WaitForSeconds(minTimePerThink + Random.value * (maxTimePerThink - minTimePerThink) );
		}
	}

	public void aiGotHurtEvent()
	{
		Debug.Log("AI got hurt! Fear and confidence are affected! HP:");
		fear += fearWhenHit; // terror
		confidence += confidenceWhenHit; // wavering
		boredom = 0f; // instant reset: it you get hurt, you get focussed
	}
}
