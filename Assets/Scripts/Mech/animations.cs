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
    private bool mechImIn;
    private float walk = 0.0f;
    private float walkSpeed = 1.0f;
    private bool animMechImIn = false;

    int walkHash = Animator.StringToHash("walk");

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
        mechImIn = Player.mechImIn;

        if(mechImIn)
        {
            anim.SetBool("animMechImIn", true);
        }

        if (inputLeft || inputRight && mechImIn)//mechScript.inUse
        {
            walk = walkSpeed;
            //Debug.Log("inputRight " + inputRight + " imin" + mechImIn + " walk " + walk);
            anim.SetFloat("walk", walk);
        }
        else
        {
            walk = 0.0f;
            anim.SetFloat("walk", walk);
        }
            
            

	}
}
