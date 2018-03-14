using UnityEngine;
using UnityEngine.Assertions;

public class Dust : MonoBehaviour
{
	[SerializeField] private GameEventAudioEvent audioEvent = null;
	[SerializeField] private GameObject dust = null;

	void Start ()
	{
		Assert.IsNotNull( audioEvent );
		Assert.IsNotNull( dust );
	}

	private void OnTriggerEnter2D( Collider2D collision )
	{
		if ( !collision.gameObject.CompareTag( "Ground" ) ) return;

		audioEvent.Raise( AudioEvents.MechLand, transform.position );

		GameObject dustInstance = Instantiate( dust, transform.position, Quaternion.identity );
		Destroy( dustInstance, 2f );
	}
}
