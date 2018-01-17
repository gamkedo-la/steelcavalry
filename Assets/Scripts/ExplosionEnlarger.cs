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
		Mech mechInstance = bumpFacts.collider.GetComponent<Mech>( );
		if ( mechInstance )
		{
			didDamageEvent.Raise( 1f );
			mechInstance.TakeDamage( damage );
		}
	}
}
