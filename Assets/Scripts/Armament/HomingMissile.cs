using UnityEngine;
using UnityEngine.Assertions;

public class HomingMissile : MonoBehaviour
{
	[SerializeField] private GameEventAudioEvent audioEvent;
	[SerializeField] private GameObject explosion = null;
	[SerializeField] private GameEventFloat didDamageEvent = null;
	[SerializeField] private float speed = 5f;
	[SerializeField] private float rotatingSpeed= 200f;
	[SerializeField] private float damagePerMissile = 20f;

	private Transform target;
	private Rigidbody2D rb;

	[HideInInspector]
	public int playerNumber;

	void Start( )
	{
		rb = GetComponent<Rigidbody2D>( );

		Assert.IsNotNull( rb );
		Assert.IsNotNull( explosion );
		Assert.IsNotNull( didDamageEvent );
		Assert.IsNotNull( audioEvent );
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
		HP hp = collision.collider.GetComponent<HP>();
		if (hp)
		{
			didDamageEvent.Raise( 0.5f );
			hp.TakeDamage(damagePerMissile);
		}

		if ( collision.contacts.Length > 0 )
			DoDestruction( collision.contacts[0].point );
		else
			DoDestruction( transform.position );
	}

	public void DoDestruction( Vector2 point )
	{
		audioEvent.Raise( AudioEvents.Explosion, transform.position );

		var exp = Instantiate( explosion, point, Quaternion.identity );

		Destroy( exp, 2f );
		Destroy( gameObject );
	}

	public void SetTarget( Transform target )
	{
		this.target = target;
	}

	public void SetDamage( float damage )
	{
		damagePerMissile = damage;
	}
}
