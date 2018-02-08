using UnityEngine;
using UnityEngine.Assertions;

public class ExplosionEnlarger : MonoBehaviour
{
	[SerializeField] private GameEventFloat didDamageEvent = null;

	[SerializeField] private float speed = 1f;
	[SerializeField] private float damage = 50f;

	void Start( )
	{
		Assert.IsNotNull( didDamageEvent );
	}

	void Update ()
	{
		transform.localScale += Vector3.one * speed * Time.deltaTime;
	}

	void OnCollisionEnter2D( Collision2D bumpFacts )
	{
		// Try to find a Mech script on the hit object
		HP hp = bumpFacts.collider.GetComponent<HP>( );
		if ( hp )
		{
			didDamageEvent.Raise( 1f );
			hp.TakeDamage( damage );
		}
	}

	public void SetDamage( float damage )
	{
		this.damage = damage;
	}
}
