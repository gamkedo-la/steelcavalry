﻿using System;
//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
	public float humanSpeed = 0.8f;
	public float mechNearEnoughToUseDistance = 1.0f;
	// public float exitMechDistancePopUp = 1.1f;
	public float jetPackPower = 1.0f;

	private Mech mechImIn = null;
	private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
	private Camera mainCam;
	private MainCamera camScript;

	private int mechOnlyMask;

	[SerializeField]
    private bool isFacingRight = true;

	private bool fireWeapon;
	public event Action OnFire = delegate {} ; //firing is now an event that can heard by other scripts

	public ParticleSystem psScriptSmoke;
	private ParticleSystem.EmissionModule jetpackSmoke;
	private ParticleSystem.MinMaxCurve emissionWhenFiringJetpackSmoke;

	public ParticleSystem psScriptThrust;
	private ParticleSystem.EmissionModule jetpackThrust;
	private ParticleSystem.MinMaxCurve emissionWhenFiringJetpackThrust;

	// Use this for initialization
	void Start () {
		mechOnlyMask = LayerMask.GetMask("Mech");
		rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
		mainCam = Camera.main;
		camScript = mainCam.GetComponent<MainCamera>();

		jetpackSmoke = psScriptSmoke.emission;
		emissionWhenFiringJetpackSmoke = jetpackSmoke.rateOverTime;
		jetpackSmoke.rateOverTime = 0;

		jetpackThrust = psScriptThrust.emission;
		emissionWhenFiringJetpackThrust = jetpackThrust.rateOverTime;
		jetpackThrust.rateOverTime = 0;
    }

	void EnteringOrLeavingMech(Mech nextMech) {
		if(mechImIn != null && mechImIn.model != null) {
			mechImIn.model.rotation = Quaternion.LookRotation(Vector3.forward);
		}

		if (mechImIn) mechImIn.wasExited();
		if (nextMech) nextMech.wasEntered();
		mechImIn = nextMech;
		

		bool notInMech = (mechImIn == null);

		GetComponent<BoxCollider2D>().enabled = notInMech;
		spriteRenderer.enabled = notInMech;
		rb.gravityScale = (notInMech ? 1.0f : 0.0f);
		if(notInMech) {
			camScript.MechZoom(); //empty for default
			//targetCamZoomSize = defaultCamZoomSize;
		} else {
			//float targetCamZoomSize = Mathf.Max(defaultCamZoomSize, mechImIn.transform.lossyScale.y);
			camScript.MechZoom(mechImIn);
			jetpackThrust.rateOverTime = jetpackSmoke.rateOverTime = 0;
		}
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetAxisRaw("Horizontal") > 0.0f && !isFacingRight) {
			isFacingRight = true;
		} else if(Input.GetAxisRaw("Horizontal") < 0.0f && isFacingRight) {
			isFacingRight = false;
		}
		if (Input.GetMouseButton(0)){
			OnFire(); //tells everyone listening that a shot has been fired
		}

		if(mechImIn) {
			if(mechImIn.model != null) {
				if(isFacingRight) {
					mechImIn.model.rotation = Quaternion.LookRotation(Vector3.right);
				} else {
					mechImIn.model.rotation = Quaternion.LookRotation(Vector3.left);
				}
			}
			//mechImIn.MechUpdate(); //now Mech has standard Update()
			rb.velocity = Vector2.zero;
			transform.position = mechImIn.transform.position;
		} else {
			transform.position += Vector3.right * Input.GetAxisRaw("Horizontal") * Time.deltaTime * humanSpeed;

			spriteRenderer.flipX = !isFacingRight;

            if (Input.GetAxisRaw("Vertical") > 0.0f) {
				jetpackSmoke.rateOverTime = emissionWhenFiringJetpackSmoke;
				jetpackThrust.rateOverTime = emissionWhenFiringJetpackThrust;
				transform.position += Vector3.up * Time.deltaTime * jetPackPower;
				rb.gravityScale = 0.0f;
				rb.velocity = Vector2.zero;
			} else {
				jetpackThrust.rateOverTime = jetpackSmoke.rateOverTime = 0;
				rb.gravityScale = 1.0f;
			}
		}

		if(Input.GetKeyDown(KeyCode.Space)) {

			if(mechImIn) {
				transform.position += Vector3.up * mechImIn.transform.lossyScale.y * 0.5f;
				EnteringOrLeavingMech(null);
			} else { // entering mech
				Collider2D[] nearbyMechs = Physics2D.OverlapCircleAll(transform.position,
													mechNearEnoughToUseDistance,
													mechOnlyMask);
				float nearestMechDist = 9000.0f;
				Collider2D nearestMechCollider = null;
				for(int i = 0; i < nearbyMechs.Length; i++) {
					float distToMech = Vector2.Distance(transform.position,
						                   nearbyMechs[i].transform.position);
					if(distToMech < nearestMechDist) {
						nearestMechDist = distToMech;
						nearestMechCollider = nearbyMechs[i];
					}
				}
				if(nearestMechCollider) {
					Mech mScript = nearestMechCollider.GetComponent<Mech>();
					if(mScript) {
						EnteringOrLeavingMech(mScript);
					} else {
						Debug.Log("Mech script not found on nearest collider, check mechOnlyMask");
					}

				} // if nearestMechCollider
			} // else (entering mech)
		} // input to get in mech
	} // playerMovement function

} // class
