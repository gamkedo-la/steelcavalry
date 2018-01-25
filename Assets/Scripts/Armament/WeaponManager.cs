using UnityEngine;
using UnityEngine.Assertions;

public class WeaponManager : MonoBehaviour
{
	[SerializeField] Transform turretMountPoint = null;
	[SerializeField] Transform launcherMountPoint = null;
	[SerializeField] Transform throwerMountPoint = null;

	[SerializeField] IWeapon turret = null;
	[SerializeField] IWeapon launcher = null;
	[SerializeField] IWeapon thrower = null;

	private bool isActive = false;

	void Start ()
	{
		Debug.Log( name + " weapons are attached" );

		Assert.IsNotNull( turretMountPoint );
		Assert.IsNotNull( launcherMountPoint );
		Assert.IsNotNull( throwerMountPoint );

		turret = turretMountPoint.GetChild(0).GetComponent<IWeapon>( );
		launcher = launcherMountPoint.GetChild( 0 ).GetComponent<IWeapon>( );
		thrower = throwerMountPoint.GetChild( 0 ).GetComponent<IWeapon>( );

		Assert.IsNotNull( turret );
		Assert.IsNotNull( launcher );
		Assert.IsNotNull( thrower );
	}

	void Update ()
	{

	}

	public void IsActive( bool active )
	{
		Debug.Log( name + " weapons are active: " + active );

		isActive = active;

		turret.Active( isActive );
		launcher.Active( isActive );
		thrower.Active( isActive );
	}

	public void SetDir( bool isRight )
	{
		turret.SetDir( isRight );
		launcher.SetDir( isRight );
		thrower.SetDir( isRight );
	}

	public void FirePrimary()
	{
		turret.TryToFire( );
	}

	public void FireSecondary( )
	{
		launcher.TryToFire( );
	}

	public void FireTertiary( )
	{
		thrower.TryToFire( );
	}
}
