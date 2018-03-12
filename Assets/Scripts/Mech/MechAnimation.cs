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
    private bool launcherOn = false;
    //direction to shoot missile
    public Transform missileLauncherLocation;
    
    //anim IDs
    //animator parameters
    int onGroundHash = Animator.StringToHash("onGround");
    int mechInUseHash = Animator.StringToHash("mechInUse");
    int missileShotHash = Animator.StringToHash("missileShot");
    int missileShotStraight1Hash = Animator.StringToHash("missileShotStraight1");
    int missileShotStraight2Hash = Animator.StringToHash("missileShotStraight2");
    int missileShotUpHash = Animator.StringToHash("missileShotUp");
    int missileShotDownHash = Animator.StringToHash("missileShotDown");
    //animation IDs
    int missileShotStraight1Tag;
    int missileShotStraight2Tag;
    int missileShotUpTag;
    int missileShotDownTag;

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
        missileShotStraight2Tag = Animator.StringToHash("shotStraight2");
        missileShotUpTag = Animator.StringToHash("shotUp");
        missileShotDownTag = Animator.StringToHash("shotDown");
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
            //Debug.Log("MissileShot Flag is " + missileShot);
            
            if (missileShot)
            {
                int animPlayingTag = anim.GetCurrentAnimatorStateInfo(0).tagHash;
                anim.SetBool(missileShotHash, true);

                //decide anim to play
                lasserAnimDirection();

                Debug.Log("missileShot " + missileShot + " ////AnimPlayingTag " + animPlayingTag + "missileShotStraight1Tag " + missileShotStraight1Tag + " Straigh2 " + missileShotStraight2Tag  + " ShotUp " + missileShotUpTag + " ShotDown " + missileShotDownTag);
                if (anim.GetBool(missileShotHash) && anim.GetCurrentAnimatorStateInfo(0).normalizedTime>=1 && (animPlayingTag==missileShotStraight1Tag || animPlayingTag==missileShotStraight2Tag || animPlayingTag==missileShotUpTag || animPlayingTag==missileShotDownTag))
                {
                    if(animPlayingTag==missileShotStraight1Tag)
                    {
                        //missileShotStraight1 = false;
                        anim.SetBool(missileShotStraight1Hash, false);
                    }
                    if(animPlayingTag== missileShotStraight2Tag)
                    {
                        //missileShotStraight2 = false;
                        anim.SetBool(missileShotStraight2Hash, false);
                    }
                    if(animPlayingTag== missileShotUpTag)
                    {
                        anim.SetBool(missileShotUpHash, false);
                    }   
                    if(animPlayingTag== missileShotDownTag)
                    {
                        anim.SetBool(missileShotDownHash, false);
                    }   
                    
                    missileLauncher.hasShotMissile = false;
                    anim.SetBool(missileShotHash, false);
                    Debug.Log("Animation has finished playing, setting flag to " + missileLauncher.hasShotMissile);
                }    
            }                
        }
    }

    private void lasserAnimDirection()
    {
        if (Utilities.GetMouseWorldPosition(Input.mousePosition).y < missileLauncherLocation.transform.position.y)
        {
            //shoot down
            anim.SetBool(missileShotDownHash, true);
        }
        else if (Utilities.GetMouseWorldPosition(Input.mousePosition).y > missileLauncherLocation.transform.position.y)
        {
            //shoot up
            anim.SetBool(missileShotUpHash, true);
        }
        else
        {
            //shoot straight
            //need to set random straight1or2
            anim.SetBool(missileShotStraight1Hash, true);
            anim.SetBool(missileShotStraight2Hash, true);
        }

    }

}