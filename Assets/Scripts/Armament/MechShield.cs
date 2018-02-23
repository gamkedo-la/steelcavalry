using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechShield : MonoBehaviour {
	public GameObject pfx;

	void OnDisable()
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
		if (other.CompareTag("PlayerProjectile") && transform.parent.parent.GetComponent<Mech>().driver.GetComponent<Player>().playerNumber
			!= other.gameObject.GetComponent<ShotBreaksIntoParticle>().playerNumber)
        {
			GameObject pfxGO = Instantiate(pfx, other.transform.position, transform.rotation);
			pfxGO.transform.SetParent(LitterContainer.instanceTransform);
			Destroy(other.gameObject);
        }
		else if (other.CompareTag("PlayerMissile") && transform.parent.parent.GetComponent<Mech>().driver.GetComponent<Player>().playerNumber
			!= other.gameObject.GetComponent<HomingMissile>().playerNumber)
		{
			other.GetComponent<HomingMissile>().DoDestruction(other.transform.position);
			Destroy(other.gameObject);
		}
    }
}
