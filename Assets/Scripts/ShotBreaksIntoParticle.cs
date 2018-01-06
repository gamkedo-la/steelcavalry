using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotBreaksIntoParticle : MonoBehaviour {
	public GameObject pfx;

	void OnCollisionEnter2D(Collision2D bumpFacts) {
		Debug.Log("Shot hit: " + bumpFacts.collider.gameObject.name +
		"Reminder: using Physics2D Layer ignore shenanigans for demo");
		GameObject.Instantiate(pfx, transform.position, transform.rotation);
		Destroy(gameObject);
	}

}
