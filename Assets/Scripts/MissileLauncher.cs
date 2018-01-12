using UnityEngine;
using UnityEngine.Assertions;

public class MissileLauncher : MonoBehaviour
{
	[SerializeField] private Transform spawnPoint;
	[SerializeField] private GameObject projectile = null;

	private bool isActive = false;

	void Start( )
	{
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
		GameObject shotGO = Instantiate( projectile, spawnPoint.position, Quaternion.Euler( 0, 0, 90 + Random.Range( -15f, 15f ) ) );

		shotGO.transform.SetParent( LitterContainer.instanceTransform );
	}
}