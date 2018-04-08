using UnityEngine;
using UnityEngine.Assertions;

public class CanisterGrenade : MonoBehaviour
{
    [HideInInspector] public Team fromTeam;

	[SerializeField] private GameObject explosionPrefab = null;
	[SerializeField] private GameEventAudioEvent explosionAudio = null;

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

	public void DoDestruction( Vector2 point )
	{
		explosionAudio.Raise( AudioEvents.MineExplosion, transform.position );
		var explosion = Instantiate(explosionPrefab, point, Quaternion.identity);
		var explosionWave = explosion.gameObject.transform.GetChild(0);
		ExplosionEnlarger explosionEnlarger = explosionWave.GetComponent<ExplosionEnlarger>();
        explosionEnlarger.fromTeam = fromTeam;
		explosionEnlarger.SetDamage(explosionDamage);

		Destroy(explosion, 2f);
		Destroy(gameObject);
	}

	public void SetDamage( float damage )
	{
		explosionDamage = damage;
	}
}
