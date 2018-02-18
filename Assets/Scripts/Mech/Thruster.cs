using UnityEngine;
using UnityEngine.Assertions;

public class Thruster : MonoBehaviour
{
	[SerializeField] private ParticleSystem thrusterParticle = null;
	[SerializeField] private Light thrusterLight = null;

	private float lightStrength;

	void Start ()
	{
		Assert.IsNotNull( thrusterParticle );
		Assert.IsNotNull( thrusterLight );

		lightStrength = thrusterLight.intensity;
	}

	void Update ()
	{

	}

	public void On()
	{
		thrusterParticle.Play( );
		thrusterLight.enabled = true;

		Invoke( "BlinkLight", Random.Range( 0.01f, 0.1f ) );
	}

	public void Off( )
	{
		thrusterParticle.Stop( );
		thrusterLight.enabled = false;

		CancelInvoke( );
	}

	private void BlinkLight()
	{
		thrusterLight.intensity = Random.Range( lightStrength * 0.5f, lightStrength );

		Invoke( "BlinkLight", Random.Range( 0.01f, 0.1f ) );
	}
}
