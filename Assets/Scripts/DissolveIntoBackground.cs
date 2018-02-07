using UnityEngine;

public class DissolveIntoBackground : MonoBehaviour
{
	public float Delay = 3f;
	public float DelayDelta = 0.3f;
	public float DecayTime = 1f;

	private bool decaing = false;
	private SpriteRenderer sprite;
	private float targetColorValue = 0.5f;
	private float targetZ = 0.5f;
	private float oldZ;
	private float oldDecayTime;

	void Start ()
	{
		sprite = GetComponent<SpriteRenderer>( );
		oldZ = transform.position.z;
		oldDecayTime = DecayTime;
	}

	void Update ()
	{
		Delay -= Time.deltaTime;

		if (!decaing && Delay <= 0)
		{
			Destroy( GetComponent<Rigidbody2D>( ) );
			Destroy( GetComponent<BoxCollider2D>( ) );
		}

		if ( !decaing && Delay + DelayDelta <= 0 )
		{
			decaing = true;
		}

		if (decaing)
		{
			DecayTime -= Time.deltaTime;

			if (DecayTime >= 0)
			{
				float z = Mathf.Lerp( oldZ, targetZ, DecayTime / oldDecayTime );
				transform.position = new Vector3( transform.position.x, transform.position.y, z );

				float v = Mathf.Lerp( targetColorValue, 1.0f, DecayTime / oldDecayTime );
				sprite.color = Color.HSVToRGB( 0, 0, v );
			}
			else
			{
				Destroy( this );
			}
		}
	}
}
