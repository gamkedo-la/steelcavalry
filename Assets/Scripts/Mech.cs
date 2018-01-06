using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mech : MonoBehaviour {
	public float mechSpeed = 2.0f;
 	public float jumpPower = 10.0f;
	public bool isOnGround;
	private Rigidbody2D mechRB;
	public Transform model;

	public Transform mainProjectileSource = null;
	public GameObject mainProjectilePrefab = null;

	// Use this for initialization
	void Start () {
		mechRB = GetComponent<Rigidbody2D>();
		
	}
	
	// Update is called once per frame
	public void MechUpdate () {
		transform.position += Vector3.right * Input.GetAxisRaw("Horizontal") * Time.deltaTime * mechSpeed;

		if(Input.GetAxisRaw("Vertical") > 0.0f && isOnGround) {
			mechRB.AddForce(Vector2.up * Input.GetAxisRaw("Vertical") * jumpPower);
			isOnGround = false;
		}

		if(mainProjectilePrefab != null) {
			if(Input.GetMouseButton(0)) {
				if(mainProjectileSource == null) {
					Debug.LogError("missing mainProjectileSource but have mainProjectilePrefab");
					return;
				}
				GameObject shotGO = GameObject.Instantiate(mainProjectilePrefab, mainProjectileSource.position, Quaternion.identity);
				Vector2 pos2D = new Vector2(mainProjectileSource.position.x,mainProjectileSource.position.y);
				Vector2 aimAt = Camera.main.ScreenToWorldPoint( new Vector2(Input.mousePosition.x, Input.mousePosition.y) );
				Rigidbody2D shotRB = shotGO.GetComponent<Rigidbody2D>();
				Vector2 movementDirection = (aimAt - pos2D).normalized;
				movementDirection += Random.insideUnitCircle * 0.1f; // randomize
				shotRB.velocity = movementDirection * 20.0f;
				shotGO.transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(shotRB.velocity.y,shotRB.velocity.x) * Mathf.Rad2Deg,
					Vector3.forward);
				//shotGO.transform.RotateAround(transform.up, 90.0f);
			}
		}
	}

	void OnCollisionEnter2D(Collision2D bumpFacts) {
		for(int i = 0; i < bumpFacts.contacts.Length; i++) {
			if(bumpFacts.contacts[i].normal.y >= 0.9f) {
				isOnGround = true;
				return;
			}
		}
	}
}
