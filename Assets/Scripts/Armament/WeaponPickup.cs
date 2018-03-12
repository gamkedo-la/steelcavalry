using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class WeaponPickup : MonoBehaviour
{
	[SerializeField] private GameObject[] objectsToPickUp = null;
	[SerializeField] private WeaponType typeToGive = WeaponType.Any;

	private List<IWeapon> weapons = new List<IWeapon>();

	void Start ()
	{
		Assert.IsNotNull( objectsToPickUp );
		Assert.AreNotEqual( objectsToPickUp.Length, 0 );

		foreach ( var item in objectsToPickUp )
		{
			IWeapon w = item.GetComponent<IWeapon>( );

			if (w != null)
			{
				weapons.Add( w );
			}
		}
	}

	void OnTriggerEnter2D( Collider2D collision )
	{
		if ( !collision.gameObject.CompareTag( "Mech" ) ) return;

		GiveOutWeapon( collision );
		Destroy( gameObject );
	}

	private void GiveOutWeapon( Collider2D collision )
	{
		WeaponManager manager = collision.gameObject.GetComponent<WeaponManager>( );
		if (manager == null)
		{
			Debug.LogWarning( "No WeaponManager on " + collision.gameObject.name );
			return;
		}

		IWeapon weapon = CreateWeapon( );
		manager.GiveWeapon( weapon );
	}

	private IWeapon CreateWeapon( )
	{
		IWeapon weapon = null;
		IWeapon[] ws;

		switch ( typeToGive )
		{
			case WeaponType.Turret:
				ws = weapons.Select( w => w ).Where( w => w.Type == WeaponType.Turret ).ToArray();
				weapon = ws[Random.Range( 0, ws.Length )];
                Debug.Log("Weapon " + weapon);
			break;

			case WeaponType.Launcher:
				ws = weapons.Select( w => w ).Where( w => w.Type == WeaponType.Launcher ).ToArray( );
				weapon = ws[Random.Range( 0, ws.Length )];
			break;

			case WeaponType.Thrower:
				ws = weapons.Select( w => w ).Where( w => w.Type == WeaponType.Thrower ).ToArray( );
				weapon = ws[Random.Range( 0, ws.Length )];
			break;

			case WeaponType.Any:
				weapon = weapons[Random.Range( 0, weapons.Count )];
			break;
		}

		return weapon;
	}
}
