using UnityEngine;

public class ExplosionEnlarger : MonoBehaviour
{
	[SerializeField] private float speed = 1f;

	void Update ()
	{
		transform.localScale += Vector3.one * speed * Time.deltaTime;
	}
}
