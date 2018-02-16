using UnityEngine;
using UnityEngine.Assertions;

public class CanisterGrenade : MonoBehaviour
{
	[SerializeField] private GameObject explosionPrefab = null;

	private float explosionDamage = 10f;

	void Start( )
	{
		Assert.IsNotNull( explosionPrefab );
	}

	private void OnCollisionEnter2D( Collision2D collision )
	{
		if ( !collision.gameObject.CompareTag( "PlayerProjectile" ) &&
			 !collision.gameObject.CompareTag( "PlayerMissile" ) &&
			 !collision.gameObject.CompareTag( "Explosion" ) )
			return;

		DoDestruction( transform.position );
	}

	private void DoDestruction( Vector2 point )
	{
		var explosion = Instantiate(explosionPrefab, point, Quaternion.identity);
		var explosionWave = explosion.gameObject.transform.GetChild(0);
		var explosionEnlarger = explosionWave.GetComponent<ExplosionEnlarger>();
		explosionEnlarger.SetDamage(explosionDamage);

		Destroy(explosion, 2f);
		Destroy(gameObject);
	}

	public void SetDamage( float damage )
	{
		explosionDamage = damage;
	}
}
