using System.Collections.Generic;
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

		GameObject weapon = CreateWeapon( );
		manager.GiveWeapon( weapon );
	}

	private GameObject CreateWeapon( )
	{
		GameObject weapon = null;

		return weapon;
	}
}
