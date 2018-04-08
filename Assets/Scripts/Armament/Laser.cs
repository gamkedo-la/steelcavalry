using UnityEngine;
using UnityEngine.Assertions;

public class Laser : MonoBehaviour, IWeapon
{
    [SerializeField] private GameObject gun;
	[SerializeField] private ParticleSystem muzzleFlesh;
	[SerializeField] private ParticleSystem impact;
	[SerializeField] private GameEventAudioEvent audioEvent;
	[SerializeField] private AudioEvents audioEventType = AudioEvents.LaserTyp1;
	[SerializeField] private Transform spawnPoint = null;
	[SerializeField] private WeaponParameters parameters = null;
	[SerializeField] private GameEventUI weaponSlotEvents;
	[SerializeField] private GameEventFloat didDamageEvent = null;
    [SerializeField] private Team fromTeam;
    [SerializeField] private WeaponType type = WeaponType.Turret;
	[SerializeField] private float maxLaserSize = 20f;
	[SerializeField] private float laserScaleCorrection = 1f;
	[SerializeField] private float laserMoveCorrection = 1f;
	[SerializeField] private float minAngle = -60f;
	[SerializeField] private float maxAngle = 60f;
	[SerializeField] private GameObject[] models;

    public Team FromTeam {
        get { return fromTeam; }
        set { fromTeam = value; }
    }

    public WeaponType Type
	{
		get { return type; }
	}

	private bool isActive = false;
	private bool isRight = false;
	private bool isPlayerDriver = false;
	private float xAngle;
	private GameObject beam;
	private bool shooting = false;

	private float realoadTimeLeft = 0;
	private float shootingTimeleft = 0;

	void Start( )
	{
		Assert.IsNotNull( muzzleFlesh );
		Assert.IsNotNull( impact );
		Assert.IsNotNull( audioEvent );
		Assert.IsNotNull( spawnPoint );
		Assert.IsNotNull( parameters );
		Assert.IsNotNull( parameters.Projectile );
		Assert.IsNotNull( didDamageEvent );
		Assert.IsNotNull( weaponSlotEvents );

		beam = Instantiate( parameters.Projectile, spawnPoint.position, Quaternion.identity, spawnPoint );
		beam.transform.localRotation = Quaternion.identity;

		shootingTimeleft = parameters.MagSize;
	}

	void Update( )
	{
		if ( !isActive )
			return;

		realoadTimeLeft -= Time.deltaTime;

		if ( isPlayerDriver && realoadTimeLeft >= 0 )
		{
			weaponSlotEvents.Raise( UIEvent.TurretOn, 1f - ( realoadTimeLeft / parameters.RealoadTime ) );
		}
	}

	void FixedUpdate( )
	{
		if(beam) {
			beam.gameObject.SetActive(false);
		} else {
			Debug.LogWarning("Beam not defined in Laser.cs file?");
		}

		if ( !isActive )
			return;

		LookAtCursor( );
	}

	public void IsPlayerDriving( bool playerDriver )
	{
		isPlayerDriver = playerDriver;

		var firstIcon = GameObject.Find( "Main UI/Icon Turret" ).GetComponent<AbilityIcon>( );
		firstIcon.SetIcon( "turretLaser" );

		weaponSlotEvents.Raise( UIEvent.TurretOn, shootingTimeleft / parameters.MagSize );
	}

	public void SwapModel( string mechName )
	{
		// Debug.Log( mechName );
		if (mechName == "Ostrich The Bot" )
		{
			if(models[0]) {
				models[0].SetActive(false);
			}
			if(models[1]) {
				models[1].SetActive(true);
			}
		}
	}

	public void Active( bool isActive )
	{
		this.isActive = isActive;

		if ( !isPlayerDriver )
			return;

		if ( isActive )
		{
			var firstIcon = GameObject.Find( "Main UI/Icon Turret" ).GetComponent<AbilityIcon>( );
			firstIcon.SetIcon( "turretLaser" );

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

	public void TryToFire( )
	{
		if ( spawnPoint == null )
			return;

		if ( realoadTimeLeft > 0 )
			return;

		ShootLaser( );

		shootingTimeleft -= Time.deltaTime;

		if ( isPlayerDriver )
		{
			weaponSlotEvents.Raise( UIEvent.TurretOn, shootingTimeleft / parameters.MagSize );
		}

		if ( shootingTimeleft <= 0 )
		{
			shootingTimeleft = parameters.MagSize;
			realoadTimeLeft = parameters.RealoadTime;
		}
	}

	private void LookAtCursor( )
	{
		if ( !( Utilities.GetMouseWorldPosition( Input.mousePosition ).x < gun.transform.position.x && !isRight ) &&
			 !( Utilities.GetMouseWorldPosition( Input.mousePosition ).x > gun.transform.position.x && isRight ) )
			return;

		Vector3 diff = Utilities.GetMouseWorldPosition( Input.mousePosition ) - gun.transform.position;
		diff.Normalize( );

		xAngle = -1 * Mathf.Atan2( diff.y, diff.x ) * Mathf.Rad2Deg;
		if ( isRight )
		{
			xAngle = Mathf.Clamp( xAngle, minAngle, maxAngle );
		}
		else
		{
			if ( xAngle > 0 )
			{
				xAngle = Mathf.Clamp( xAngle, 180f + minAngle, 180f );
			}
			else
			{
				xAngle = Mathf.Clamp( xAngle, -180f, -180f + maxAngle );
			}
		}

		float yAngle = 90f;

		gun.transform.rotation = Quaternion.Euler( xAngle, yAngle, 0f );
	}

	private void StopSound()
	{
		shooting = false;
	}

	private void ShootLaser()
	{
		if ( !shooting )
		{
			audioEvent.Raise( audioEventType, gun.transform.position );
			shooting = true;
			Invoke( "StopSound", 1f );
		}

		beam.gameObject.SetActive( true );
		if (!muzzleFlesh.isPlaying) muzzleFlesh.Play( );

		Vector2 laserDirection = spawnPoint.rotation * ( spawnPoint.right * -1 );
		float currentLaserSize = maxLaserSize;

		RaycastHit2D hit = Physics2D.Raycast( spawnPoint.position, laserDirection, maxLaserSize );

		if ( hit.collider != null )
		{
			currentLaserSize = Vector2.Distance( spawnPoint.position, hit.point );
			beam.transform.localScale = new Vector3( beam.transform.localScale.x, beam.transform.localScale.y, currentLaserSize * laserScaleCorrection );
			beam.transform.localPosition = new Vector3( beam.transform.localPosition.x, beam.transform.localPosition.y, currentLaserSize * laserMoveCorrection );

			impact.transform.position = hit.point;

			if ( !impact.isPlaying )
				impact.Play( );

            Player collidedPlayer = hit.collider.gameObject.GetComponent<Player>();
            Mech collidedMech = hit.collider.gameObject.GetComponent<Mech>();

            if (collidedPlayer != null || collidedMech != null) {
                collidedPlayer = collidedPlayer == null ? collidedMech.driver : collidedPlayer;
            }

            if (FromTeam != Team.Independant && collidedPlayer && collidedPlayer.team == FromTeam) {
                return;                
            }

            HP hp = hit.collider.GetComponent<HP>( );
			if ( hp )
			{
				didDamageEvent.Raise( 0.1f );
				hp.TakeDamage( parameters.GetDamage( ) * Time.deltaTime );
			}
			
			CanisterGrenade canisterGrenade = hit.collider.GetComponent<CanisterGrenade>();
			if (canisterGrenade) {
				canisterGrenade.DoDestruction(canisterGrenade.transform.position);
			}

			Mine mine = hit.collider.GetComponent<Mine>();
			if (mine) {
				mine.ExplodeAndDestroy();
			}
		}
		else
		{
			// Shooting in the air :(
			currentLaserSize = maxLaserSize;
			beam.transform.localScale = new Vector3( beam.transform.localScale.x, beam.transform.localScale.y, currentLaserSize * laserScaleCorrection );
			beam.transform.localPosition = new Vector3( beam.transform.localPosition.x, beam.transform.localPosition.y, currentLaserSize * laserMoveCorrection );
		}
	}
}
