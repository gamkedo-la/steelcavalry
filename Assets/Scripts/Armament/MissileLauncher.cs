using UnityEngine;
using UnityEngine.Assertions;

public class MissileLauncher : MonoBehaviour, IWeapon
{
	[SerializeField] private GameEventAudioEvent audioEvent;
	[SerializeField] private MouseCursor cursor;
	[SerializeField] private Transform spawnPoint;
	[SerializeField] private GameEventUI weaponSlotEvents;
	[SerializeField] private WeaponParameters parameters = null;
	[SerializeField] private WeaponType type = WeaponType.Launcher;

    //var for animations
    public bool hasShotMissile;
    WingedSpawnAnimator mechAnimScript;
    [HideInInspector] public GameObject shootingMech;

	public WeaponType Type
	{
		get { return type; }
	}

	private bool isActive = false;
	private bool isPlayerDriver = false;
	private float timeToNextShot = 0;
	private float realoadTimeLeft = 0;
	private int currentMagSize = 0;

	void Start( )
	{
        cursor = UIResourceManager.MouseCursor;

		Assert.IsNotNull( audioEvent );
		Assert.IsNotNull( parameters );
		Assert.IsNotNull( parameters.Projectile );
		Assert.IsNotNull( spawnPoint );
		Assert.IsNotNull( cursor );
		Assert.IsNotNull( weaponSlotEvents );

		currentMagSize = (int)parameters.MagSize;

        mechAnimScript = gameObject.GetComponentInParent<WingedSpawnAnimator>();//access MechAnimation script from current object
	}

	void Update( )
	{
		if ( !isActive )
			return;

		realoadTimeLeft -= Time.deltaTime;
		timeToNextShot -= Time.deltaTime;

		if ( isPlayerDriver && realoadTimeLeft >= 0 )
		{
			weaponSlotEvents.Raise( UIEvent.LauncherOn, 1f - ( realoadTimeLeft / parameters.RealoadTime ) );
		}
	}

	public void IsPlayerDriving( bool playerDriver )
	{
		isPlayerDriver = playerDriver;
	}

	public void SwapModel( string mechName ) { }

	public void Active( bool isActive )
	{
		this.isActive = isActive;

		if ( !isPlayerDriver )
			return;

		if ( isActive )
		{
			weaponSlotEvents.Raise( UIEvent.LauncherOn );
		}
		else
		{
			weaponSlotEvents.Raise( UIEvent.LauncherOff );
		}
	}

	public void SetCursor( MouseCursor cursor )
	{
		this.cursor = cursor;
	}

	public GameObject GetGameObject( )
	{
		return gameObject;
	}

	public void TryToFire( )
	{
		if ( spawnPoint == null ) return;

		if ( realoadTimeLeft > 0 || timeToNextShot > 0 )
			return;

		audioEvent.Raise( AudioEvents.RocketLaunch, transform.position );
        GameObject missile = Instantiate( parameters.Projectile, spawnPoint.position, Quaternion.Euler( 0, 0, 90 + Random.Range( -15f, 15f ) ) );
        hasShotMissile = true;
        if ( mechAnimScript != null) mechAnimScript.positionLocked = true;

        //Debug.Log("Has shot missile Launcher CS " + hasShotMissile);
        missile.GetComponent<HomingMissile>( ).SetDamage( parameters.GetDamage( ) );

        //let homingMissile script know mech that shot. Used so self-circle collider of wing spawn is ignored for collision
        //currently console returns error because the below hierarchy fits the winged spawn only. TODO
        //Transform shootingMech = transform.parent.parent.parent.parent.parent.parent.parent.parent.parent.parent.parent.parent;
        //Debug.Log("Mech Shooting" + shootingMech);
        //missile.GetComponent<HomingMissile>().ReceiveMechName(shootingMech);

        //find parent mech object that shot and pass it to homing Missile so missile ignores hitting that mech at first
        shootingMech = FindParentWithTag(transform, "Mech");
        Debug.Log("Name of Shooting Mech IS " + shootingMech.name);
        missile.GetComponent<HomingMissile>().ReceiveMechName(shootingMech);

        // TODO: suggest that we compare TAGS, not which gamepad this player is controlled by
        missile.GetComponent<HomingMissile>().playerNumber = transform.GetComponentInParent<Mech>().driver.gamepadNumber; // FIXME

		cursor.AddMissile( missile );
		missile.transform.SetParent( LitterContainer.instanceTransform );

		timeToNextShot = parameters.DelayBetweenShots;
		currentMagSize--;

		if ( isPlayerDriver )
		{
			weaponSlotEvents.Raise( UIEvent.LauncherOn, currentMagSize / parameters.MagSize );
		}

		if ( currentMagSize <= 0 )
		{
			currentMagSize = (int)parameters.MagSize;
			realoadTimeLeft = parameters.RealoadTime;
		}
    }

	public void SetDir( bool isRight ) { }
    //find Mech parent name that shot
    public static GameObject FindParentWithTag(Transform childTransform, string tag)
    {
        Transform t = childTransform;
        //Debug.Log("Tag name " + tag);
        //Debug.Log("ChildTransform Name " + t.name);
        while (t.parent != null)
        {
            //Debug.Log("Parent of child Name " + t.parent.name + "Has tag " + t.parent.tag);
            if (t.parent.tag == tag)
            {
                return t.parent.gameObject;
            }
            t = t.parent.transform;
        }
        return null; // Could not find a parent with given tag.
    }


}