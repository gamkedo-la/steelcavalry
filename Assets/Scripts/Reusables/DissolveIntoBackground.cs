using UnityEngine;

public class DissolveIntoBackground : MonoBehaviour
{
	public float delay = 3f;
	public float delayDelta = 0.3f;
	public float decayTime = 1f;

	private bool isDecaying = false;
	private SpriteRenderer sprite;
	private float targetColorValue = 0.5f;
	private float targetZ = 0.5f;
	private float oldZ;
	private float oldDecayTime;

	void Start ()
	{
		sprite = GetComponent<SpriteRenderer>( );
		oldZ = transform.position.z;
		oldDecayTime = decayTime;
	}

	void Update ()
	{
		delay -= Time.deltaTime;

		if (!isDecaying && delay <= 0)
		{
			Destroy( GetComponent<Rigidbody2D>( ) );
			Destroy( GetComponent<BoxCollider2D>( ) );
		}

		if ( !isDecaying && delay + delayDelta <= 0 )
		{
			isDecaying = true;
		}

		if (isDecaying)
		{            
			decayTime -= Time.deltaTime;

			if (decayTime >= 0)
			{
				float z = Mathf.Lerp( oldZ, targetZ, decayTime / oldDecayTime );
				transform.position = new Vector3( transform.position.x, transform.position.y, z );

				float v = Mathf.Lerp( targetColorValue, 1.0f, decayTime / oldDecayTime );
				sprite.color = Color.HSVToRGB( 0, 0, v );
			}
			else
			{
				Destroy( this );
			}
		}
	}
}
