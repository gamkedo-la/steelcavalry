using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class HPChanged : UnityEvent<float> { }

public class HP : MonoBehaviour
{
	[SerializeField] private float maxHp = 100f;
	[SerializeField] private HPChanged onHPChanged;
	[SerializeField] private UnityEvent onDeath;

	private bool useMultiplier = false;
	private float damageTaken = 0;

	public void UseMultiplier (bool use)
	{
		useMultiplier = use;
	}

	public void TakeDamage( float damageAmount )
	{
		if ( useMultiplier ) {
			damageAmount *= 1.25f;
		}

		damageTaken += damageAmount;
		damageTaken = damageTaken > maxHp ? maxHp : damageTaken;

		onHPChanged.Invoke( 1f - damageTaken / maxHp );

		if (damageTaken >= maxHp) {
			damageTaken = 0; // reset to 0 on death
			onDeath.Invoke( );
		}
	}
}
