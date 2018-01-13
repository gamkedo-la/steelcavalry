using System;
using System.Collections;
using System.Collections.Generic;
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

    private bool isFacingRight = true;

	public event Action OnFire = delegate {} ; //firing is now an event that can heard by other scripts
	public event Action OnAltFire = delegate {} ; //firing is now an event that can heard by other scripts
	public event Action OnAltFire2 = delegate {} ; //firing is now an event that can heard by other scripts

	private Jetpack jetpack;

	public enum PlayerState{
		inMech,
		outOfMech
	}
	public PlayerState _state;

	// Use this for initialization
	void Start () {
		mechOnlyMask = LayerMask.GetMask("Mech");
		rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
		mainCam = Camera.main;
		camScript = mainCam.GetComponent<MainCamera>();

		_state = PlayerState.outOfMech; //default player state, switches between in and out of mech

		jetpack = GetComponent<Jetpack>();
    }

	void EnterMech(Mech mech){
		if (mech.model == null){
			//Debug.Log("Attempt to enter mech with no model. Exiting...");
			Debug.Log("Entering mech with no model... :/");
			//return;
		}
		mech.wasEntered();
		mechImIn = mech;

		//Disables human character
		GetComponent<BoxCollider2D>().enabled = false;
		spriteRenderer.enabled = false;
		rb.gravityScale = 0;
		jetpack.JetpackToggle(false);

		camScript.MechZoom(mechImIn);

		_state = PlayerState.inMech; //changes player state

	}

	void ExitMech(){
		transform.position += Vector3.up * mechImIn.transform.lossyScale.y * 0.5f;
		mechImIn.wasExited();
		mechImIn = null;

		//Enable human character
		GetComponent<BoxCollider2D>().enabled = true;
		spriteRenderer.enabled = true;
		rb.gravityScale = 1.0f;

		camScript.MechZoom(); //default cam size

		_state = PlayerState.outOfMech;
	}

	// Update is called once per frame
	void Update () {

		//Common to both in and out of mech; prob will be changed later
		if (Input.GetAxisRaw("Horizontal") > 0.0f && !isFacingRight) {
			if ( _state == PlayerState.inMech && Input.GetMouseButton( 0 ) )
				isFacingRight = false;
			else
				isFacingRight = true;
		} else if(Input.GetAxisRaw("Horizontal") < 0.0f && isFacingRight) {
			if ( _state == PlayerState.inMech && Input.GetMouseButton( 0 ) )
				isFacingRight = true;
			else
				isFacingRight = false;
		}
		if (Input.GetMouseButton(0)){
			OnFire(); //tells everyone listening that a shot has been fired
		}
		if ( Input.GetMouseButton( 1 ) )
		{
			OnAltFire( ); //tells everyone listening that a shot has been fired
		}
		if ( Input.GetMouseButtonDown( 2 ) )
		{
			OnAltFire2( ); //tells everyone listening that a shot has been fired
		}

		switch (_state){

			// Update method for when inside mech
			case PlayerState.inMech:
				if(mechImIn && mechImIn.model != null) {
					if(isFacingRight) {
						mechImIn.model.rotation = Quaternion.LookRotation(Vector3.right);
						mechImIn.Side( true );
				} else {
						mechImIn.model.rotation = Quaternion.LookRotation(Vector3.left);
						mechImIn.Side( false );
				}
				}

				transform.position = mechImIn.transform.position;
				rb.velocity = Vector2.zero;
				if (Input.GetKeyDown(KeyCode.Space)) ExitMech();

			break;

			//Update method for outside mech
			case PlayerState.outOfMech:
				if (Input.GetKeyDown(KeyCode.Space)){
					Mech nearestMech = FindNearbyMech();
					if (nearestMech) EnterMech(nearestMech);
				}
					transform.position += Vector3.right * Input.GetAxisRaw("Horizontal") * Time.deltaTime * humanSpeed;

					spriteRenderer.flipX = !isFacingRight;

					if (Input.GetAxisRaw("Vertical") > 0.0f) {
						jetpack.JetpackToggle(true);
						transform.position += Vector3.up * Time.deltaTime * jetPackPower;
						rb.gravityScale = 0.0f;
						rb.velocity = Vector2.zero;
					} else {
						jetpack.JetpackToggle(false);
						rb.gravityScale = 1.0f;
					}
				break;

			default: return;

		}
	} // end of Update

	//returns nearest mech in range or null if there are none
	Mech FindNearbyMech(){
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
			return mScript; //we found a mech!
			}
			else{
				Debug.Log("Mech script not found on nearest collider, check mechOnlyMask");
				return null;
			}
		}
		else return null; //no mech in range
	}
} // class
