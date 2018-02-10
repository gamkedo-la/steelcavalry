using UnityEngine;
using UnityEngine.Assertions;

public class Thruster : MonoBehaviour
{
	[SerializeField] private ParticleSystem thrusterParticle = null;
	[SerializeField] private Light thrusterLight = null;

	void Start ()
	{
		Assert.IsNotNull( thrusterParticle );
		Assert.IsNotNull( thrusterLight );
	}

	void Update ()
	{

	}

	public void On()
	{

	}

	public void Off( )
	{

	}
}
