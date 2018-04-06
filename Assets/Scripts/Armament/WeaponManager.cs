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

	[SerializeField] private GameEventUI weaponSlotEvents = null;

	private MouseCursor cursor;

    private bool isActive = false;
	private bool isRight = false;
	private bool isPlayerDriver = false;

    public bool launcherMounted = false;//using for animation.


    void Start ()
	{
        //Debug.Log( name + " weapons are attached" );

        cursor = UIResourceManager.MouseCursor;

		Assert.IsNotNull( turretMountPoint );
		Assert.IsNotNull( launcherMountPoint );
		Assert.IsNotNull( throwerMountPoint );

		Assert.IsNotNull( cursor ); // FIXME: null is valid for bots since they do not use the mouse cursor
		Assert.IsNotNull( weaponSlotEvents );

		if ( turretMountPoint.transform.childCount > 0 )
			turret = turretMountPoint.GetChild( 0 ).GetComponent<IWeapon>( );

		if ( launcherMountPoint.transform.childCount > 0 )
			launcher = launcherMountPoint.GetChild( 0 ).GetComponent<IWeapon>( );

		if ( throwerMountPoint.transform.childCount > 0 )
			thrower = throwerMountPoint.GetChild( 0 ).GetComponent<IWeapon>( );
	}

	public void IsPlayerDriving( bool playerDriver )
	{
		//Debug.Log( name + " weapons are used by human player: " + playerDriver );

		isPlayerDriver = playerDriver;

		if ( turret != null )
			turret.IsPlayerDriving( isPlayerDriver );

		if ( launcher != null )
			launcher.IsPlayerDriving( isPlayerDriver );

		if ( thrower != null )
			thrower.IsPlayerDriving( isPlayerDriver );

		if ( isPlayerDriver )
			weaponSlotEvents.Raise( UIEvent.ThusterOn );
	}

	public void IsActive( bool active )
	{
		//Debug.Log( name + " weapons are active: " + active );

		isActive = active;

		if ( turret != null )
			turret.Active( isActive );

		if ( launcher != null )
			launcher.Active( isActive );

		if ( thrower != null )
			thrower.Active( isActive );
	}

	public void SwapModel( string mechName )
	{
		if ( turret != null )
			turret.SwapModel( mechName );

		if ( launcher != null )
			launcher.SwapModel( mechName );

		if ( thrower != null )
			thrower.SwapModel( mechName );
	}

	public void SetDir( bool right )
	{
		isRight = right;

		if ( turret != null )
			turret.SetDir( isRight );

		if ( launcher != null )
			launcher.SetDir( isRight );

		if ( thrower != null )
			thrower.SetDir( isRight );
	}

	public void FirePrimary()
	{
		if ( turret != null )
			turret.TryToFire( );
	}

	public void FireSecondary( )
	{
		if ( launcher != null )
			launcher.TryToFire( );
	}

	public void FireTertiary( )
	{
		if ( thrower != null )
			thrower.TryToFire( );
	}

	public void GiveWeapon( IWeapon weapon )
	{
		Debug.Log( "I've received " + weapon.Type.ToString() );
		GameObject g;
        //Debug.Log("weapon type " + weapon.Type);
        switch ( weapon.Type )
		{
			case WeaponType.Turret:
			{
				if ( turret != null )
				{
					Destroy( turret.GetGameObject( ) );
				}

				g = Instantiate( weapon.GetGameObject( ), turretMountPoint.position, Quaternion.identity, turretMountPoint );
				g.transform.localPosition = Vector3.zero;

				turret = g.GetComponent<IWeapon>( );

				IsPlayerDriving( isPlayerDriver );
				SetDir( isRight );
				IsActive( isActive );

				var mech = GetComponent<Mech>( );
				if ( mech ) SwapModel( mech.GetName( ) );
			}
			break;

			case WeaponType.Launcher:
			{
				if ( launcher != null )
				{
					Destroy( launcher.GetGameObject( ) );
				}

				g = Instantiate( weapon.GetGameObject( ), launcherMountPoint.position, Quaternion.identity, launcherMountPoint );

                launcherMounted = true;//using for animation. TODO: need to set when launcher is dismounted if that can happen

				g.transform.localPosition = Vector3.zero;
				g.transform.localRotation = Quaternion.Euler
				(
					g.transform.localRotation.eulerAngles.x,
					0,
					g.transform.localRotation.eulerAngles.z
				);

				launcher = g.GetComponent<IWeapon>( );
				g.GetComponent<MissileLauncher>( ).SetCursor( cursor );

				IsPlayerDriving( isPlayerDriver );
				SetDir( isRight );
				IsActive( isActive );
			}
			break;

			case WeaponType.Thrower:
			{
				if ( thrower != null )
				{
					Destroy( thrower.GetGameObject( ) );
				}

				g = Instantiate( weapon.GetGameObject( ), throwerMountPoint.position, Quaternion.identity, throwerMountPoint );
				g.transform.localPosition = Vector3.zero;
				g.transform.localRotation = Quaternion.Euler ( 0, 0, 0 );

				thrower = g.GetComponent<IWeapon>( );

				IsPlayerDriving( isPlayerDriver );
				SetDir( isRight );
				IsActive( isActive );
			}
			break;

			default:
				Debug.LogError( "Ups, " + name + " received a weapon of type " + weapon.Type + " and dos not know what to do with it" );
			break;
		}
	}
}
