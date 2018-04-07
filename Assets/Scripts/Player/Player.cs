using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Jetpack), typeof(WeaponManager), typeof(HP))]
[RequireComponent(typeof(SlopeWalker))]
public class Player : MonoBehaviour {
	[SerializeField] private PlayerHealthUI playerHealthUI = null;
	[SerializeField] private GameEventAudioEvent audioEvent;
    private AbilityIcon firstIcon;
    private AbilityIcon secondIcon;

    public float playerSpeed = 100f;
    public float playerEjectForceMagnitude = 100;
	public float mechNearEnoughToUseDistance = 1.0f;
	// public float exitMechDistancePopUp = 1.1f;
	public float jetPackPower = 1.0f;

	private Rigidbody2D playerBody;
    private SlopeWalker slopeWalker;
    private float oldGravityScale;

    private SpriteRenderer spriteRenderer;

    private Camera mainCam;
	private MainCamera camScript;

    private MouseCursor cursor;

	private int mechOnlyMask;

    public event Action OnFire = delegate {} ;      //firing is now an event that can heard by other scripts
	public event Action OnAltFire = delegate {} ;   //firing is now an event that can heard by other scripts
	public event Action OnAltFire2 = delegate {} ;  //firing is now an event that can heard by other scripts

	private Jetpack jetpack;

    [Header("Player Weapon")]
    private IWeapon weapon;
    public Transform weaponFiringPoint;
    public WeaponManager weaponManager;
    public GameObject weaponEquipped;

    [Header("Player Input")]
    // public input flags for keyboard/gamepad *or* AI
    public bool useKeyboardInput = true; // default for player 1, false for bots
	public bool useGamepadInput = false; // optional gamepad support for players 2..n
	public int gamepadNumber = 1;        // easy local multiplayer woo hoo used by gamepad stuff

	// flags set by either the AI, keyboard, or gamepad
	public bool inputUp = false;
	public bool inputDown = false;
	public bool inputLeft = false;
	public bool inputRight = false;
	public bool inputFire = false;
	public bool inputAltFire = false;
	public bool inputAltFire2 = false;
	public bool inputEnter = false;

    [Header("Player States")]
    public Mech mechImIn = null;
    public bool isOnGround = false;
    public bool isFacingRight = true;
    public bool isAttacking = false;
    private bool jetpackOn = false;
    private bool isAiPlayer = false;
	public enum PlayerState{
		inMech,
		outOfMech
	}
	public PlayerState _state;

	public GameObject enterMechTooltipToHide; // ignored if null for levels besides #1

    // Use this for initialization
    void Start () {
		Assert.IsNotNull( audioEvent );

		firstIcon = GameObject.Find("Main UI/Icon Turret").GetComponent<AbilityIcon>();
		secondIcon = GameObject.Find("Main UI/Icon Thrusters").GetComponent<AbilityIcon>();

		mechOnlyMask = LayerMask.GetMask("Mechs");
		playerBody = GetComponent<Rigidbody2D>();
        slopeWalker = GetComponent<SlopeWalker>();

        spriteRenderer = GetComponent<SpriteRenderer>();
		mainCam = Camera.main;
		camScript = mainCam.GetComponent<MainCamera>();

        cursor = UIResourceManager.MouseCursor; // NOTE: ai bots use this class but do not use mouse cursor

        _state = PlayerState.outOfMech; //default player state, switches between in and out of mech

		jetpack = GetComponent<Jetpack>();

		if (weaponEquipped) { // sanity check: some bots don't have this?
			weapon = weaponEquipped.GetComponent<PlayerMachineGun> ();
			weaponManager.GiveWeapon (weapon);
		}

        isAiPlayer = GetComponent<AI>() != null;

        EnableWeapons(true);
    }

	void EnterMech(Mech mech) {
		if (mech.mechModel == null) {
			//Debug.Log("Attempt to enter mech with no model. Exiting...");
			Debug.Log("Entering mech with no model... :/");
			//return;
		}


		if (mech.driver && !mech.canBeStolen) return;

		// eject the previous pilot
		if (mech.driver &&
			mech.driver.GetInstanceID() != gameObject.GetInstanceID() &&
			mech.canBeStolen) {

			mech.driver.ExitMech();
			mech.ToggleCanBeStolen();
		}

		if(enterMechTooltipToHide) {
			enterMechTooltipToHide.SetActive(false);
			enterMechTooltipToHide = null;
		}

        EnableWeapons(false);

		mech.wasEntered(this.transform.gameObject); // tell the mech who is driving
		mechImIn = mech;

		//Disables human character
		GetComponent<BoxCollider2D>().enabled = false;
		spriteRenderer.enabled = false;
		oldGravityScale = playerBody.gravityScale;
		playerBody.gravityScale = 0;
		jetpack.JetpackToggle(false);

        playerHealthUI.SetHealthVisibility(false);

        _state = PlayerState.inMech; //changes player state

        if (!isAiPlayer) {
			camScript.MechZoom(mechImIn);
			//firstIcon.SetIcon( "turretGatling" );
			//weaponManager.IsPlayerDriving( true );
			secondIcon.SetIcon("exitIcon");
	    }
	}

	public void ExitMech() {
		if ( mechImIn == null ) return;

		//transform.position += Vector3.up * mechImIn.transform.lossyScale.y * 0.5f;

        if (mechImIn.isFacingRight) {
            //transform.position += Vector3.left * mechImIn.transform.lossyScale.y * 0.5f;
            isFacingRight = false;
            weapon.SetDir(false);
            playerBody.AddForce(Vector3.up * playerEjectForceMagnitude * 2f + Vector3.left * playerEjectForceMagnitude);
        }
        else if (!mechImIn.isFacingRight) {
            transform.position += Vector3.right * mechImIn.transform.lossyScale.y * 0.5f;
            isFacingRight = true;
            weapon.SetDir(true);
            playerBody.AddForce(Vector3.up * playerEjectForceMagnitude * 2f + Vector3.right * playerEjectForceMagnitude);
        }        

		mechImIn.wasExited();
		mechImIn = null;

		// enable human character
		GetComponent<BoxCollider2D>().enabled = true;
		spriteRenderer.enabled = true;
		playerBody.gravityScale = oldGravityScale;

        EnableWeapons(true);

        playerHealthUI.SetHealthVisibility(true);

        _state = PlayerState.outOfMech;

        if (!isAiPlayer) {
			camScript.MechZoom(); //default cam size
	        firstIcon.SetIcon("pilotGunIcon");
	        secondIcon.SetIcon("enterIcon");
	    }
	}

	// putting all input response here lets us turn it off for "dumb players" ie AI
	void handleInput() {

		if (useKeyboardInput) {
			inputUp = Input.GetAxisRaw ("Vertical") > 0.1f || Input.GetKey(KeyCode.Space);
			inputDown = Input.GetAxisRaw ("Vertical") < -0.1f;
			inputRight = Input.GetAxisRaw ("Horizontal") > 0.1f;
			inputLeft = Input.GetAxisRaw ("Horizontal") < -0.1f;
			inputFire = Input.GetMouseButton(0);
			inputAltFire = Input.GetMouseButton(1);
			inputAltFire2 = Input.GetKeyDown(KeyCode.Q);
			inputEnter = Input.GetKeyDown(KeyCode.E);
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
			inputUp = Input.GetAxis("player"+gamepadNumber+"updown") < -0.5f;
			inputDown = Input.GetAxis("player"+gamepadNumber+"updown") > 0.5f;
			inputLeft = Input.GetAxis("player"+gamepadNumber+"leftright") < -0.5f;
			inputRight = Input.GetAxis("player"+gamepadNumber+"leftright") > 0.5f;

			inputFire = Input.GetButton("player"+gamepadNumber+"fire1");
			inputAltFire = Input.GetButton("player"+gamepadNumber+"fire2");
			inputAltFire2 = Input.GetButton("player"+gamepadNumber+"fire3");
			inputEnter = Input.GetButton("player"+gamepadNumber+"jump");

			// debug spam
			//Debug.Log("player"+playerNumber+"updown="+Input.GetAxis("player"+playerNumber+"updown"));
			//Debug.Log("player"+playerNumber+"updown="+Input.GetAxisRaw("player"+playerNumber+"updown"));

		}

	}

    void EnableWeapons(bool enabled) {
        if (weaponManager != null) {
            if (enabled) {
                if (!isAiPlayer) {
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

                if (!isAiPlayer) {
                    weaponManager.IsPlayerDriving(true);
                }
            }
        }
    }

    // Update is called once per frame
    void Update () {
		handleInput();

        if (weaponManager != null) {
            weaponManager.SetDir(isFacingRight);
        }

        // flips weapon firing point to a side depending on facing direction
        int dir = isFacingRight ? 1 : -1;
		if (weaponFiringPoint) // sanity check - some bots don't don't have this?
			weaponFiringPoint.transform.localPosition =  new Vector3(dir * 0.092f, 0.0337f, 0);

        if (inputFire || inputAltFire || inputAltFire2) {
			if(cursor != null) {
				isFacingRight = cursor.transform.position.x > transform.position.x;
			} else {
				if(inputRight) { // not setting directly so standing won't flip it
					isFacingRight = true;
				} else if(inputLeft) {
					isFacingRight = false;
				}

			}
            isAttacking = true;

        }
        else {
            isAttacking = false;
        }


        if (inputFire) OnFire();            //tells everyone listening that a shot has been fired
		if (inputAltFire) OnAltFire( );     //tells everyone listening that a shot has been fired
		if (inputAltFire2) OnAltFire2( );   //tells everyone listening that a shot has been fired

        switch (_state) {
            // Update method for when inside mech
            case PlayerState.inMech:

                if (!(inputFire || inputAltFire || inputAltFire2)) {
                    if (inputRight && !isFacingRight) {
                        isFacingRight = true;
                        if (weapon != null) {
                            weapon.SetDir(isFacingRight);
                        }
                    }
                    else if (inputLeft && isFacingRight) {
                        isFacingRight = false;
                        if (weapon != null) {
                            weapon.SetDir(isFacingRight);
                        }
                    }
                }

                if (mechImIn && mechImIn.mechModel != null) {
                    Quaternion facingDirection;
                    facingDirection = Quaternion.AngleAxis(mechImIn.transform.rotation.eulerAngles.z, Vector3.forward);

                    Vector3 facingDir = isFacingRight ? Vector3.right : Vector3.left;
                    facingDirection *= Quaternion.LookRotation(facingDir);

                    mechImIn.Side(isFacingRight);

                    mechImIn.mechModel.rotation = Quaternion.Slerp(mechImIn.mechModel.rotation,
                                                                   facingDirection,
                                                                   mechImIn.mechRotateSpeed * Time.deltaTime);
                }

                if (mechImIn != null) {
                    transform.position = mechImIn.transform.position;
                }

                playerBody.velocity = Vector2.zero;
                if (inputEnter) ExitMech();

                break;

            // Update method for outside mech
            case PlayerState.outOfMech:

                if (!(inputFire || inputAltFire || inputAltFire2)) {
                    if (inputRight && !isFacingRight) {
                        isFacingRight = true;
                        if (weapon != null) {
                            weapon.SetDir(isFacingRight);
                        }
                    } else if (inputLeft && isFacingRight) {
                        isFacingRight = false;
                        if (weapon != null) {
                            weapon.SetDir(isFacingRight);
                        }
                    }
                }

                if (inputEnter) {
                    Mech nearestMech = FindNearbyMech();
                    if (nearestMech) {
                        EnterMech(nearestMech);
                    }
                }

                // inputLeft: -1f; inputRight: 1f; neither: 0f
                float horizontalImpulse = inputLeft ? -1f : inputRight ? 1f : 0f;

                //transform.position += Vector3.right * horizImpulse /*Input.GetAxisRaw("Horizontal")*/ * Time.deltaTime * humanSpeed;
                if (horizontalImpulse != 0) {
                    playerBody.velocity = new Vector2(horizontalImpulse * Time.deltaTime * playerSpeed, playerBody.velocity.y);
                }

			    spriteRenderer.flipX = !isFacingRight;

			    if (inputUp) {
				    isOnGround = false;
				    jetpack.JetpackToggle(true);
                    slopeWalker.isMovingUp = true;

				    if ( !jetpackOn ) {
					    jetpackOn = true;
					    audioEvent.Raise( AudioEvents.PlayerJetpack, transform.position );
					    Invoke( "JetpackSoundOff", 0.2f );
				    }

				    //transform.position += Vector3.up * Time.deltaTime * jetPackPower;
				    playerBody.velocity = new Vector2(playerBody.velocity.x, Time.deltaTime * jetPackPower);
                    //playerBody.gravityScale = 1.0f;
				    //playerBody.velocity = Vector2.zero;
			    }
                else {
				    jetpack.JetpackToggle(false);
                    slopeWalker.isMovingUp = false;
				    //playerBody.gravityScale = 1.0f;
			    }

			    break;

			default: return;
		}

        slopeWalker.isFacingRight = isFacingRight;
	} // end of Update

	public string getNameOfMechPlayerIsIn () {
		if (!mechImIn) return "";
		return mechImIn.name;
	}

	private void JetpackSoundOff ()	{
		jetpackOn = false;
	}

	void OnCollisionEnter2D (Collision2D other) {
		for (int i = 0; i < other.contacts.Length; i++) {
			if (other.contacts[i].normal.y >= 0.9f) {
				isOnGround = true;
				return;
			}
		}
	}

	//returns nearest mech in range or null if there are none
	Mech FindNearbyMech () {
        // detect nearby Mech GameObjects
		Collider2D[] nearbyMechs = Physics2D.OverlapCircleAll(transform.position,
		                                                      mechNearEnoughToUseDistance,
		                                                      mechOnlyMask);

        // making sure the detected nearby Mech GameObjects are tagged "Mech"
		nearbyMechs = nearbyMechs.Select( m => m ).Where( m => m.gameObject.CompareTag( "Mech" ) ).ToArray( );

		float nearestMechDist = 9000.0f;
		Collider2D nearestMechCollider = null;
		for (int i = 0; i < nearbyMechs.Length; i++) {
			float distToMech = Vector2.Distance(transform.position,
			                                    nearbyMechs[i].transform.position);
			if (distToMech < nearestMechDist) {
				nearestMechDist = distToMech;
				nearestMechCollider = nearbyMechs[i];
			}
		}
		if (nearestMechCollider) {
			Mech mScript = nearestMechCollider.GetComponent<Mech>();
			if (mScript) {
			    return mScript; //we found a mech!
			}
			else {
				Debug.Log("Mech script not found on nearest collider, check mechOnlyMask");
				return null;
			}
		}
		else return null; //no mech in range
	}

} // class
