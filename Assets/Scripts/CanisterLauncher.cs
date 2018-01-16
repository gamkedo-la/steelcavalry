using UnityEngine;
using UnityEngine.Assertions;

public class CanisterLauncher : MonoBehaviour
{
	[SerializeField] private Transform spawnPoint;
	[SerializeField] private GameObject projectile = null;
	[SerializeField] private float force = 20;

	private bool isActive = false;
	//private bool isRight = false;

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

	public void SetDir( bool isRight )
	{
		//this.isRight = isRight;
	}

	public void Active( bool isActive )
	{
		this.isActive = isActive;
	}

	public void HandleFire( )
	{
		GameObject shotGO = Instantiate
		(
			projectile,
			spawnPoint.position,
			Quaternion.Euler
			(
				spawnPoint.rotation.eulerAngles.x,
				spawnPoint.rotation.eulerAngles.y,
				spawnPoint.rotation.eulerAngles.z + Random.Range( 0f, 15f )
			)
		);

		Rigidbody2D shotRB = shotGO.GetComponent<Rigidbody2D>( );
		shotRB.velocity = shotGO.transform.rotation * Vector2.left * force;

		shotGO.transform.SetParent( LitterContainer.instanceTransform );
	}
}

