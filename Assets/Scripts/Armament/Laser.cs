using UnityEngine;
using UnityEngine.Assertions;

public class Laser : MonoBehaviour, IWeapon
{
	[SerializeField] private Transform spawnPoint = null;
	[SerializeField] private WeaponParameters parameters = null;
	[SerializeField] private GameEventFloat didDamageEvent = null;
	[SerializeField] private WeaponType type = WeaponType.Turret;
	[SerializeField] private float maxLaserSize = 20f;
	[SerializeField] private float laserScaleCorrection = 1f;
	[SerializeField] private float laserMoveCorrection = 1f;
	[SerializeField] private float minAngle = -60f;
	[SerializeField] private float maxAngle = 60f;

	public WeaponType Type
	{
		get { return type; }
	}

	private bool isActive = false;
	private bool isRight = false;
	private float xAngle;
	private GameObject beem;

	private float realoadTimeLeft = 0;
	private float shootingTimeleft = 0;

	void Start( )
	{
		Assert.IsNotNull( spawnPoint );
		Assert.IsNotNull( parameters );
		Assert.IsNotNull( parameters.Projectile );
		Assert.IsNotNull( didDamageEvent );

		beem = Instantiate( parameters.Projectile, spawnPoint.position, Quaternion.identity, spawnPoint );
		beem.transform.localRotation = Quaternion.identity;

		shootingTimeleft = parameters.MagSize;
	}

	void Update( )
	{
		if ( !isActive )
			return;

		realoadTimeLeft -= Time.deltaTime;
	}

	void FixedUpdate( )
	{
		beem.gameObject.SetActive( false );

		if ( !isActive )
			return;

		LookAtCursor( );
	}

	public void Active( bool isActive )
	{
		this.isActive = isActive;
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

		if ( shootingTimeleft <= 0 )
		{
			shootingTimeleft = parameters.MagSize;
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

		transform.rotation = Quaternion.Euler( xAngle, yAngle, 0f );
	}

	private void ShootLaser()
	{
		beem.gameObject.SetActive( true );

		Vector2 laserDirection = spawnPoint.rotation * ( spawnPoint.right * -1 );
		float currentLaserSize = maxLaserSize;

		RaycastHit2D hit = Physics2D.Raycast( spawnPoint.position, laserDirection, maxLaserSize );

		if ( hit.collider != null )
		{
			currentLaserSize = Vector2.Distance( spawnPoint.position, hit.point );
			beem.transform.localScale = new Vector3( beem.transform.localScale.x, beem.transform.localScale.y, currentLaserSize * laserScaleCorrection );
			beem.transform.localPosition = new Vector3( beem.transform.localPosition.x, beem.transform.localPosition.y, currentLaserSize * laserMoveCorrection );

			//Debug.DrawLine( spawnPoint.position, hit.point, Color.red );

			Mech mechInstance = hit.collider.GetComponent<Mech>( );
			if ( mechInstance )
			{
				didDamageEvent.Raise( 0.1f );
				mechInstance.TakeDamage( parameters.GetDamage( ) * Time.deltaTime );
			}
		}
		else
		{
			// Shooting in the air :(
			currentLaserSize = maxLaserSize;
			beem.transform.localScale = new Vector3( beem.transform.localScale.x, beem.transform.localScale.y, currentLaserSize * laserScaleCorrection );
			beem.transform.localPosition = new Vector3( beem.transform.localPosition.x, beem.transform.localPosition.y, currentLaserSize * laserMoveCorrection );
		}
	}
}