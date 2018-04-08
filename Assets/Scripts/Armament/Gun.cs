using UnityEngine;
using UnityEngine.Assertions;

public class Gun : MonoBehaviour, IWeapon
{
    [SerializeField] private GameObject shell;
    [SerializeField] private Transform shellSpawnPoint;
    [SerializeField] private ParticleSystem muzzleFlesh;
    [SerializeField] private GameEventAudioEvent audioEvent;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameEventUI weaponSlotEvents;
    [SerializeField] private WeaponParameters parameters = null;
    [SerializeField] private Team fromTeam;
    [SerializeField] private WeaponType type = WeaponType.Turret;
    [SerializeField] private float minAngle = -60f;
    [SerializeField] private float maxAngle = 60f;
    [SerializeField] private GameObject model;

    public Team FromTeam {
        get { return fromTeam; }
        set { fromTeam = value; }
    }

    public WeaponType Type {
        get { return type; }
    }

    private bool isActive = false;
    private bool isRight = false;
    private bool isPlayerDriver = false;
    private float xAngle;

    private float timeToNextShot = 0;
    private float realoadTimeLeft = 0;
    private int currentMagSize = 0;
    private int magSize = 15;

    void Start() {
        Assert.IsNotNull(shell);
        Assert.IsNotNull(shellSpawnPoint);
        Assert.IsNotNull(muzzleFlesh);
        Assert.IsNotNull(parameters);
        Assert.IsNotNull(parameters.Projectile);
        Assert.IsNotNull(spawnPoint);
        Assert.IsNotNull(weaponSlotEvents);
        Assert.IsNotNull(audioEvent);

        // currentMagSize = (int)parameters.MagSize;
        currentMagSize = magSize;
    }

    void Update() {
        if (!isActive) return;

        realoadTimeLeft -= Time.deltaTime;
        timeToNextShot -= Time.deltaTime;

        if (isPlayerDriver && realoadTimeLeft >= 0) {
            weaponSlotEvents.Raise(UIEvent.TurretOn, 1f - (realoadTimeLeft / parameters.RealoadTime));
        }
    }

    void FixedUpdate() {
        if (!isActive) return;

        LookAtCursor();
    }

    public void IsPlayerDriving(bool playerDriver) {
        isPlayerDriver = playerDriver;

		var firstIcon = GameObject.Find( "Main UI/Icon Turret" ).GetComponent<AbilityIcon>( );
		firstIcon.SetIcon( "turretGatling" );

		weaponSlotEvents.Raise( UIEvent.TurretOn, currentMagSize / (float)magSize );
	}

    public void SwapModel(string mechName) { }

	public void Active(bool isActive)
	{
		this.isActive = isActive;

		if ( !isPlayerDriver )
			return;

		if (isActive)
		{
			var firstIcon = GameObject.Find( "Main UI/Icon Turret" ).GetComponent<AbilityIcon>( );
			firstIcon.SetIcon( "turretGatling" );

			weaponSlotEvents.Raise( UIEvent.TurretOn );
		}
		else
		{
			weaponSlotEvents.Raise( UIEvent.TurretOff );
		}
	}

	public GameObject GetGameObject( )
	{
		return gameObject;
	}

	public void SetDir( bool isRight )
	{
		this.isRight = isRight;
	}

	public void TryToFire()
	{
		if ( spawnPoint == null )
			return;

		if ( realoadTimeLeft > 0 || timeToNextShot > 0 )
			return;

		audioEvent.Raise( AudioEvents.Shot, transform.position );
		muzzleFlesh.Play( );
		GameObject shotGO = Instantiate( parameters.Projectile, spawnPoint.position, Quaternion.Euler(0, 0, -xAngle + Random.Range( -5f, 5f ) ) );

		// TODO: suggest that we compare TAGS, not which gamepad this player is controlled by
		shotGO.GetComponent<ShotBreaksIntoParticle>().fromTeam = FromTeam;

		Rigidbody2D shotRB = shotGO.GetComponent<Rigidbody2D>( );
		shotRB.velocity = shotGO.transform.rotation * Vector2.right * parameters.Force;
		shotGO.GetComponent<ShotBreaksIntoParticle>( ).SetDamage( parameters.GetDamage( ) );

		shotGO.transform.SetParent( LitterContainer.instanceTransform );

		GameObject s = Instantiate( shell, shellSpawnPoint.position, Quaternion.identity );
		s.GetComponent<Rigidbody2D>( ).AddForce( Quaternion.Euler( 0, 0, Random.Range( -30, 30 ) ) * spawnPoint.up * 15 );

		timeToNextShot = parameters.DelayBetweenShots;

        currentMagSize--;

        model.transform.rotation = Quaternion.Euler(model.transform.eulerAngles.x, model.transform.eulerAngles.y, model.transform.eulerAngles.z + 10f);

        if ( isPlayerDriver )
		{
			weaponSlotEvents.Raise( UIEvent.TurretOn, currentMagSize / (float)magSize );
		}

		if ( currentMagSize <= 0 )
		{
			currentMagSize = magSize;
			realoadTimeLeft = parameters.RealoadTime;
		}
	}

	private void LookAtCursor( )
	{
		if ( !( Utilities.GetMouseWorldPosition( Input.mousePosition ).x < transform.position.x && !isRight ) &&
			 !( Utilities.GetMouseWorldPosition( Input.mousePosition ).x > transform.position.x && isRight ) )
			return;

		Vector3 diff = Utilities.GetMouseWorldPosition( Input.mousePosition ) - transform.position;
		diff.Normalize( );

		xAngle = -1 * Mathf.Atan2( diff.y, diff.x ) * Mathf.Rad2Deg;
		if(isRight)
		{
			xAngle = Mathf.Clamp( xAngle, minAngle, maxAngle );
		}
		else
		{
			if (xAngle > 0)
			{
				xAngle = Mathf.Clamp( xAngle, 180f + minAngle, 180f );
			}
			else
			{
				xAngle = Mathf.Clamp( xAngle, -180f, -180f + maxAngle );
			}
		}

		float yAngle = 90f;

		transform.rotation = Quaternion.Euler( xAngle, yAngle, 0f );
	}
}
