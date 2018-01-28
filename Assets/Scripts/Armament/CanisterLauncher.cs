using UnityEngine;
using UnityEngine.Assertions;

public class CanisterLauncher : MonoBehaviour, IWeapon
{
	[SerializeField] private Transform spawnPoint;
	[SerializeField] private WeaponParameters parameters = null;
	[SerializeField] private WeaponType type = WeaponType.Thrower;

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

		currentMagSize = (int)parameters.MagSize;
	}

	void Update( )
	{
		if ( !isActive )
			return;

		realoadTimeLeft -= Time.deltaTime;
		timeToNextShot -= Time.deltaTime;
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
		shotGO.GetComponent<CanisterGranade>( ).SetDamage( parameters.GetDamage( ) );

		shotGO.transform.SetParent( LitterContainer.instanceTransform );

		timeToNextShot = parameters.DelayBetweenShots;
		currentMagSize--;

		if ( currentMagSize <= 0 )
		{
			currentMagSize = (int)parameters.MagSize;
			realoadTimeLeft = parameters.RealoadTime;
		}
	}
}

