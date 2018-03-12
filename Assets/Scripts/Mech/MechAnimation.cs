using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechAnimation : MonoBehaviour
{
    public Player Player;
    Mech mechScript;
    MissileLauncher missileLauncher;
    WeaponManager weaponMgr;

    private bool inputUp;
    private bool inputDown;
    private bool inputRight;
    private bool inputLeft;
    private bool inputFire;
    private bool mechInUse;
    private float walk = 0.0f;
    private float walkSpeed = 1.0f;
    //missile vars
    Animator anim;
    private bool missileShot;
    private bool missileShotStraight1;
    private bool launcherOn = false;
    //direction to shoot missile
    private Transform missileLauncherChild;
    
    //anim IDs
    //animator parameters
    int onGroundHash = Animator.StringToHash("onGround");
    int mechInUseHash = Animator.StringToHash("mechInUse");
    int missileShotHash = Animator.StringToHash("missileShot");
    int missileShotStraight1Hash = Animator.StringToHash("missileShotStraight1");
    //animation IDs
    int missileShotStraight1Tag;

    //on fly rotation
    private float smooth = 2.0f;
    private float tiltAngle = 30.0f;
    float GroundDist;

    // Use this for initialization
    private void Awake()
    {
        anim = GetComponent<Animator>();
        mechScript = GetComponent<Mech>();
        weaponMgr = GetComponent<WeaponManager>();
        //animation IDs
        missileShotStraight1Tag = Animator.StringToHash("shotStraight1");
    }
    void Start()
    {
        

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
        inputFire = Input.GetMouseButton(0);
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
            anim.speed = 1;
            anim.SetBool(mechInUseHash, true);
        }
        else 
        {
            anim.speed = 0;
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
            launcherOn = true;
        }

        //Debug.Log("launcher is On " + launcherOn);
        //Debug.Log("Missile Shot is on " + missileShot);

        //Debug.Log("Launcher Mounted NOW " + weaponMgr.launcherMounted);

        if (weaponMgr.launcherMounted)
        {
            //Debug.Log("I'm here now");
            missileShot = missileLauncher.hasShotMissile;
            Debug.Log("MissileShot Flag is " + missileShot);
            
            if (missileShot)
            {
                int animPlayingTag = anim.GetCurrentAnimatorStateInfo(0).tagHash;
                //Debug.Log("AnimPlaying " + animPlaying);
                //missileShotStraight1 = true;//assume for now
                anim.SetBool(missileShotHash, true);
                anim.SetBool(missileShotStraight1Hash, true);
                
                
                
                //Debug.Log("Anim missileShot bool: " + anim.GetBool("missileShot") + " anim Playing Tag " + animPlayingTag+ "missileShotStraight1Tag " + missileShotStraight1Tag);
                if(anim.GetBool(missileShotHash) && animPlayingTag== missileShotStraight1Tag && anim.GetCurrentAnimatorStateInfo(0).normalizedTime>=1)
                {
                    missileLauncher.hasShotMissile = false;
                    anim.SetBool(missileShotHash, false);
                    missileShotStraight1 = false;
                    anim.SetBool(missileShotStraight1Hash, false);
                    
                    Debug.Log("Animation has finished playing, setting flag to " + missileLauncher.hasShotMissile);
                }    
            }                
        }
    }

    private void lasserAnimDirection()
    {
        missileLauncherChild = transform.Find("Launcher Mount Point");
        if (Utilities.GetMouseWorldPosition(Input.mousePosition).y < missileLauncherChild.transform.position.y && mechScript.isFacingRight)
        {

        }
        
    }

}