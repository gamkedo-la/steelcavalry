using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class killbox : MonoBehaviour {

	//private GameEventFloat didDamageEvent = null;

	private void OnCollisionEnter2D(Collision2D collision)
	{

		// Try to find a Mech script on the hit object
		HP hp = collision.collider.GetComponent<HP>( );
		if ( hp )
		{
			Debug.Log (collision.collider.gameObject.name + " hit a killbox!");
			//didDamageEvent.Raise( 1f );
			hp.TakeDamage( 9999999 );
		}

	}

}
