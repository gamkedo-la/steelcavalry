using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlopeWalker : MonoBehaviour {
    public GameObject walkerFeet;
    public bool isFacingRight = false;
    public string slopeTag = "Ground";
    public float maxClimbableSlopeAngle = 85f;

    private bool isOnSlope = false;
    private Vector2 velocityOnSlope = Vector2.zero;
    private Rigidbody2D walkerBody;

    public float GetCollidedSlopeAngle (GameObject feet, Vector2 velocity, bool isFacingRight, string tag, float maxClimbableAngle) {
        float angle = 0;

        int feetRaycastDepth = 5;
        float rayLength = 2.0f;
        RaycastHit2D[] raycastHits = new RaycastHit2D[feetRaycastDepth];
        Vector2 raycastDirection = isFacingRight ? Vector2.right : Vector2.left;
        int rayCount = Physics2D.RaycastNonAlloc(feet.transform.position, raycastDirection, raycastHits, rayLength);

        for (int i = 0; i < rayCount; i++) {
            if (raycastHits[i].collider.tag == tag) {
                angle = Vector2.Angle(raycastHits[i].normal, Vector2.up);
                break;
            }
        }

        float moveAmount = Mathf.Abs(velocity.x);
        float slopeAngle = angle * Mathf.Deg2Rad;

        if (angle > 0 && angle < maxClimbableAngle) {
            isOnSlope = true;            
            Vector2 slopeVector = new Vector2(moveAmount * Mathf.Cos(slopeAngle) * Mathf.Sign(velocity.x), moveAmount * Mathf.Sin(slopeAngle));
            velocityOnSlope = slopeVector;
        }
        else {
            isOnSlope = false;
        }

        return angle;
    }

    public void Start () {
        walkerBody = GetComponent<Rigidbody2D>();
    }

    public void LateUpdate () {
        GetCollidedSlopeAngle(walkerFeet, walkerBody.velocity, isFacingRight, slopeTag, maxClimbableSlopeAngle);

        if (isOnSlope) {
            walkerBody.velocity = velocityOnSlope;
        }
    }
}
