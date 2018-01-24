using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotLauncher : MonoBehaviour {

	public GameObject shot;

	public float fireRate; //Time between each fires
    private float nextFire = 0f; //Time until next shot is available

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public bool CheckIfReady(){
		return nextFire < Time.time;
	}

	public void Fire(Vector2 targetPosition){
		nextFire = Time.time + fireRate; //Update the timers for shots

		if (shot == null) return;

		GameObject shotGO = GameObject.Instantiate(shot, transform.position, Quaternion.identity);
		Vector2 pos2D = new Vector2(transform.position.x, transform.position.y);
		Vector2 aimAt = new Vector2 (targetPosition.x, targetPosition.y);
		Rigidbody2D shotRB = shotGO.GetComponent<Rigidbody2D>();
		Vector2 movementDirection = (aimAt - pos2D).normalized;
		movementDirection += Random.insideUnitCircle * 0.1f; // randomize
		shotRB.velocity = movementDirection * 20.0f;
		shotGO.transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(shotRB.velocity.y, shotRB.velocity.x) * Mathf.Rad2Deg,
			Vector3.forward);
		shotGO.transform.SetParent(LitterContainer.instanceTransform);
	}
}
