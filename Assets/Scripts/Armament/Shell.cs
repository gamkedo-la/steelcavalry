using UnityEngine;
using UnityEngine.Assertions;

public class Shell : MonoBehaviour
{
	[SerializeField] private SpriteRenderer sp = null;
	[SerializeField] private Gradient color;
	[SerializeField] private float timeToDissapear = 5f;

	private float timeLeft;

	void Start ()
	{
		Assert.IsNotNull( sp );

		timeLeft = timeToDissapear;
	}

	void Update ()
	{
		timeLeft -= Time.deltaTime;
		if (timeLeft < 0)
		{
			Destroy( gameObject );
			return;
		}

		sp.color = color.Evaluate( 1 - timeLeft / timeToDissapear );
	}
}
