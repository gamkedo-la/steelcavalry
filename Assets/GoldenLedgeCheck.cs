using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldenLedgeCheck : MonoBehaviour {

    public LedgeChecker ledgeLeft;
    public LedgeChecker ledgeRight;
    public bool isGroundLeft = false;
    public bool isGroundRight = false;

    
	void FixedUpdate () {
        isGroundLeft = ledgeLeft.isGround;
        isGroundRight = ledgeRight.isGround;
	}
}
