using UnityEngine;
using UnityEngine.Assertions;

public class CanisterLauncher : MonoBehaviour, IWeapon
{
	[SerializeField] private Transform spawnPoint;
	[SerializeField] private GameEventUI weaponSlotEvents;
	[SerializeField] private WeaponParameters parameters = null;
    [SerializeField] private Team fromTeam;
	[SerializeField] private WeaponType type = WeaponType.Thrower;

    public Team FromTeam {
        get { return fromTeam; }
        set { fromTeam = value; }
    }

    public WeaponType Type
	{
		get { return type; }
	}

	private bool isActive = false;
	private bool isPlayerDriver = false;
	private float timeToNextShot = 0;
	private float realoadTimeLeft = 0;
	private int currentMagSize = 0;

	void Start( )
	{
		Assert.IsNotNull( parameters );
		Assert.IsNotNull( parameters.Projectile );
		Assert.IsNotNull( spawnPoint );
		Assert.IsNotNull( weaponSlotEvents );

		currentMagSize = (int)parameters.MagSize;
	}

	void Update( )
	{
		if ( !isActive )
			return;

		realoadTimeLeft -= Time.deltaTime;
		timeToNextShot -= Time.deltaTime;

		if ( isPlayerDriver && realoadTimeLeft >= 0 )
		{
			weaponSlotEvents.Raise( UIEvent.ThrowerOn, 1f - ( realoadTimeLeft / parameters.RealoadTime ) );
		}
	}

	public void IsPlayerDriving( bool playerDriver )
	{
		isPlayerDriver = playerDriver;
	}

	public void SwapModel( string mechName ) { }

	public void SetDir( bool isRight ) { }

	public void Active( bool isActive )
	{
		this.isActive = isActive;

		if ( !isPlayerDriver )
			return;

		if ( isActive )
		{
			weaponSlotEvents.Raise( UIEvent.ThrowerOn );
		}
		else
		{
			weaponSlotEvents.Raise( UIEvent.ThrowerOff );
		}
	}

	public GameObject GetGameObject( )
	{
		return gameObject;
	}

	public void TryToFire( )
	{
		if ( spawnPoint == null ) return;

		if ( realoadTimeLeft > 0 || timeToNextShot > 0 )
			return;

		GameObject shotGO = Instantiate
		(
			parameters.Projectile,
			spawnPoint.position,
			Quaternion.Euler
			(
				spawnPoint.rotation.eulerAngles.x,
				spawnPoint.rotation.eulerAngles.y,
				spawnPoint.rotation.eulerAngles.z + Random.Range( 0f, 15f )
			)
		);

		Rigidbody2D shotRB = shotGO.GetComponent<Rigidbody2D>( );
		shotRB.velocity = shotGO.transform.rotation * Vector2.left * parameters.Force;

        CanisterGrenade grenade = shotGO.GetComponent<CanisterGrenade>();
        grenade.fromTeam = FromTeam;
		grenade.SetDamage( parameters.GetDamage( ) );

		shotGO.transform.SetParent( LitterContainer.instanceTransform );

		timeToNextShot = parameters.DelayBetweenShots;
		currentMagSize--;

		if ( isPlayerDriver )
		{
			weaponSlotEvents.Raise( UIEvent.ThrowerOn, currentMagSize / parameters.MagSize );
		}

		if ( currentMagSize <= 0 )
		{
			currentMagSize = (int)parameters.MagSize;
			realoadTimeLeft = parameters.RealoadTime;
		}
	}
}

