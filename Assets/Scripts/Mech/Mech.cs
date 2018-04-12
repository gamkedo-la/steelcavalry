using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(WeaponManager), typeof(HP), typeof(SlopeWalker))]
[RequireComponent(typeof(MechAnimator))]
public class Mech : MonoBehaviour
{
    [SerializeField] private GameEventAudioEvent audioEvent;
    [Header("Mech Body")]    
    private MechBodyParts mechBodyParts;
    public GameObject bodyPartToVanish = null;//added to destroy winged swpan mech body as it is too large to roll after destroyed
    public Transform mechModel;
    [HideInInspector]
    public Rigidbody2D mechRigidbody;
    private SlopeWalker slopeWalker;
    [SerializeField] private GameObject explosion = null;
    [SerializeField] private MechUI ui = null;
    [SerializeField] private bool canExplodeIn3D = true;
    [SerializeField] private bool isAffectedByGravity = true;

    [Header("Mech Specs")]
    [SerializeField]
    private string mechName = "The Bot";
    [SerializeField] private float expForceMin = 300f;
    [SerializeField] private float expForceMax = 500f;
    [SerializeField] private float thrusterFuelMax = 100f;
    [SerializeField] private float thrusterFuelRegen = 20f;
    [SerializeField] private float thrusterCost = 20f;
    [SerializeField] private float thrusterPower = 20f;
    [SerializeField] private float firstThrustCost = 0.25f;
    [SerializeField] private float drag = 0f;
    public float mechMoveSpeed = 2.0f;
    public float mechRotateSpeed = 5.0f;
    public float jumpPower = 10.0f;
    public float takingOffTime = 2.4f;
    public float crushDamage = 100f;

    [Header("Mech State")]
    public bool inUse = false;
    public float damageTaken = 0.0f;
    public float maxDamage = 100.0f;
    public bool canBeStolen = true;
    public bool isOnGround;
    public float currentTimeToTakeOff = 0;
    public bool canRotateOnSlopes = true;
    public bool isFlying = false;
    private bool canFly = true;
    public bool isFacingRight;
    private float thrusterFuelCurrent = 100f;
    private HP hp = null;
    private bool isBeingDestroyed = false;

    [Header("Mech Driver")]
    public PodLauncher pod;
    [HideInInspector] public Player driver;
    public float minimumSecondsBetweenSteals = 2.0f;
    public float lastStolenAt = 0.0f;

    private WeaponManager weaponManager;

    [Header("Golden Goose Mech")]
    // limit the mech to a platform
    public GoldenLedgeCheck goldenLedgeCheck;
    public bool isGoldenGoose;
    // Rocket Rotation
    public GameObject rocketPivot;
    public float gGRocketRotateSpeed = 1;
    // Rocket Pod Launching
    public bool podLaunched = false;

    [Header("Mech Shield")]
    public bool mechCanShield;
    public float shieldWait, shieldDuration, shieldCost;
    private bool canShield = true;
    private bool shieldActive = false;
    public GameObject shieldGO;
    private GameObject shieldInstance;

    private bool thrustersOn = false;

    private float explosionDamageToPlayer = 1000;

    public UnityEvent ThrustersOn;
    public UnityEvent ThrustersOff;

	private Animator anim;

    void Awake () {
        slopeWalker = GetComponent<SlopeWalker>();
        slopeWalker.enabled = false;

		anim = GetComponentInChildren<Animator>();

        Assert.IsNotNull(ui);
        Assert.IsNotNull(explosion);
        Assert.IsNotNull(audioEvent);
        ui.SetName(mechName);

        hp = GetComponent<HP>();
        weaponManager = GetComponent<WeaponManager>();
        mechRigidbody = GetComponent<Rigidbody2D>();

        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName.Contains("Space")) {
            isAffectedByGravity = false;
        }

        if (sceneName.Contains("Base")) {
            canExplodeIn3D = false;
        }

        mechBodyParts = GetComponentInChildren<MechBodyParts>();
        mechBodyParts.canExplodeIn3D = canExplodeIn3D;
        mechBodyParts.isAffectedByGravity = isAffectedByGravity;

        thrusterFuelCurrent = thrusterFuelMax;

		anim.enabled = false;
    }

    void Start() {
        slopeWalker.enabled = true;    
    }

    public void Side(bool isRight) {
        isFacingRight = isRight;
        slopeWalker.isFacingRight = isFacingRight;

        if (weaponManager != null) {
            weaponManager.SetDir(isRight);
        }

        if (pod != null) {
            pod.SetDir(isRight);
        }
    }

    public void wasEntered(GameObject newDriver) {
        driver = newDriver.GetComponent<Player>();
        inUse = true;
		anim.enabled = inUse;
        //Debug.Log("in Use " + inUse);
        hp.UseMultiplier(!inUse);
        if (weaponManager != null) {
            weaponManager.IsPlayerDriving(newDriver.CompareTag("Player"));
            ui.IsPlayerDriving(newDriver.CompareTag("Player"));
            weaponManager.IsActive(true);
            driver.OnFire += weaponManager.FirePrimary;
            driver.OnAltFire += weaponManager.FireSecondary;
            driver.OnAltFire2 += weaponManager.FireTertiary;
        }

        if (pod != null && pod.enabled) {
            driver.OnFire += pod.HandleFire; //adds itself to the listeners of OnFire()
            pod.Active(true);
        }
    }

    public void wasExited() {
        inUse = false;
		anim.enabled = inUse;
        hp.UseMultiplier(!inUse);
        if (weaponManager != null) {
            driver.OnFire -= weaponManager.FirePrimary;
            driver.OnAltFire -= weaponManager.FireSecondary;
            driver.OnAltFire2 -= weaponManager.FireTertiary;
            weaponManager.IsActive(false);
            weaponManager.IsPlayerDriving(false);
            ui.IsPlayerDriving(false);
        }
    }

    public void Update() {
        if (slopeWalker.isOnSlope && canRotateOnSlopes) {
            Quaternion slopeQuaternion = Quaternion.Euler(Mathf.RoundToInt(-slopeWalker.slopeAngle), mechModel.transform.eulerAngles.y, mechModel.transform.eulerAngles.x);
            mechModel.transform.rotation = Quaternion.Slerp(mechModel.transform.rotation, slopeQuaternion, 10f * Time.deltaTime);
        }
    }

    // Update is called once per frame
    public void FixedUpdate() {
        // BRANCH controls for Regular/Golden Goose Mech
        if (!isGoldenGoose) {
            if (!inUse) return; //could be made into a function to do something else when idle
            if (!driver) return;

            if (driver.inputRight) {
                mechRigidbody.velocity = new Vector2(Time.deltaTime * mechMoveSpeed, mechRigidbody.velocity.y);
            }

            if (driver.inputLeft) {
                mechRigidbody.velocity = new Vector2(Time.deltaTime * -mechMoveSpeed, mechRigidbody.velocity.y);
            }

            if (driver.inputUp && !isFlying /*isOnGround*/ && thrusterFuelCurrent >= thrusterFuelMax * firstThrustCost) {
                currentTimeToTakeOff += Time.deltaTime;

                if (currentTimeToTakeOff >= takingOffTime) {
                    mechRigidbody.AddForce(Vector2.up * jumpPower);
                    thrusterFuelCurrent -= thrusterFuelMax * firstThrustCost;
                    isOnGround = false;
                    isFlying = true;
                    canFly = true;                    
                }

                ThrustersOn.Invoke();
            }

            if (driver.inputUp && isFlying && canFly && thrusterFuelCurrent > thrusterCost * Time.deltaTime) {
                mechRigidbody.AddForce(Vector2.up * thrusterPower * Time.deltaTime);
                thrusterFuelCurrent -= thrusterCost * Time.deltaTime;
                ui.SetFuel(thrusterFuelCurrent / thrusterFuelMax);                
            }
            else {
                thrusterFuelCurrent += Time.deltaTime * thrusterFuelRegen;
                thrusterFuelCurrent = thrusterFuelCurrent > thrusterFuelMax ? thrusterFuelMax : thrusterFuelCurrent;
                ui.SetFuel(thrusterFuelCurrent / thrusterFuelMax);

                if (canFly)
                    ThrustersOff.Invoke();

                canFly = false;
            }

            if (!driver.inputUp) {
                isFlying = false;
            }

			if (isFlying && canFly && !thrustersOn )
			{
				thrustersOn = true;
				audioEvent.Raise( AudioEvents.MechThrustter, transform.position );
				Invoke( "ThrustersOffSound", 0.5f );
			}

        }
        else {
            if (!inUse) return; //could be made into a function to do something else when idle
            if (!driver) return;
            if (podLaunched) return;

            if (driver.inputRight && isOnGround && goldenLedgeCheck.isGroundRight)
                transform.position += Vector3.right * Time.deltaTime * mechMoveSpeed;
            if (driver.inputLeft && isOnGround && goldenLedgeCheck.isGroundLeft)
                transform.position += Vector3.left * Time.deltaTime * mechMoveSpeed;
            if (driver.inputUp && isOnGround)
                rocketPivot.transform.Rotate(Vector3.forward * Time.deltaTime * gGRocketRotateSpeed);
            if (driver.inputDown && isOnGround)
                rocketPivot.transform.Rotate(Vector3.back * Time.deltaTime * gGRocketRotateSpeed);
        }

        HandleAbilities();

        mechRigidbody.drag = drag * Mathf.Pow(mechRigidbody.velocity.magnitude, 2);        

        if (!canBeStolen) {
            AttemptToToggleCanBeStolen();
        }

        slopeWalker.isMovingUp = driver.inputUp;
    }

    private void ThrustersOffSound() {
        thrustersOn = false;
    }

    void AttemptToToggleCanBeStolen() {
        float differenceInTime = Time.time - lastStolenAt;
        if (differenceInTime >= minimumSecondsBetweenSteals) {
            ToggleCanBeStolen();
        }
    }

    public void ToggleCanBeStolen() {
        canBeStolen = !!canBeStolen;
        if (canBeStolen) {
            lastStolenAt = Time.time;
        }
    }

    void OnCollisionEnter2D(Collision2D col) {
        Player player = CheckForCollisionWithPlayer(col);

        for (int i = 0; i < col.contacts.Length; i++) {
            if (col.contacts[i].normal.y >= 0.9f) {

                // crush the human beneath the weight of a mech
                if (player != null && player.isOnGround) {
                    HP hp = col.collider.GetComponent<HP>();
                    if (hp) {
                        hp.TakeDamage(crushDamage);
                    }
                    else {
                        Destroy(col.gameObject);
                    }
                }

                isOnGround = true;
                currentTimeToTakeOff = 0;
                return; // beware, this exits the whole method!
            }
        }
    }

    private Player CheckForCollisionWithPlayer(Collision2D other) {
        string playerTag = "Player";
        bool collidedWithPlayer = other.gameObject.tag == playerTag;
        if (collidedWithPlayer) {
            return other.collider.GetComponent<Player>();
        }
        return null;
    }

    public string GetName() {
        return mechName;
    }

    public void DestroyMech() {
        audioEvent.Raise(AudioEvents.MechExplosion, transform.position);
        var exp = Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(exp, 3f);

        const float delay = 0.1f;
        Invoke("MakeDestructionEffect", delay);
    }

    private void MakeDestructionEffect() {
        if (isBeingDestroyed) return;

        isBeingDestroyed = true;

        mechBodyParts.MakeBodyParts(expForceMin, expForceMax);

        KillPlayerInMech();

        Destroy(bodyPartToVanish);//added to destroy winged spawn mech body as it is too large to roll after destroyed
        Destroy(ui.gameObject);
        Destroy(gameObject);
    }

    private void KillPlayerInMech() {
        if (driver && driver.mechImIn && driver.mechImIn.GetName() == mechName) {
            driver.ExitMech();
            HP hp = driver.GetComponent<HP>();
            hp.TakeDamage(explosionDamageToPlayer);
        }
    }

    private void HandleAbilities() {
        HandleShield();
    }

    private void HandleShield() {
        if (Input.GetKeyDown(KeyCode.LeftShift) && canShield && mechCanShield) //Temporal test imput
        {
            shieldInstance = Instantiate(shieldGO, transform.position, Quaternion.identity);
            shieldInstance.transform.parent = transform;
            canShield = false;
            shieldActive = true;
            //Invoke("EnableShield", shieldWait);
            //Invoke("DisableShieldObject", shieldDuration);
        }
        else if (Input.GetKey(KeyCode.LeftShift) && !canShield) {
            thrusterFuelCurrent -= shieldCost * Time.deltaTime;
            thrusterFuelCurrent = Mathf.Clamp(thrusterFuelCurrent, 0f, thrusterFuelMax);
            ui.SetFuel(thrusterFuelCurrent / thrusterFuelMax);
            if (thrusterFuelCurrent <= 0f && shieldActive) {
                DisableShieldObject();
            }
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift) && shieldActive) {
            DisableShieldObject();
        }
    }

    private void EnableShield() {
        canShield = true;
    }

    private void DisableShieldObject() {
        shieldActive = false;
        Invoke("EnableShield", shieldWait);
        shieldInstance.GetComponentInChildren<Animator>().SetTrigger("CloseShield");
    }
}
