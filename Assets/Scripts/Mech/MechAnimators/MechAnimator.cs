using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechAnimator : MonoBehaviour {
    public float idleWaitSeconds = 5f;
    public float belowThisSpeedIsIdle = 0.2f;

    Mech mech;

    Animator animController;
    int isIdlingParam = Animator.StringToHash("isIdling");
    int isWalkingParam = Animator.StringToHash("isWalking");
    int isFlyingParam = Animator.StringToHash("isFlying");
    bool canIdle = true, canWalk = false, canFly = false;

	// Use this for initialization
	void Start () {
        mech = GetComponent<Mech>();
        animController = GetComponent<Animator>();
        animController.SetBool(isIdlingParam, true);
    }
	
	// Update is called once per frame
	void Update () {        
        canIdle = mech.isOnGround && 
                  Mathf.Abs(mech.mechRigidbody.velocity.x) < belowThisSpeedIsIdle && 
                  mech.driver == null ? 
                    true : false;

        if (canIdle) {
            StartCoroutine("IdleTracker");
        }
        else {
            StopCoroutine("IdleTracker");            
            animController.SetBool(isIdlingParam, canIdle);            
        }

        canWalk = mech.isOnGround && 
                  Mathf.Abs(mech.mechRigidbody.velocity.x) > belowThisSpeedIsIdle && 
                  mech.driver != null && 
                  (mech.driver.inputLeft || mech.driver.inputRight);
        animController.SetBool(isWalkingParam, canWalk);

        canFly = !mech.isOnGround;
        animController.SetBool(isFlyingParam, canFly);
	}

    IEnumerator IdleTracker () {        
        yield return new WaitForSeconds(idleWaitSeconds);        
        Debug.Log("Set mech animation to idle if mech is idle.");
        animController.SetBool(isIdlingParam, canIdle);
    }
}
