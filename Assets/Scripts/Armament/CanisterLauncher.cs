using UnityEngine;
using UnityEngine.Assertions;

public class CanisterLauncher : MonoBehaviour, IWeapon
{
	[SerializeField] private Transform spawnPoint;
	[SerializeField] private GameObject projectile = null;
	[SerializeField] private WeaponType type = WeaponType.Thrower;
	[SerializeField] private float force = 20;

	public WeaponType Type
	{
		get { return type; }
	}

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

	public void SetDir( bool isRight ) { }

	public void Active( bool isActive )
	{
		this.isActive = isActive;
	}

	public GameObject GetGameObject( )
	{
		return gameObject;
	}

	public void TryToFire( )
	{
		if ( spawnPoint == null ) return;

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

