using UnityEngine;
using UnityEngine.Assertions;

public class Mech : MonoBehaviour
{
	[SerializeField] private GameObject[] bodyParts = null;
	public float mechSpeed = 2.0f;
 	public float jumpPower = 10.0f;
 	public float damageTaken = 0.0f;
 	public float maxDamage = 100.0f;

	private bool isOnGround;
	private bool inUse = false;

	public Gun gun;
	public MissileLauncher missiles;
	public CanisterLauncher canisters;

	private GameObject driver; // either the player or an enemy ai player
	private PlayerMovement driverMovement;

	private Rigidbody2D mechRB;
	public Transform model;

	// Use this for initialization
	void Start () {

		//Assert.IsNotNull( gun );

		mechRB = GetComponent<Rigidbody2D>();
		//driver = GameObject.FindWithTag("Player"); // this may not be who is really driving
	}

	public void Side (bool isRight)
	{
		if ( gun != null )
		{
			gun.SetDir( isRight );
			canisters.SetDir( isRight );
		}
	}

	public void wasEntered(GameObject newDriver) {

		driver = newDriver;
		driverMovement = driver.GetComponent<PlayerMovement>();

		inUse = true;
		if ( gun != null )
		{
			driverMovement.OnFire += gun.HandleFire; //adds itself to the listeners of OnFire()
			driverMovement.OnAltFire += missiles.HandleFire;
			driverMovement.OnAltFire2 += canisters.HandleFire;
			gun.Active( true );
			missiles.Active( true );
			canisters.Active( true );
		}
	}

	public void wasExited() {
		inUse = false;
		if ( gun != null )
		{
			driverMovement.OnFire -= gun.HandleFire;
			driverMovement.OnAltFire -= missiles.HandleFire;
			driverMovement.OnAltFire2 -= canisters.HandleFire;
			gun.Active( false );
			missiles.Active( false );
			canisters.Active( false );
		}
	}

	// Update is called once per frame
	public void Update () {

		if (!inUse) return; //could be made into a function to do something else when idle
		if (!driverMovement) return;

		if (driverMovement.inputRight)
			transform.position += Vector3.right * Time.deltaTime * mechSpeed;

		if (driverMovement.inputLeft)
			transform.position += Vector3.left * Time.deltaTime * mechSpeed;

		if (driverMovement.inputUp && isOnGround) {
			mechRB.AddForce(Vector2.up * Input.GetAxisRaw("Vertical") * jumpPower);
			isOnGround = false;
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
		if(damageTaken >= maxDamage)
		{
			MakeDestructionEffect( );
			Destroy(gameObject);
		}
	}

	private void MakeDestructionEffect()
	{
		if ( bodyParts == null || bodyParts.Length == 0 ) return;

		foreach ( var part in bodyParts )
		{
			part.GetComponent<CircleCollider2D>( ).enabled = true;
			part.AddComponent<Rigidbody2D>( );
			part.transform.SetParent( null );
		}
	}
}
