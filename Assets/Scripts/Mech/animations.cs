using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animations : MonoBehaviour
{

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

    //anim IDs
    int flyHash = Animator.StringToHash("flying");

    //on fly rotation
    private float smooth = 2.0f;
    private float tiltAngle = 30.0f;
    float GroundDist;
    int mechsLayer;
    int possibleGround;

    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
        mechScript = GetComponent<Mech>();

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
        mechImIn = Player.mechImIn;
        
        //raycast dist to ground
        float minDistToGround = 1;//dist for standby animation to occur
        Vector2 rayDown = transform.TransformDirection(Vector2.down);
        //circle collider may required this
        //Vector2 shift = new Vector2(0, 0.0f);
        //Vector2 shiftedPosition = transform.position;
        //shiftedPosition+=shift;

        //start += transform.position + Vector2(0, -1);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDown);
        
        if (hit)
        {
            if(hit.collider)
            {
                GroundDist = hit.distance;//dist to ground
                Debug.Log("collider hit " + hit.collider.gameObject.name);
                Debug.Log("Distance to Ground " + GroundDist);
                Debug.DrawRay(transform.position, rayDown, Color.red);
            }
        }
        
        if (GroundDist< minDistToGround)
            anim.SetBool("onGround", true);
        else
            anim.SetBool("onGround", false);

        if (mechImIn)
        {
            anim.SetBool("animMechImIn", true);
        }

        if (GroundDist < minDistToGround && mechImIn && !inputUp && !inputDown && (inputLeft || inputRight))//mechScript.inUse
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

        if (mechImIn && (inputLeft || inputRight) && (inputUp || inputDown))
        {
            anim.SetBool(flyHash, true);
        }
        else
        {
            anim.SetBool(flyHash, false);
        }
    }
}