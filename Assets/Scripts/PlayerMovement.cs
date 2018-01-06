using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
	public float humanSpeed = 0.8f;
	public float mechNearEnoughToUseDistance = 1.0f;
	public float exitMechDistancePopUp = 1.1f;
	public float jetPackPower = 1.0f;

	private Mech mechImIn = null;
	private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

	private int mechOnlyMask;
	private int groundOnlyMask;

    public bool isFacingRight = true;

	// Use this for initialization
	void Start () {
		mechOnlyMask = LayerMask.GetMask("Mech");
		groundOnlyMask = LayerMask.GetMask("Ground");
		rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

	void EnteringOrLeavingMech() {
		bool notInMech = (mechImIn == null);
		GetComponent<BoxCollider2D>().enabled = notInMech;
		spriteRenderer.enabled = notInMech;
		rb.velocity = Vector2.zero;
	}

	// Update is called once per frame
	void Update () {
		if(mechImIn) {
			mechImIn.MechUpdate();
			transform.position = mechImIn.transform.position;
		} else {
			transform.position += Vector3.right * Input.GetAxisRaw("Horizontal") * Time.deltaTime * humanSpeed;

            if (Input.GetAxisRaw("Horizontal") > 0.0f && !isFacingRight) {
                isFacingRight = true;
                spriteRenderer.flipX = false;
            } else if(Input.GetAxisRaw("Horizontal") < 0.0f && isFacingRight) {
                isFacingRight = false;
                spriteRenderer.flipX = true;
            }

            if (Input.GetAxisRaw("Vertical") > 0.0f) {
				transform.position += Vector3.up * Time.deltaTime * jetPackPower;
				rb.gravityScale = 0.0f;
				rb.velocity = Vector2.zero;
			} else {
				rb.gravityScale = 1.0f;
			}
		}

		if(Input.GetKeyDown(KeyCode.Space)) {

			if(mechImIn) {
				mechImIn = null;
				transform.position += Vector3.up * exitMechDistancePopUp;
				EnteringOrLeavingMech();
			} else { // entering mech
				Collider2D[] nearbyMechs = Physics2D.OverlapCircleAll(transform.position,
													mechNearEnoughToUseDistance,
													mechOnlyMask);
				float nearestMechDist = 9000.0f;
				Collider2D nearestMechCollider = null;
				for(int i = 0; i < nearbyMechs.Length; i++) {
					float distToMech = Vector2.Distance(transform.position,
						                   nearbyMechs[i].transform.position);
					if(distToMech < nearestMechDist) {
						nearestMechDist = distToMech;
						nearestMechCollider = nearbyMechs[i];
					}
				}
				if(nearestMechCollider) {
					Mech mScript = nearestMechCollider.GetComponent<Mech>();
					if(mScript) {
						mechImIn = mScript;
					}
					EnteringOrLeavingMech();
				} // if nearestMechCollider
			} // else (entering mech)
		} // input to get in mech
	} // playerMovement function


} // class
