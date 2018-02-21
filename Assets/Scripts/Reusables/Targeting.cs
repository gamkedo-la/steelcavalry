using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targeting : MonoBehaviour
{

    //private CircleCollider2D fov;
    public GameObject currentTarget;

    public bool willFollow;
    public bool willRotate;
    public float followSpeed;
    public float rotateSpeed;
    public float approachDist;

    private Vector3 originalAngles;
    private float minAngle = -10;
	private float maxAngle = 10;


    private BackAndForth backAndForth; //kinda hacky, but I need to disable this when needed :/ 

    void Start()
    {
        backAndForth = GetComponentInParent<BackAndForth>();
        originalAngles = transform.rotation.eulerAngles;
    }

    void FixedUpdate()
    {
        if (currentTarget && willFollow)
        {

            //For approaching target
            Vector3 endPos;
            Vector3 dir = transform.position - currentTarget.transform.position;
            if (dir.magnitude > approachDist)
            {
                endPos = currentTarget.transform.position + dir.normalized * approachDist;
            }
            else
            {
                endPos = transform.position;
            }

            transform.parent.position = Vector3.MoveTowards(transform.position, endPos, followSpeed * Time.deltaTime);

            //For rotating towards target
            Vector3 targetDir = currentTarget.transform.position - transform.position;
            float step = rotateSpeed * Time.deltaTime;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 1.0F);
            Debug.DrawRay(transform.position, -newDir, Color.red);
            transform.parent.rotation = Quaternion.LookRotation(newDir);

            /*bool targetIsBehindMech = (
                !(endPos.x < transform.position.x && !mech.isFacingRight) &&
                !(endPos.x > transform.position.x && mech.isFacingRight)
            );
            if (targetIsBehindMech) return;

            float zCorrection = 0;
            float yAngle = 90f;
            float xAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            if (isFacingRight) {
                xAngle = Mathf.Clamp(xAngle, minAngle, maxAngle);
            } else {
                if (xAngle > 0) {
                    xAngle = Mathf.Clamp(xAngle, 180f + minAngle, 180f);
                } else {
                    xAngle = Mathf.Clamp(xAngle, -180f, -180f + maxAngle);
                }

                zCorrection = 180;
                xAngle += 180;
            }

            transform.rotation = Quaternion.Euler(
                xAngle + originalAngles.x,
                yAngle + originalAngles.y,
                originalAngles.z + zCorrection
            );*/
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Found the player!!!!");
            currentTarget = other.gameObject; //locked on the player

            backAndForth.enabled = false;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == currentTarget)
        {
            Debug.Log("I lost'em....");
            currentTarget = null;
            backAndForth.enabled = true;
        }
    }
}
