using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animations : MonoBehaviour {

    Animator anim;
    public Player Player;
    Mech mechScript;

    private bool inputUp;
    private bool inputDown;
    private bool inputRight;
    private bool inputLeft;
	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
        mechScript = GetComponent<Mech>();
        
        /*inputFire = Input.GetMouseButton(0);
        inputAltFire = Input.GetMouseButton(1);
        inputAltFire2 = Input.GetKeyDown(KeyCode.Q);
        inputEnter = Input.GetKeyDown(KeyCode.Space);*/
    }
	
	// Update is called once per frame
	void Update () {
        //inputUp = Player.inputUp;
        //inputDown = Player.inputDown;
        inputRight = Player.inputRight;
        inputLeft = Player.inputLeft;

        if (inputRight && mechScript.inUse);
            anim.SetBool("walk", inputRight);

	}
}
