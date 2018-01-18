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

	// public input flags for keyboard/gamepad *or* AI
	public bool useKeyboardInput = true; // default for player 1, false for bots
	public bool useGamepadInput = false; // unimplemented
	public bool inputUp = false;
	public bool inputDown = false;
	public bool inputLeft = false;
	public bool inputRight = false;
	public bool inputFire = false;
	public bool inputAltFire = false;
	public bool inputAltFire2 = false;
	public bool inputEnter = false;

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
		mech.wasEntered(this.transform.gameObject); // tell the mech who is driving
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

	// putting all input response here lets us turn it off for "dumb players" ie AI
	void handleInput() {

		if (useKeyboardInput) {
			inputRight = Input.GetAxisRaw ("Horizontal") > 0.0f;
			inputLeft = Input.GetAxisRaw ("Horizontal") < 0.0f;
			inputFire = Input.GetMouseButton(0);
			inputAltFire = Input.GetMouseButton(1);
			inputAltFire2 = Input.GetMouseButton(2);
			inputEnter = Input.GetKeyDown(KeyCode.Space);
			inputUp = Input.GetAxisRaw ("Vertical") > 0.0f;
		}

	}

	// Update is called once per frame
	void Update () {

		handleInput();

		//Common to both in and out of mech; prob will be changed later
		if (inputRight && !isFacingRight) {
			if ( _state == PlayerState.inMech && inputFire)
				isFacingRight = false;
			else
				isFacingRight = true;
		} else if(inputLeft && isFacingRight) {
			if ( _state == PlayerState.inMech && inputFire)
				isFacingRight = true;
			else
				isFacingRight = false;
		}
		if (inputFire){
			OnFire(); //tells everyone listening that a shot has been fired
		}
		if (inputAltFire)
		{
			OnAltFire( ); //tells everyone listening that a shot has been fired
		}
		if (inputAltFire2)
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
				if (inputEnter) ExitMech();

			break;

			//Update method for outside mech
		case PlayerState.outOfMech:
			if (inputEnter) {
				Mech nearestMech = FindNearbyMech ();
				if (nearestMech)
					EnterMech (nearestMech);
			}

			float horizImpulse = 0f;
			if (inputLeft) horizImpulse = -1f;
			else if (inputRight) horizImpulse = 1f;

			transform.position += Vector3.right * horizImpulse /*Input.GetAxisRaw("Horizontal")*/ * Time.deltaTime * humanSpeed;

				spriteRenderer.flipX = !isFacingRight;

				if (inputUp) {
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

	public string getNameOfMechPlayerIsIn() {
		if(!mechImIn) return "";
		return mechImIn.name;
	}

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
