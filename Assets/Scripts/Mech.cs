using UnityEngine;
using UnityEngine.Assertions;

public class Mech : MonoBehaviour
{
	[SerializeField] private GameObject[] bodyParts = null;
	[SerializeField] private MechUI ui = null;
	[SerializeField] private string mechName = "The Bot";
	public float mechSpeed = 2.0f;
 	public float jumpPower = 10.0f;
 	public float damageTaken = 0.0f;
 	public float maxDamage = 100.0f;

	private bool isOnGround;
	public bool inUse = false;

	public WeaponManager weaponManager;
	public PodLauncher pod;

	public GameObject driver; // either the player or an enemy ai player
	private PlayerMovement driverMovement;

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

    // Use this for initialization
    void Start () {

		Assert.IsNotNull( ui );
		ui.SetName( mechName );

		mechRB = GetComponent<Rigidbody2D>();
		//driver = GameObject.FindWithTag("Player"); // this may not be who is really driving
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
		driverMovement = driver.GetComponent<PlayerMovement>();

		inUse = true;
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
            /*driverMovement.OnFire += laser.HandleFire; //adds itself to the listeners of OnFire()
            driverMovement.OnAltFire += missiles.HandleFire;
            driverMovement.OnAltFire2 += canisters.HandleFire;
            laser.Active(true);
            missiles.Active(true);
            canisters.Active(true);*/
            driverMovement.OnFire += pod.HandleFire; //adds itself to the listeners of OnFire()
            pod.Active(true);
        }
    }

	public void wasExited() {
		inUse = false;
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
	public void Update () {

        // BRANCH controls for Regular/Golden Goose Mech
        if (!isGoldenGoose) {
            if (!inUse) return; //could be made into a function to do something else when idle
            if (!driverMovement) return;

            if (driverMovement.inputRight)
                transform.position += Vector3.right * Time.deltaTime * mechSpeed;

            if (driverMovement.inputLeft)
                transform.position += Vector3.left * Time.deltaTime * mechSpeed;

            if (driverMovement.inputUp && isOnGround)
            {
                mechRB.AddForce(Vector2.up * Input.GetAxisRaw("Vertical") * jumpPower);
                isOnGround = false;
            }
		}
        else {
            if (!inUse) return; //could be made into a function to do something else when idle
            if (!driverMovement) return;
            if (podLaunched) return;

            if (driverMovement.inputRight && isOnGround && goldenLedgeCheck.isGroundRight)
                transform.position += Vector3.right * Time.deltaTime * mechSpeed;
            if (driverMovement.inputLeft && isOnGround && goldenLedgeCheck.isGroundLeft)
                transform.position += Vector3.left * Time.deltaTime * mechSpeed;
            if (driverMovement.inputUp && isOnGround)
                rocketPivot.transform.Rotate(Vector3.forward * Time.deltaTime * gGRocketRotateSpeed);
            if (driverMovement.inputDown && isOnGround)
                rocketPivot.transform.Rotate(Vector3.back * Time.deltaTime * gGRocketRotateSpeed);
        }
	}

	void OnCollisionEnter2D(Collision2D bumpFacts) {
		for(int i = 0; i < bumpFacts.contacts.Length; i++) {
			if(bumpFacts.contacts[i].normal.y >= 0.9f) {
				isOnGround = true;
				return;
			}
		}
	}

	public void TakeDamage(float damageAmount)
	{
		if (!inUse) damageAmount *= 1.25f;

		damageTaken += damageAmount;
		damageTaken = damageTaken > maxDamage ? maxDamage : damageTaken;

		ui.SetHP( 1f - damageTaken / maxDamage );

		if(damageTaken >= maxDamage)
		{
			MakeDestructionEffect( );
			destroying = true;
			Destroy( ui.gameObject );
			Destroy(gameObject);
		}
	}

	private void MakeDestructionEffect()
	{
		if ( bodyParts == null || bodyParts.Length == 0 || destroying ) return;

		foreach ( var part in bodyParts )
		{
			part.GetComponent<CircleCollider2D>( ).enabled = true;
			part.AddComponent<Rigidbody2D>( );
			part.transform.SetParent( null );
		}
	}
}
