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
    int isTakingOffParam = Animator.StringToHash("isTakingOff");
    bool canIdle = false, canWalk = false, canFly = false, canTakeOff = false;

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
                  mech.inUse &&
                  !mech.driver.isAttacking ? 
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
                  mech.inUse && 
                  (mech.driver.inputLeft || mech.driver.inputRight);
        animController.SetBool(isWalkingParam, canWalk);

        if (mech.inUse) canTakeOff = mech.driver.inputUp;
        animController.SetBool(isTakingOffParam, canTakeOff);

        canFly = (mech.currentTimeToTakeOff >= mech.takingOffTime || canFly);
        animController.SetBool(isFlyingParam, canFly);

        if (mech.isOnGround) canFly = false;
    }

    IEnumerator IdleTracker () {        
        yield return new WaitForSeconds(idleWaitSeconds);        
        Debug.Log("Set mech animation to idle if mech is idle.");
        animController.SetBool(isIdlingParam, canIdle);
    }
}
