using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotBreaksIntoParticle : MonoBehaviour {
	public GameObject pfx;
	private GameObject player;
	private string nameOfMechPlayerIsIn;
	private string nameOfObjectHit;

	void Start() {
		player = GameObject.FindWithTag("Player");
	}

	void OnCollisionEnter2D(Collision2D bumpFacts) {
		/*Debug.Log("Shot hit: " + bumpFacts.collider.gameObject.name +
		"Reminder: using Physics2D Layer ignore shenanigans for demo");*/

		nameOfMechPlayerIsIn = player.GetComponent<PlayerMovement>().getNameOfMechPlayerIsIn();
		nameOfObjectHit = bumpFacts.collider.gameObject.name;

		// If the shot is from the player, ignore it
		if(nameOfMechPlayerIsIn == nameOfObjectHit) return;

		GameObject pfxGO = GameObject.Instantiate(pfx, transform.position, transform.rotation);
		pfxGO.transform.SetParent(LitterContainer.instanceTransform);
		Destroy(gameObject);
	}
}
