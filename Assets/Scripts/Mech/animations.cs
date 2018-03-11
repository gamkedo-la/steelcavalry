using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animations : MonoBehaviour
{

    Animator anim;
    public Player Player;
    Mech mechScript;
    MissileLauncher missileLauncher;
    WeaponManager weaponMgr;

    private bool inputUp;
    private bool inputDown;
    private bool inputRight;
    private bool inputLeft;
    private bool mechInUse;
    private float walk = 0.0f;
    private float walkSpeed = 1.0f;
    //missile vars
    public bool missileShot;
    private bool launcherOn = false;


    //anim IDs
    int onGroundHash = Animator.StringToHash("onGround");
    int mechInUseHash = Animator.StringToHash("mechInUse");
    int missileShotHash = Animator.StringToHash("missileShotStraight1");

    //on fly rotation
    private float smooth = 2.0f;
    private float tiltAngle = 30.0f;
    float GroundDist;

    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
        mechScript = GetComponent<Mech>();
        weaponMgr = GetComponent<WeaponManager>();

        /*inputFire = Input.GetMouseButton(0);
        inputAltFire = Input.GetMouseButton(1);
        inputAltFire2 = Input.GetKeyDown(KeyCode.Q);
        inputEnter = Input.GetKeyDown(KeyCode.Space);*/
    }

    /*private void FixedUpdate()
    {
        RaycastHit hit;
        Vector2 rayDown = transform.TransformDirection(Vector2.down);
        if (Physics.Raycast(transform.position, rayDown, out hit))
        {
            float GroundDist = hit.distance;//dist to ground
            Debug.Log("GrounDist " + GroundDist);
            //get rotation
            Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

            if (mechImIn && (inputLeft || inputRight) && (inputUp || inputDown))
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, smooth * Time.deltaTime);
                Debug.Log(targetRotation + " " + transform.rotation);
            }
        }
    }*/

    void Update()
    {
        inputUp = Player.inputUp;
        inputDown = Player.inputDown;
        inputRight = Player.inputRight;
        inputLeft = Player.inputLeft;
        mechInUse = mechScript.inUse;

        //Debug.Log("Mech in use " + mechInUse);
        
        //raycast dist to ground
        float minDistToGround = 0.25f;//dist for standby animation to occur
        Vector2 rayDown = transform.TransformDirection(Vector2.down);
        //circle collider may required this
        //Vector2 shift = new Vector2(0, 0.0f);
        //Vector2 shiftedPosition = transform.position;
        //shiftedPosition+=shift;

        //start += transform.position + Vector2(0, -1);
        RaycastHit2D groundHit = Physics2D.Raycast(transform.position, rayDown);
        
        if (groundHit)
        {
            if(groundHit.collider)
            {
                GroundDist = groundHit.distance;//dist to ground
                //Debug.Log("collider hit " + groundHit.collider.gameObject.name);
                //Debug.Log("Distance to Ground " + GroundDist);
                Debug.DrawRay(transform.position, rayDown, Color.red);
            }
        }
        
        if (GroundDist<= minDistToGround)
            anim.SetBool(onGroundHash, true);
        else
            anim.SetBool(onGroundHash, false);

        if (mechInUse)
        {
            anim.SetBool(mechInUseHash, true);
        }

        if (GroundDist <= minDistToGround && mechInUse && !inputUp && !inputDown && (inputLeft || inputRight))//mechScript.inUse
        {
            walk = walkSpeed;
            //Debug.Log("inputLeft " + inputLeft + " inputUp" + inputUp + " inputDown" + inputDown);
            anim.SetFloat("walk", walk);
        }
        else
        {
            walk = 0.0f;
            anim.SetFloat("walk", walk);
        }

        if (weaponMgr.launcherMounted && launcherOn==false)
        {
            missileLauncher = GetComponentInChildren<MissileLauncher>();
        }

        Debug.Log("launcher is On " + launcherOn);
        Debug.Log("Missile Shot is on " + missileShot);

        if (weaponMgr.launcherMounted)
        {
            //Debug.Log("I'm here now");
            missileShot = missileLauncher.hasShotMissile;
            //Debug.Log("Missile was shot " + missileShot);
            if (missileShot)
            {
                anim.SetBool(missileShotHash, true);
                Debug.Log("Animation shooting Missile");
                Debug.Log("Anim missileShot bool: " + anim.GetBool("missileShot") + "normalized time " + anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
                if(anim.GetBool("missileShot") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime>=1)
                {
                    Debug.Log("Animation has finished playing");
                    missileLauncher.hasShotMissile = false;
                }    
            }                
        }
    }
}