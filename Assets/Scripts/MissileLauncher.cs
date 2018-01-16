using UnityEngine;
using UnityEngine.Assertions;

public class MissileLauncher : MonoBehaviour
{
	[SerializeField] private MouseCursor cursor;
	[SerializeField] private Transform spawnPoint;
	[SerializeField] private GameObject projectile = null;

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

	public void HandleFire( )
	{
		GameObject missile = Instantiate( projectile, spawnPoint.position, Quaternion.Euler( 0, 0, 90 + Random.Range( -15f, 15f ) ) );

		cursor.AddMissile( missile );
		missile.transform.SetParent( LitterContainer.instanceTransform );
	}
}