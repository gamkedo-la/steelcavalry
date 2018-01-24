using UnityEngine;
using UnityEngine.Assertions;

public class HomingMissile : MonoBehaviour
{
	[SerializeField] private GameObject explosion = null;
	[SerializeField] private GameEventFloat didDamageEvent = null;
	[SerializeField] private float speed = 5f;
	[SerializeField] private float rotatingSpeed= 200f;
	[SerializeField] private float damagePerMissile = 20f;

	private Transform target;
	private Rigidbody2D rb;

	void Start( )
	{
		rb = GetComponent<Rigidbody2D>( );

		Assert.IsNotNull( rb );
		Assert.IsNotNull( explosion );
		Assert.IsNotNull( didDamageEvent );
	}

	void FixedUpdate( )
	{
		Vector2 targetPos = Vector2.zero;
		if (target == null)
		{
			targetPos = Utilities.GetMouseWorldPosition( Input.mousePosition );
		}

		if (Vector2.Distance(targetPos, transform.position) <= 0.1f)
		{
			DoDestruction( transform.position );
			return;
		}

		Vector2 vector = (Vector2)transform.position - targetPos;
		vector.Normalize( );
		float moveVector = Vector3.Cross( vector, transform.right ).z;

		rb.angularVelocity = rotatingSpeed * moveVector;
		rb.velocity = transform.right * speed;
	}

	private void OnCollisionEnter2D( Collision2D collision )
	{
		// Try to find a Mech script on the hit object
		Mech mechInstance = collision.collider.GetComponent<Mech>();
		if (mechInstance)
		{
			didDamageEvent.Raise( 0.5f );
			mechInstance.TakeDamage(damagePerMissile);
		}

		if ( collision.contacts.Length > 0 )
			DoDestruction( collision.contacts[0].point );
		else
			DoDestruction( transform.position );
	}

	private void DoDestruction( Vector2 point )
	{
		var exp = Instantiate( explosion, point, Quaternion.identity );

		Destroy( exp, 2f );
		Destroy( gameObject );
	}

	public void SetTarget( Transform target )
	{
		this.target = target;
	}
}
