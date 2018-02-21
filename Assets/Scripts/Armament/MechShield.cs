using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechShield : MonoBehaviour {
    void OnDisable()
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
		//Debug.Log("!!!!!!!!!!!!!!!!  " + other.gameObject.name + " " + other.gameObject.tag + " " + Time.time);
		if (other.CompareTag("PlayerProjectile"))
        {
			//TODO: Add some kind of effect
			Destroy(other.gameObject);
        }
		else if (other.CompareTag("PlayerMissile"))
		{
			//TODO: Add some kind of effect
			Destroy(other.gameObject);
		}
    }
}
