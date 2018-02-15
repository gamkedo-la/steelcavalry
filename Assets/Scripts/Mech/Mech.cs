using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

[RequireComponent(typeof(WeaponManager), typeof(HP))]
public class Mech : MonoBehaviour
{
	[SerializeField] private GameObject[] bodyParts = null;
	[SerializeField] private GameObject explosion = null;
	[SerializeField] private MechUI ui = null;
	[SerializeField] private string mechName = "The Bot";
	[SerializeField] private float expForceMin = 300f;
	[SerializeField] private float expForceMax = 500f;
	[SerializeField] private float thrusterFuelMax = 100f;
	[SerializeField] private float thrusterFuelRegen = 20f;
	[SerializeField] private float thrusterCost = 20f;
	[SerializeField] private float thrusterPower = 20f;
	[SerializeField] private float firstThrustCost = 0.25f;

	private HP hp = null;

	public float mechMoveSpeed = 2.0f;
    public float mechRotateSpeed = 5.0f;
 	public float jumpPower = 10.0f;
 	public float damageTaken = 0.0f;
 	public float maxDamage = 100.0f;

	private float thrusterFuelCurrent = 100f;
	private bool isOnGround;
	private bool isFlying = false;
	private bool canFly = true;
	public bool inUse = false;

	private WeaponManager weaponManager;
	public PodLauncher pod;

	public GameObject driver; // either the player or an enemy ai player
	private Player driverMovement;

	private Rigidbody2D mechRB;
	public Transform model;

	private bool destroying = false;
	public bool isFacingRight;

    [Header("Golden Goose Mech")]
    // limit the mech to a platform
    public GoldenLedgeCheck goldenLedgeCheck;
    public bool isGoldenGoose;
    // Rocket Rotation
    public GameObject rocketPivot;
    public float gGRocketRotateSpeed = 1;
    // Rocket Pod Launching
    public bool podLaunched = false;

    [Header("Mech Abilities")]
    public bool mechCanShield;
    public float shieldCooldown, shieldDuration;
    private bool canShield = true;
    public GameObject shieldGO;
    private GameObject shieldInstance;

	public UnityEvent ThrustersOn;
	public UnityEvent ThrustersOff;

	void Start () {

		Assert.IsNotNull( ui );
		Assert.IsNotNull( explosion );
		ui.SetName( mechName );

        hp = GetComponent<HP>();
        weaponManager = GetComponent<WeaponManager>();
		mechRB = GetComponent<Rigidbody2D>();

		thrusterFuelCurrent = thrusterFuelMax;
	}

	public void Side (bool isRight)
	{
		isFacingRight = isRight;
		if ( weaponManager != null )
		{
			weaponManager.SetDir( isRight );
		}

        if (pod != null) {
            pod.SetDir(isRight);
        }
    }

	public void wasEntered(GameObject newDriver)
	{
		driver = newDriver;
		driverMovement = driver.GetComponent<Player>();

		inUse = true;
		hp.UseMultiplier( !inUse );
		if ( weaponManager != null )
		{
			weaponManager.IsPlayerDriving( newDriver.CompareTag("Player") );
			ui.IsPlayerDriving( newDriver.CompareTag( "Player" ) );
			weaponManager.IsActive( true );
			driverMovement.OnFire += weaponManager.FirePrimary;
			driverMovement.OnAltFire += weaponManager.FireSecondary;
			driverMovement.OnAltFire2 += weaponManager.FireTertiary;
		}

        if (pod != null && pod.enabled)
        {
            driverMovement.OnFire += pod.HandleFire; //adds itself to the listeners of OnFire()
            pod.Active(true);
        }
    }

	public void wasExited() {
		inUse = false;
		hp.UseMultiplier( !inUse );
		if ( weaponManager != null )
		{
			driverMovement.OnFire -= weaponManager.FirePrimary;
			driverMovement.OnAltFire -= weaponManager.FireSecondary;
			driverMovement.OnAltFire2 -= weaponManager.FireTertiary;
			weaponManager.IsActive( false );
			weaponManager.IsPlayerDriving( false );
			ui.IsPlayerDriving( false );
		}
	}

	// Update is called once per frame
	public void LateUpdate ()
    {

        // BRANCH controls for Regular/Golden Goose Mech
        if (!isGoldenGoose)
        {
            if (!inUse) return; //could be made into a function to do something else when idle
            if (!driverMovement) return;

            if (driverMovement.inputRight)
                mechRB.velocity = new Vector2(Time.deltaTime * mechMoveSpeed, mechRB.velocity.y);

            if (driverMovement.inputLeft)
                mechRB.velocity = new Vector2(Time.deltaTime * -mechMoveSpeed, mechRB.velocity.y);

            if (driverMovement.inputUp && !isFlying /*isOnGround*/ && thrusterFuelCurrent >= thrusterFuelMax * firstThrustCost )
            {
                mechRB.AddForce(Vector2.up * jumpPower);
				thrusterFuelCurrent -= thrusterFuelMax * firstThrustCost;

				isOnGround = false;
				isFlying = true;
				canFly = true;

				ThrustersOn.Invoke( );
            }

			if ( driverMovement.inputUp && isFlying && canFly && thrusterFuelCurrent > thrusterCost * Time.deltaTime )
			{
				mechRB.AddForce( Vector2.up * thrusterPower * Time.deltaTime );
				thrusterFuelCurrent -= thrusterCost * Time.deltaTime;
				ui.SetFuel( thrusterFuelCurrent / thrusterFuelMax );
			}
			else
			{
				thrusterFuelCurrent += Time.deltaTime * thrusterFuelRegen;
				thrusterFuelCurrent = thrusterFuelCurrent > thrusterFuelMax ? thrusterFuelMax : thrusterFuelCurrent;
				ui.SetFuel( thrusterFuelCurrent / thrusterFuelMax );

				if (canFly)
					ThrustersOff.Invoke( );

				canFly = false;
			}

			if ( !driverMovement.inputUp )
			{
				isFlying = false;
			}

		}
        else
        {
            if (!inUse) return; //could be made into a function to do something else when idle
            if (!driverMovement) return;
            if (podLaunched) return;

            if (driverMovement.inputRight && isOnGround && goldenLedgeCheck.isGroundRight)
                transform.position += Vector3.right * Time.deltaTime * mechMoveSpeed;
            if (driverMovement.inputLeft && isOnGround && goldenLedgeCheck.isGroundLeft)
                transform.position += Vector3.left * Time.deltaTime * mechMoveSpeed;
            if (driverMovement.inputUp && isOnGround)
                rocketPivot.transform.Rotate(Vector3.forward * Time.deltaTime * gGRocketRotateSpeed);
            if (driverMovement.inputDown && isOnGround)
                rocketPivot.transform.Rotate(Vector3.back * Time.deltaTime * gGRocketRotateSpeed);
        }

        HandleAbilities();
    }

    void OnCollisionEnter2D(Collision2D bumpFacts) {
		for(int i = 0; i < bumpFacts.contacts.Length; i++) {
			if(bumpFacts.contacts[i].normal.y >= 0.9f) {
				isOnGround = true;
				return;
			}
		}
	}

	public void DestroyMech()
	{
		var exp = Instantiate( explosion, transform.position, Quaternion.identity );
		Destroy( exp, 3f );

		const float delay = 0.1f;
		Invoke( "MakeDestructionEffect", delay);
	}

	private void MakeDestructionEffect()
	{
		if ( bodyParts == null || bodyParts.Length == 0 || destroying ) return;

		destroying = true;

		foreach ( var part in bodyParts )
		{
			part.GetComponent<CircleCollider2D>( ).enabled = true;
			part.AddComponent<Rigidbody2D>( );
			part.GetComponent<Rigidbody2D>( ).AddForce( Quaternion.Euler( 0, 0, Random.Range( 0, 360 ) ) * Vector2.left * Random.Range( expForceMin, expForceMax ) );
			part.transform.SetParent( null );
		}

		Destroy( ui.gameObject );
		Destroy( gameObject );
	}


    private void HandleAbilities()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && canShield && mechCanShield) //Temporal test imput
        {
            shieldInstance = Instantiate(shieldGO, transform.position, Quaternion.identity);
            shieldInstance.transform.parent = transform;
            canShield = false;
            Invoke("EnableShield", shieldCooldown);
            Invoke("DisableShieldObject", shieldDuration);
        }
    }

    private void EnableShield()
    {
        canShield = true;
    }

    private void DisableShieldObject()
    {
        shieldInstance.GetComponentInChildren<Animator>().SetTrigger("CloseShield");
    }
}
