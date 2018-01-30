using UnityEngine;
using UnityEngine.Assertions;

public class MissileLauncher : MonoBehaviour, IWeapon
{
	[SerializeField] private MouseCursor cursor;
	[SerializeField] private Transform spawnPoint;
	[SerializeField] private WeaponParameters parameters = null;
	[SerializeField] private WeaponType type = WeaponType.Launcher;

	private float timeToNextShot = 0;
	private float realoadTimeLeft = 0;
	private int currentMagSize = 0;

	public WeaponType Type
	{
		get { return type; }
	}

	private bool isActive = false;

	void Start( )
	{
		Assert.IsNotNull( parameters );
		Assert.IsNotNull( parameters.Projectile );
		Assert.IsNotNull( spawnPoint );
		Assert.IsNotNull( cursor );

		currentMagSize = (int)parameters.MagSize;
	}

	void Update( )
	{
		if ( !isActive )
			return;

		realoadTimeLeft -= Time.deltaTime;
		timeToNextShot -= Time.deltaTime;
	}

	public void IsPlayerDriving( bool playerDriver )
	{

	}

	public void Active( bool isActive )
	{
		this.isActive = isActive;
	}

	public void SetCursor( MouseCursor cursor )
	{
		this.cursor = cursor;
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

		GameObject missile = Instantiate( parameters.Projectile, spawnPoint.position, Quaternion.Euler( 0, 0, 90 + Random.Range( -15f, 15f ) ) );
		missile.GetComponent<HomingMissile>( ).SetDamage( parameters.GetDamage( ) );

		cursor.AddMissile( missile );
		missile.transform.SetParent( LitterContainer.instanceTransform );

		timeToNextShot = parameters.DelayBetweenShots;
		currentMagSize--;

		if ( currentMagSize <= 0 )
		{
			currentMagSize = (int)parameters.MagSize;
			realoadTimeLeft = parameters.RealoadTime;
		}
	}

	public void SetDir( bool isRight ) { }
}