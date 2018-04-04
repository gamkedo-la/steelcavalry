using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlopeWalker : MonoBehaviour {
    private Rigidbody2D walkerBody;
    public GameObject walkerFeet;
    public float rayLength = 2.0f;
    public bool isFacingRight = false;
    public bool isMovingUp = false;
    public string slopeTag = "Ground";
    public float maxClimbableSlopeAngle = 85f;

    private bool isOnSlope = false;
    private Vector2 velocityOnSlope = Vector2.zero;

    public float GetCollidedSlopeAngle (Vector2 velocity, GameObject feet, float rayLength, bool isFacingRight, string tag, float maxClimbableAngle) {
        float angle = 0;

        int feetRaycastDepth = 5;        
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

    void Start () {
        walkerBody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate () {        
        GetCollidedSlopeAngle(walkerBody.velocity, walkerFeet, rayLength, isFacingRight, slopeTag, maxClimbableSlopeAngle);
        if (isOnSlope && !isMovingUp) {
            walkerBody.velocity = velocityOnSlope;
        }
    }
}
