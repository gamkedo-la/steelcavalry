using UnityEngine;
using UnityEngine.Assertions;

public class FadePart : MonoBehaviour
{
	[SerializeField] private float fadePerSecond = 0.1f;
	[SerializeField] private bool destroyAtTheEnd = true;

	private Material material;
	private float alpha = 1f;

	void Start ()
	{
		material = GetComponent<Renderer>( ).material;
		Assert.IsNotNull( material );
	}

	void Update( )
	{
		var color = material.color;
		alpha = alpha - ( fadePerSecond * Time.deltaTime );

		material.color = new Color( material.color.r, material.color.g, material.color.b, alpha );

		if ( alpha <= 0 && destroyAtTheEnd )
		{
			Destroy( gameObject );
		}
		else if ( alpha <= 0 )
		{
			enabled = false;
		}
	}
}
