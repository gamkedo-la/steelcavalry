﻿using System.Collections;
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

    public bool reverseDir = false; //made because the drone had a flipped z axis, messed up targeting

    private BackAndForth backAndForth; //kinda hacky, but I need to disable this when needed :/ 

    private List<string> targetableTags = new List<string>();

    void Start()
    {
        backAndForth = GetComponentInParent<BackAndForth>();
        originalAngles = transform.rotation.eulerAngles;
        targetableTags.Add("Player");
        targetableTags.Add("Mech");
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
            Vector3 targetDir = currentTarget.transform.position - transform.parent.position;
            
            float step = rotateSpeed * Time.deltaTime;
            if (reverseDir){
                targetDir = - targetDir;
            }
            Vector3 newDir = Vector3.RotateTowards(transform.parent.forward, targetDir, step, 1F);
            transform.parent.rotation = Quaternion.LookRotation(newDir);

        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (targetableTags.Contains(other.gameObject.tag))
        {
            // Debug.Log("Found the player!!!!");
            AI aiPilot = other.gameObject.GetComponent<AI>();
            if (aiPilot) return;
            currentTarget = other.gameObject; //locked on the player

            backAndForth.enabled = false;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == currentTarget)
        {
            // Debug.Log("I lost'em....");
            currentTarget = null;
            backAndForth.enabled = true;
        }
    }
}
