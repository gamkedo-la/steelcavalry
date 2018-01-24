using UnityEngine;
using UnityEngine.Assertions;

public class CanisterGranade : MonoBehaviour
{
	[SerializeField] private GameObject explosion = null;

	void Start( )
	{
		Assert.IsNotNull( explosion );
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
		var exp = Instantiate( explosion, point, Quaternion.identity );

		Destroy( exp, 2f );
		Destroy( gameObject );
	}
}