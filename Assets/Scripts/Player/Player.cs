using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[RequireComponent(typeof(Jetpack), typeof(WeaponManager), typeof(HP))]
public class Player : MonoBehaviour {
	[SerializeField] private PlayerHealthUI playerHealthUI = null;

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

    public Transform weaponFiringPoint;
    public WeaponManager weaponManager;
    public GameObject weaponEquipped;

	// public input flags for keyboard/gamepad *or* AI
	public bool useKeyboardInput = true; // default for player 1, false for bots
	public bool useGamepadInput = false; // optional gamepad support for players 2..n
	public int playerNumber = 1; // easy local multiplayer woo hoo used by gamepad stuff

	// flags set by either the AI, keyboard, or gamepad
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
		mechOnlyMask = LayerMask.GetMask("Mechs");
		rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
		mainCam = Camera.main;
		camScript = mainCam.GetComponent<MainCamera>();

		_state = PlayerState.outOfMech; //default player state, switches between in and out of mech

		jetpack = GetComponent<Jetpack>();

        IWeapon weapon = weaponEquipped.GetComponent<PlayerMachineGun>();
        weaponManager.GiveWeapon(weapon);

        EnableWeapons(true);
    }

	void EnterMech(Mech mech){
		if (mech.model == null){
			//Debug.Log("Attempt to enter mech with no model. Exiting...");
			//Debug.Log("Entering mech with no model... :/");
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

        EnableWeapons(false);

        playerHealthUI.SetHealthVisibility(false);

        _state = PlayerState.inMech; //changes player state
	}

	void ExitMech()
	{
		if ( mechImIn == null ) return;

		transform.position += Vector3.up * mechImIn.transform.lossyScale.y * 0.5f;
		mechImIn.wasExited();
		mechImIn = null;

		//Enable human character
		GetComponent<BoxCollider2D>().enabled = true;
		spriteRenderer.enabled = true;
		rb.gravityScale = 1.0f;

		camScript.MechZoom(); //default cam size

        EnableWeapons(true);

        playerHealthUI.SetHealthVisibility(true);

        _state = PlayerState.outOfMech;
	}

	// putting all input response here lets us turn it off for "dumb players" ie AI
	void handleInput() {

		if (useKeyboardInput) {
			inputUp = Input.GetAxisRaw ("Vertical") > 0.0f;
			inputDown = Input.GetAxisRaw ("Vertical") < 0.0f;
			inputRight = Input.GetAxisRaw ("Horizontal") > 0.0f;
			inputLeft = Input.GetAxisRaw ("Horizontal") < 0.0f;
			inputFire = Input.GetMouseButton(0);
			inputAltFire = Input.GetMouseButton(1);
			inputAltFire2 = Input.GetKeyDown(KeyCode.Q);
			inputEnter = Input.GetKeyDown(KeyCode.Space);
		}

		if (useGamepadInput) {

			//A Button joystick button 0
			//B Button joystick button 1
			//X Button joystick button 2
			//Y Button joystick button 3
			//L Button joystick button 4
			//R Button joystick button 5
			//Back joystick button 6
			//Start joystick button 7
			//Left Analog pressed joystick button 8
			//Right Analog pressed joystick button 9
			//Left/Right on D-Pad Joystick Axis, Axis 6
			//Up/Down on D-Pad Joystick Axis, Axis 7
			//Left Trigger and Right Trigger both correspond to joystick axis, axis 3
			//D-pad up: joystick button 5
			//D-pad down: joystick button 6
			//D-pad left: joystick button 7
			//D-pad right: joystick button 8

			/*
			// can't reference inputs by string - MUST click it together in unity input editor
			inputFire = Input.GetKey ("joystick " + gamepadNumber + " button 0");
			inputAltFire = Input.GetKey ("joystick " + gamepadNumber + " button 1");
			inputAltFire2 = Input.GetKey ("joystick " + gamepadNumber + " button 2");
			inputEnter = Input.GetKey ("joystick " + gamepadNumber + " button 3");
			inputLeft = Input.GetAxisRaw ("joystick " + gamepadNumber + " axis 1") < -0.1f;
			inputRight = Input.GetAxisRaw ("joystick " + gamepadNumber + " axis 1") > 0.1f;
			inputUp = Input.GetAxisRaw ("joystick " + gamepadNumber + " axis 2") < -0.1f;
			inputDown = Input.GetAxisRaw ("joystick " + gamepadNumber + " axis 2") > 0.1f;
			*/

			// todo: d-pad and right thumbstick aiming etc

			// need to manually put stuff like "gamepad1updown" in the unity input manager
			inputUp = Input.GetAxis("player"+playerNumber+"updown") < -0.5f;
			inputDown = Input.GetAxis("player"+playerNumber+"updown") > 0.5f;
			inputLeft = Input.GetAxis("player"+playerNumber+"leftright") < -0.5f;
			inputRight = Input.GetAxis("player"+playerNumber+"leftright") > 0.5f;

			inputFire = Input.GetButton("player"+playerNumber+"fire1");
			inputAltFire = Input.GetButton("player"+playerNumber+"fire2");
			inputAltFire2 = Input.GetButton("player"+playerNumber+"fire3");
			inputEnter = Input.GetButton("player"+playerNumber+"jump");

			// debug spam
			//Debug.Log("player"+playerNumber+"updown="+Input.GetAxis("player"+playerNumber+"updown"));
			//Debug.Log("player"+playerNumber+"updown="+Input.GetAxisRaw("player"+playerNumber+"updown"));


		}

	}

    void EnableWeapons(bool enabled) {
        if (weaponManager != null) {
            if (enabled) {
                if (gameObject.tag == "Player") {
                    weaponManager.IsPlayerDriving(true);
                }

                weaponManager.IsActive(true);
                OnFire += weaponManager.FirePrimary;
                OnAltFire += weaponManager.FireSecondary;
                OnAltFire2 += weaponManager.FireTertiary;
            }
            else {
                OnFire -= weaponManager.FirePrimary;
                OnAltFire -= weaponManager.FireSecondary;
                OnAltFire2 -= weaponManager.FireTertiary;
                weaponManager.IsActive(false);

                if (gameObject.tag == "Player") {
                    weaponManager.IsPlayerDriving(true);
                }
            }
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

        if (weaponManager != null) {
            weaponManager.SetDir(isFacingRight);
        }

        if (isFacingRight) {
            weaponFiringPoint.transform.localPosition = new Vector3(0.092f, 0.0337f, 0);
        }
        else if (!isFacingRight) {
            weaponFiringPoint.transform.localPosition = new Vector3(-0.092f, 0.0337f, 0);
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
                    Quaternion facingDirection;

                    if (isFacingRight) {
                        facingDirection = Quaternion.LookRotation(Vector3.right);
                        mechImIn.Side( true );
					}
                    else {
                        facingDirection = Quaternion.LookRotation(Vector3.left);
                        mechImIn.Side( false );
					}
                    mechImIn.model.rotation = Quaternion.Slerp(mechImIn.model.rotation, facingDirection, mechImIn.mechRotateSpeed * Time.deltaTime);
                }

				if ( mechImIn != null )
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

			//transform.position += Vector3.right * horizImpulse /*Input.GetAxisRaw("Horizontal")*/ * Time.deltaTime * humanSpeed;
            rb.velocity = new Vector2(horizImpulse * Time.deltaTime * humanSpeed, rb.velocity.y);

			spriteRenderer.flipX = !isFacingRight;

			if (inputUp) {
				jetpack.JetpackToggle(true);
				//transform.position += Vector3.up * Time.deltaTime * jetPackPower;
                rb.velocity = new Vector2(rb.velocity.x, Time.deltaTime * jetPackPower);
                rb.gravityScale = 1.0f;
				//rb.velocity = Vector2.zero;
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

		nearbyMechs = nearbyMechs.Select( m => m ).Where( m => m.gameObject.CompareTag( "Mech" ) ).ToArray( );

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
