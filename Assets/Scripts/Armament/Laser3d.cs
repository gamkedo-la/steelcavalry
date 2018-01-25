using UnityEngine;
using UnityEngine.Assertions;

public class Laser3d : MonoBehaviour, IWeapon
{
	[SerializeField] private Transform spawnPoint = null;
	[SerializeField] private Transform laserBeem = null;
	[SerializeField] private GameEventFloat didDamageEvent = null;
	[SerializeField] private float maxLaserSize = 20f;
	[SerializeField] private float laserScaleCorrection = 1f;
	[SerializeField] private float laserMoveCorrection = 1f;

	[SerializeField] private float damagePerSecond = 2f;
	[SerializeField] private float minAngle = -60f;
	[SerializeField] private float maxAngle = 60f;

	private bool isActive = false;
	private bool isRight = false;
	private float xAngle;
	private Transform beem;

	void Start( )
	{
		Assert.IsNotNull( spawnPoint );
		Assert.IsNotNull( laserBeem );
		Assert.IsNotNull( didDamageEvent );

		beem = Instantiate( laserBeem, spawnPoint.position, Quaternion.identity, spawnPoint );
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

	public void SetDir( bool isRight )
	{
		this.isRight = isRight;
	}

	public void TryToFire( )
	{
		if ( spawnPoint == null )
			return;

		ShootLaser( );
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
			beem.localScale = new Vector3( beem.localScale.x, beem.localScale.y, currentLaserSize * laserScaleCorrection );
			beem.localPosition = new Vector3( beem.localPosition.x, beem.localPosition.y, currentLaserSize * laserMoveCorrection );

			//Debug.DrawLine( spawnPoint.position, hit.point, Color.red );

			Mech mechInstance = hit.collider.GetComponent<Mech>( );
			if ( mechInstance )
			{
				didDamageEvent.Raise( 0.1f );
				mechInstance.TakeDamage( damagePerSecond * Time.deltaTime );
			}
		}
		else
		{
			// Shooting in the air :(
			currentLaserSize = maxLaserSize;
			beem.localScale = new Vector3( beem.localScale.x, beem.localScale.y, currentLaserSize * laserScaleCorrection );
			beem.localPosition = new Vector3( beem.localPosition.x, beem.localPosition.y, currentLaserSize * laserMoveCorrection );
		}
	}
}
