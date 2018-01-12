using UnityEngine;
using UnityEngine.Assertions;

public class Gun : MonoBehaviour
{
	[SerializeField] private Transform spawnPoint;
	[SerializeField] private GameObject projectile = null;
	[SerializeField] private float minAngle = -60f;
	[SerializeField] private float maxAngle = 60f;

	private bool isActive = false;
	private bool isRight = false;
	private float xAngle;

	void Start( )
	{
		Assert.IsNotNull( projectile );
		Assert.IsNotNull( spawnPoint );
	}

	void Update( )
	{
		if ( !isActive ) return;

		LookAtCursor( );
	}

	public void Active(bool isActive)
	{
		this.isActive = isActive;
	}

	public void SetDir( bool isRight )
	{
		this.isRight = isRight;
	}

	public void HandleFire( )
	{
		GameObject shotGO = Instantiate( projectile, spawnPoint.position, Quaternion.Euler(0, 0, -xAngle + Random.Range( -5f, 5f ) ) );
		Rigidbody2D shotRB = shotGO.GetComponent<Rigidbody2D>( );

		shotRB.velocity = shotGO.transform.rotation * Vector2.right * 20.0f;
		shotGO.transform.SetParent( LitterContainer.instanceTransform );
	}

	private void LookAtCursor( )
	{
		if ( !( Camera.main.ScreenToWorldPoint( Input.mousePosition ).x < transform.position.x && !isRight ) &&
			 !( Camera.main.ScreenToWorldPoint( Input.mousePosition ).x > transform.position.x && isRight ) )
			return;

		Vector3 diff = Camera.main.ScreenToWorldPoint( Input.mousePosition ) - transform.position;
		diff.Normalize( );

		xAngle = -1 * Mathf.Atan2( diff.y, diff.x ) * Mathf.Rad2Deg;
		if(isRight)
		{
			xAngle = Mathf.Clamp( xAngle, minAngle, maxAngle );
		}
		else
		{
			if (xAngle > 0)
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
}
