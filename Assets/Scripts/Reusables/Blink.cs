using UnityEngine;
using UnityEngine.Assertions;

public class Blink : MonoBehaviour
{
	[SerializeField] private SpriteRenderer objToBlink = null;
	[SerializeField, Tooltip( "One full cycle" )] private float interval = 1f;

	void Start ()
	{
		Assert.IsNotNull( objToBlink );

		Invoke( "On", 0f );
	}

	private void On()
	{
		objToBlink.enabled = true;
		Invoke( "Off", interval / 2f );
	}

	private void Off( )
	{
		objToBlink.enabled = false;
		Invoke( "On", interval / 2f );
	}
}
