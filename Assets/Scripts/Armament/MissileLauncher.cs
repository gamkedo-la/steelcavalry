using UnityEngine;
using UnityEngine.Assertions;

public class MissileLauncher : MonoBehaviour, IWeapon
{
	[SerializeField] private MouseCursor cursor;
	[SerializeField] private Transform spawnPoint;
	[SerializeField] private GameObject projectile = null;
	[SerializeField] private WeaponType type = WeaponType.Launcher;

	public WeaponType Type
	{
		get { return type; }
	}

	private bool isActive = false;

	void Start( )
	{
		Assert.IsNotNull( cursor );
		Assert.IsNotNull( projectile );
		Assert.IsNotNull( spawnPoint );
	}

	void Update( )
	{
		if ( !isActive )
			return;
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

		GameObject missile = Instantiate( projectile, spawnPoint.position, Quaternion.Euler( 0, 0, 90 + Random.Range( -15f, 15f ) ) );

		cursor.AddMissile( missile );
		missile.transform.SetParent( LitterContainer.instanceTransform );
	}

	public void SetDir( bool isRight ) { }
}