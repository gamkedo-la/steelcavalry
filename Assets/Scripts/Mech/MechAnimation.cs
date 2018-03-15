﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechAnimation : MonoBehaviour
{

    float mechDist;//TODO: revise why can't delcare in swing function


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
    public ParticleSystem missileFireParticles;
    //direction to shoot missile
    public Transform missileLauncherLocation;
    private float missleLauncherLocationY;
    [HideInInspector] public bool positionLocked=false;//lock mouse and missileLauncher position to determine direction of anim
    LayerMask enemyLayer;//masking for sword swing raycast

    //anim IDs
    //animator parameters
    int onGroundHash = Animator.StringToHash("onGround");
    int mechInUseHash = Animator.StringToHash("mechInUse");
    int missileShotHash = Animator.StringToHash("missileShot");
    int missileShotStraight1Hash = Animator.StringToHash("missileShotStraight1");
    int missileShotStraight2Hash = Animator.StringToHash("missileShotStraight2");
    int missileShotUpHash = Animator.StringToHash("missileShotUp");
    int missileShotDownHash = Animator.StringToHash("missileShotDown");
    int swingAtReachHash = Animator.StringToHash("swingAtReach");

    //animation IDs
    int missileShotStraight1Tag;
    int missileShotStraight2Tag;
    int missileShotUpTag;
    int missileShotDownTag;
    int swordSwingTag;

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
        //missileFireParticles = GetComponent<ParticleSystem>();
        //disable particles
        //var missileEmission = missileFireParticles.emission;
        //missileEmission.enabled = false;

        //animation IDs
        missileShotStraight1Tag = Animator.StringToHash("shotStraight1");
        missileShotStraight2Tag = Animator.StringToHash("shotStraight2");
        missileShotUpTag = Animator.StringToHash("shootUp");
        missileShotDownTag = Animator.StringToHash("shootDown");
        swordSwingTag = Animator.StringToHash("swordSwing");

        //Debug.Log("OnWatch2 ID " + Animator.StringToHash("onWatch2"));
        Debug.Log("missileShotStraight1Tag " + missileShotStraight1Tag + " Straigh2 " + missileShotStraight2Tag + " ShotUp " + missileShotUpTag + " ShotDown " + missileShotDownTag);

        //swordSwing raycast
        enemyLayer = LayerMask.GetMask("Mech");
    }
    void Start()
    {
        /*inputFire = Input.GetMouseButton(0);
        inputAltFire = Input.GetMouseButton(1);
        inputAltFire2 = Input.GetKeyDown(KeyCode.Q);
        inputEnter = Input.GetKeyDown(KeyCode.Space);*/
    }
    private void FixedUpdate()
    {
        //raycast dist to ground
        float minDistToGround = 0.25f;//dist for standby animation to occur
        Vector2 rayDown = transform.TransformDirection(Vector2.down);

        RaycastHit2D groundHit = Physics2D.Raycast(transform.position, rayDown);

        if (groundHit)
        {
            if (groundHit.collider)
            {
                GroundDist = groundHit.distance;//dist to ground
                Debug.DrawRay(transform.position, rayDown, Color.red);
            }
        }

        if (GroundDist > minDistToGround)
        {
            /*Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, groundHit.normal);
            if ((inputLeft || inputRight) && (inputUp || inputDown))//need to add (mechImIn &&)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, smooth * Time.deltaTime);
            }*/
            float tiltDegrees = 0.0f;
            if(inputLeft)
            {
                tiltDegrees = 45.0f;
            }
            if(inputRight)
            {
                tiltDegrees = -45.0f;
            }
            Quaternion targetRotation = Quaternion.AngleAxis(tiltDegrees, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.05f);
        }
    }

    void Update()
    {
        inputUp = Player.inputUp;
        inputDown = Player.inputDown;
        inputRight = Player.inputRight;
        inputLeft = Player.inputLeft;
        inputFire = Input.GetMouseButton(0);
        mechInUse = mechScript.inUse;

        int animPlayingTag;

        //Debug.Log("Mech in use " + mechInUse);

        //Mech fly animation: raycast dist to ground
        float minDistToGround = 0.25f;//dist for standby animation to occur
        Vector2 rayDown = transform.TransformDirection(Vector2.down);
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

        //swing Sword Animation
        swordSwing();
        animPlayingTag = anim.GetCurrentAnimatorStateInfo(0).tagHash;
        Debug.Log("Anim playing " + animPlayingTag + "TagID_SwordSwing"  + swordSwingTag + "Current State Anim " + anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
        if (animPlayingTag==swordSwingTag && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            anim.SetBool(swingAtReachHash, false);
        }

        //Missile Animations
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
            //TODO: need to move this outside update
            //render off for missile launcher cos mech already has missile shooter
            GameObject missileLauncherGO = GameObject.FindGameObjectWithTag("missileLauncher");
            MeshRenderer missileLauncherRender = missileLauncherGO.GetComponent<MeshRenderer>();
            missileLauncherRender.enabled = false;

            missileShot = missileLauncher.hasShotMissile;
            //Debug.Log("MissileShot Flag is " + missileShot);
            
            if (missileShot)
            {
                
                animPlayingTag = anim.GetCurrentAnimatorStateInfo(0).tagHash;
                anim.SetBool(missileShotHash, true);

                //decide anim to play
                if(positionLocked)
                {
                    missleLauncherLocationY = missileLauncherLocation.transform.position.y;
                    Debug.Log("I'm inside positionLocked and the var is " + positionLocked);
                    lasserAnimDirection(missleLauncherLocationY);
                    //enable missile shot particles when shot
                    //var missileEmission = missileFireParticles.emission;
                    //missileEmission.enabled = true;
                }
                Debug.Log("Now PositionLocked is " + positionLocked);
                //Debug.Log("missileShot " + missileShot + " ////AnimPlayingTag " + animPlayingTag + "missileShotStraight1Tag " + missileShotStraight1Tag + " Straigh2 " + missileShotStraight2Tag  + " ShotUp " + missileShotUpTag + " ShotDown " + missileShotDownTag);
                //Debug.Log("Bool for Shooting Anim " + anim.GetBool(missileShotHash) + " time to end anim " + anim.GetCurrentAnimatorStateInfo(0).normalizedTime + " AnimPlayingTag " + animPlayingTag);
                Debug.Log("Current Anim playing before flag to False " + animPlayingTag);
                if (anim.GetBool(missileShotHash) && anim.GetCurrentAnimatorStateInfo(0).normalizedTime>=1)//(animPlayingTag==missileShotStraight1Tag || animPlayingTag==missileShotStraight2Tag || animPlayingTag==missileShotUpTag || animPlayingTag==missileShotDownTag)
                {
                    //Debug.Log("Has finished playing Anim");
                    Debug.Log("We flag to false now : Bool for Shooting Anim " + anim.GetBool(missileShotHash) + " time to end anim " + anim.GetCurrentAnimatorStateInfo(0).normalizedTime + " AnimPlayingTag " + animPlayingTag);
                    if (animPlayingTag == missileShotStraight1Tag)
                    {
                        //missileShotStraight1 = false;
                        anim.SetBool(missileShotStraight1Hash, false);
                    }
                    else if (animPlayingTag == missileShotStraight2Tag)
                    {
                        //missileShotStraight2 = false;
                        anim.SetBool(missileShotStraight2Hash, false);
                    }
                    else if (animPlayingTag == missileShotUpTag)
                    {
                        anim.SetBool(missileShotUpHash, false);
                    }
                    else if (animPlayingTag == missileShotDownTag)
                    {
                        anim.SetBool(missileShotDownHash, false);
                    }
                    else
                    {
                      //  Debug.Log("Unrecognized AnimPlayingTag" + " AnimPlayingTag " + animPlayingTag);
                    }
                    missileLauncher.hasShotMissile = false;
                    anim.SetBool(missileShotHash, false);
                    //disable particles missile launch
                    var missileEmission = missileFireParticles.emission;
                    missileEmission.enabled = false;
                    //Debug.Log("Animation has finished playing, setting flag to " + missileLauncher.hasShotMissile);
                }    
            }                
        }
    }

    private void lasserAnimDirection(float launcherYLocation)
    {
        float straightAimMargin = 1f;
        
        Debug.Log("Entering this time in AnimDirection");
        //Debug.Log("BEFORE" + "Location Mouse y: " + Utilities.GetMouseWorldPosition(Input.mousePosition).y + " Location transform y " + (missleLauncherLocationY - 1.0f));
        if (Utilities.GetMouseWorldPosition(Input.mousePosition).y< (launcherYLocation-straightAimMargin))
        {
            //shoot down
            //Debug.Log("AFTER AND DOWN" + "Location Mouse y: " + Utilities.GetMouseWorldPosition(Input.mousePosition).y + " Location transform y " + (missleLauncherLocationY - 1.0f));
            Debug.Log("Anim to play At Direction Function is Down");
            anim.SetBool(missileShotDownHash, true);
            //activate particles
        }
        else if (Utilities.GetMouseWorldPosition(Input.mousePosition).y > (launcherYLocation+straightAimMargin))
        {
            //shoot up
            //Debug.Log("AFTER AND UP " + "Location Mouse y: " + Utilities.GetMouseWorldPosition(Input.mousePosition).y + " Location transform y " + (missleLauncherLocationY - 1.0f));
            Debug.Log("Anim to play At Direction Function is Up");
            anim.SetBool(missileShotUpHash, true);
        }
        else
        {
            //shoot straight
            //Debug.Log("AFTER AND STRAIGHT " + "Location Mouse y: " + Utilities.GetMouseWorldPosition(Input.mousePosition).y + " Location transform y " + (missleLauncherLocationY - 1.0f));
            float randomStraightAnim = Random.Range(0f, 1f);
            Debug.Log("Random is " + randomStraightAnim);
            if(randomStraightAnim<=0.5f)
            {
                Debug.Log("Anim to play At Direction Function is Straight 1");
                anim.SetBool(missileShotStraight1Hash, true);
            }
            else
            {
                Debug.Log("Anim to play At Direction Function is Straight 2");
                anim.SetBool(missileShotStraight2Hash, true);
            }
        }
        positionLocked = false;
    }

    void swordSwing()
    {
        //raycast in front
        float minDistToMech = 1f;//dist for mech to swing sword
        
        bool mechFacingRight = mechScript.isFacingRight;
        Vector2 directionRight = new Vector2(5, 3);
        Vector2 directionLeft = new Vector2(-5, 3);
        Vector2 rayFwd;
        RaycastHit2D mechHit;

        if (mechFacingRight)
        {
            rayFwd = transform.TransformDirection(directionRight);
            mechHit = Physics2D.Raycast(transform.position + transform.right * -0.5f, rayFwd, enemyLayer);
            Debug.DrawRay(transform.position + transform.right * 1f, rayFwd, Color.red);
        }
        else
        {
            rayFwd = transform.TransformDirection(directionLeft);
            mechHit = Physics2D.Raycast(transform.position + transform.right * 0.5f, rayFwd, enemyLayer);
            Debug.DrawRay(transform.position + transform.right * -1f, rayFwd, Color.red);
        }

        if (mechHit)
        {
            if (mechHit.collider)
            {
                mechDist = mechHit.distance;//dist to mech
                Debug.Log("mechDist " + mechDist);
                Debug.Log("I hit collider " + mechHit.collider);
            }
            
        }
        if (mechDist < minDistToMech)
        {
            Debug.Log("Swing Anim");
            anim.SetBool(swingAtReachHash, true);
        }
    }

}