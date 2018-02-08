using UnityEngine;
using UnityEngine.Assertions;

public class ShotBreaksIntoParticle : MonoBehaviour
{
	[SerializeField] private GameEventFloat didDamageEvent = null;
	[SerializeField] private GameObject pfx;
	[SerializeField] private float damagePerShot = 10.0f;

	private GameObject player;
	private string nameOfMechPlayerIsIn;
	private string nameOfObjectHit;

	void Start()
	{
		Assert.IsNotNull( didDamageEvent );

		player = GameObject.FindWithTag("Player");
	}

	void OnCollisionEnter2D(Collision2D bumpFacts) {
		/*Debug.Log("Shot hit: " + bumpFacts.collider.gameObject.name +
		"Reminder: using Physics2D Layer ignore shenanigans for demo");*/

		nameOfMechPlayerIsIn = player.GetComponent<PlayerMovement>().getNameOfMechPlayerIsIn();
		nameOfObjectHit = bumpFacts.collider.gameObject.name;

		// If the shot is from the player, ignore it
		if(nameOfMechPlayerIsIn == nameOfObjectHit) return;

		// Try to find a Mech script on the hit object
		HP hp = bumpFacts.collider.GetComponent<HP>();
		if (hp)
		{
			didDamageEvent.Raise( 0.1f );
			hp.TakeDamage(damagePerShot);
		}

		GameObject pfxGO = Instantiate(pfx, transform.position, transform.rotation);
		pfxGO.transform.SetParent(LitterContainer.instanceTransform);
		Destroy(gameObject);
	}

	public void SetDamage( float damage )
	{
		damagePerShot = damage;
	}
}
