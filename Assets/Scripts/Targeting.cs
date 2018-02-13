using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targeting : MonoBehaviour
{

    //private CircleCollider2D fov;
    public GameObject currentTarget;

    public bool willFollow;
    public float followSpeed;
    public float approachDist;

    private BackAndForth backAndForth; //kinda hacky, but I need to disable this when needed :/ 

    void Start()
    {
        backAndForth = GetComponentInParent<BackAndForth>();
    }

    void FixedUpdate()
    {
        if (currentTarget && willFollow)
        {

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
