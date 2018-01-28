using UnityEngine;

[CreateAssetMenu]
public class WeaponParameters : ScriptableObject
{
	[Header( "Objects" )]
	public GameObject Projectile = null;

	[Header( "Damage & speed" )]
	[Tooltip( "Minimum damage (per second or instant, depending on weapon type)" )]
	public float DamageMin = 10f;

	[Tooltip( "Maximum damage (per second or instant, depending on weapon type)" )]
	public float DamageMax = 12f;

	[Tooltip( "Initial force (speed) given to the projectile" )]
	public float Force = 20f;

	[Header( "Mag" )]
	[Tooltip( "For laser type weapons it's the shooting time" )]
	public float MagSize = 10f;

	[Header( "Timers" )]
	public float DelayBetweenShots = 0.1f;
	public float RealoadTime = 3f;

	public float GetDamage()
	{
		return Random.Range( DamageMin, DamageMax );
	}
}
