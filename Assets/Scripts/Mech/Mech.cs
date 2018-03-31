using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

[RequireComponent(typeof(WeaponManager), typeof(HP), typeof(SlopeWalker))]
[RequireComponent(typeof(MechAnimator))]
public class Mech : MonoBehaviour
{
    [SerializeField] private GameEventAudioEvent audioEvent;
    [Header("Mech Body")]
    [SerializeField]
    private GameObject[] bodyParts = null;
    public GameObject bodyPartToVanish = null;//added to destroy winged swpan mech body as it is too large to roll after destroyed
    public Transform mechModel;
    [HideInInspector]
    public Rigidbody2D mechRigidbody;
    private SlopeWalker slopeWalker;
    [SerializeField] private GameObject explosion = null;
    [SerializeField] private MechUI ui = null;

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
    public float crushDamage = 100f;

    [Header("Mech State")]
    public bool inUse = false;
    public float damageTaken = 0.0f;
    public float maxDamage = 100.0f;
    public bool canBeStolen = true;
    public bool isOnGround;
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

    public UnityEvent ThrustersOn;
    public UnityEvent ThrustersOff;

    void Start() {

        Assert.IsNotNull(ui);
        Assert.IsNotNull(explosion);
        Assert.IsNotNull(audioEvent);
        ui.SetName(mechName);

        hp = GetComponent<HP>();
        weaponManager = GetComponent<WeaponManager>();
        mechRigidbody = GetComponent<Rigidbody2D>();

        slopeWalker = GetComponent<SlopeWalker>();

        thrusterFuelCurrent = thrusterFuelMax;
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

    // Update is called once per frame
    public void LateUpdate() {
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
                mechRigidbody.AddForce(Vector2.up * jumpPower);
                thrusterFuelCurrent -= thrusterFuelMax * firstThrustCost;

                isOnGround = false;
                isFlying = true;
                canFly = true;

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

        foreach (var part in bodyParts) {
            Debug.Log("body part " + part + "as part of bodyParts " + bodyParts);
            part.GetComponent<CircleCollider2D>().enabled = true;
            part.AddComponent<Rigidbody2D>();
            part.GetComponent<Rigidbody2D>().AddForce(Quaternion.Euler(0, 0, Random.Range(0, 360)) * Vector2.left * Random.Range(expForceMin, expForceMax));
            part.transform.SetParent(null);
        }
        Destroy(bodyPartToVanish);//added to destroy winged spawn mech body as it is too large to roll after destroyed
        Destroy(ui.gameObject);
        Destroy(gameObject);
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
