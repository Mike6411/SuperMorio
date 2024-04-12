using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;
using static UnityEditor.Timeline.TimelinePlaybackControls;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private Vector2 myinput;
    private Vector3 mydirection;
    private CharacterController characterController;
    private float grav = -9.81f;
    private float verticalVelocity;
    private Camera myCamera;
    private GameObject spawnpoint;
    private int jumpChain = 0;
    private float OGjumpPower;
    private float OGtargetTime;
    private bool jumped;
    private bool grounded;
    private float groundedCheckDistance;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private bool crouchjumped;
    private bool backfliped;
    private bool cappyExists;

    //In seconds
    [SerializeField] private float coyoteTime = 0.2f;

    //In seconds
    [SerializeField]private float jumpBufferTime = 0.2f;

    //Sensitivity
    [SerializeField] private float rotationSpeed = 50f;

    //multiplier applied to gravity
    [SerializeField] private float gravMod;

    //Initial jump strength
    [SerializeField] private float jumpPower;

    //Initial jump strength
    [SerializeField] private float crouchjumpPower;

    //Initial jump strength
    [SerializeField] private float backflipPower;

    //Consecutive jump increase
    [SerializeField] private float jumpIncrement;

    //Time between jumps before jumpchain gets reset
    [SerializeField] private float targetTime;

    [SerializeField] private GameObject cappyGO;

    [SerializeField] private Movement movement;

    //Bonus length for redundancy purposes
    [SerializeField] private float groundedbuffer;


    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        spawnpoint = GameObject.Find("Cappyspawn");
        myCamera = Camera.main;

        //Saving important values for crossreferencing 
        OGjumpPower = jumpPower;
        OGtargetTime = targetTime;
        grounded = true;
        cappyExists = false;

        //Saving the ideal length of the grounding raycast
        groundedCheckDistance = (GetComponent<CapsuleCollider>().height / 2) + groundedbuffer;
    }

    private void Update()
    {
        ApplyRotation();
        ApplyGravity();
        ApplyMovement();

        //Jumpchain buffer time handling
        Jumpchain();

        //Currently unimplemented
        JumpBuffer();

        Coyotetime();
    }

    private void FixedUpdate()
    {
        //Check grounded status
        RaycastHit hit;

        if (Physics.Raycast(transform.position, -transform.up, out hit, groundedCheckDistance))
        {
            grounded = true;
        }
        else { grounded = false; }
    }

    private void ApplyGravity()
    {
        if (grounded && verticalVelocity < 0f)
        {
            verticalVelocity = -1.0f;
        }
        else if(verticalVelocity >= grav)
        {
            verticalVelocity += grav * gravMod * Time.deltaTime;        
        }
        mydirection.y = verticalVelocity;
    }

    private void ApplyRotation()
    {

        if (myinput.sqrMagnitude == 0) return;

        //Setting the camera relative movement
        mydirection = Quaternion.Euler(0.0f, myCamera.transform.eulerAngles.y, 0.0f) * new Vector3(myinput.x, 0f, myinput.y);

        var targetRotation = Quaternion.LookRotation(mydirection, Vector3.up);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void ApplyMovement()
    {
        //Applying the sprinting and crouching movement speed modifiers
        float targetSpeed;

        //Sprint and crouch speed alterations
        if (movement.isSprinting) { targetSpeed = movement.speed * movement.sprintMultiplier; }
        else if (movement.isCrouching) { targetSpeed = movement.speed * movement.crouchMultiplier; }
        else { targetSpeed = movement.speed; }

        setAcceleration();

        //Applying acceleration with unwated speed prevention
        movement.currentSpeed = Mathf.MoveTowards(movement.currentSpeed, Mathf.Clamp(targetSpeed,0,movement.speedcap), movement.currentAccel * Time.deltaTime);

        //Clamped to not stop you from falling
        characterController.Move(mydirection * Mathf.Clamp(movement.currentSpeed,1,movement.speedcap) * Time.deltaTime);
    }

    public void Move(InputAction.CallbackContext context)
    {
        myinput = context.ReadValue<Vector2>();
        mydirection = new Vector3(myinput.x, mydirection.y, myinput.y);

        //Set bool
        if (context.started){movement.isMoving = true;}        
        else if (context.canceled){movement.isMoving = false;}
    }

    public void Jump(InputAction.CallbackContext context)
    {

        //Set bool
        if (context.started) { jumped = true; }

        if (context.canceled) { jumped = false; }

        if (!context.started) { return; }
        else if (coyoteTimeCounter > 0f)
        {
            //Reset so you can't doublejump by spamming jump
            coyoteTimeCounter = 0f;


            //Crouch jump and backflip
            if (movement.isCrouching)
            {
                //Check if it's moving backwards or forwards
                if (myinput.y > 0)
                {
                    ApplyJump(crouchjumpPower);
                }

                if (myinput.y < 0)
                {
                    ApplyJump(backflipPower);
                }
            }
            //Normal jump
            else
            {
                //JumpPower increase depending on where on the jumpchain you are
                if (jumpChain < 3 && targetTime != 0)
                {
                    jumpPower += jumpIncrement;
                    jumpChain++;
                }

                ApplyJump(jumpPower);
                targetTime = OGtargetTime;

                //JumpChain reset once you reach 3
                if (jumpChain >= 3)
                {
                    //So that the animator can actually get the jumpchain value
                    Invoke("timerEnded", 0.2f);
                }
            }

        }
    }

    private void JumpBuffer()
    {
        if (jumped) 
        {
            jumpBufferCounter = jumpBufferTime;
        } 
        else 
        {
            jumpBufferCounter -= Time.deltaTime;
        }
    }

    internal void ApplyJump(float var)
    {
        verticalVelocity = 0f;
        verticalVelocity += var;

    }

    public void Cappy(InputAction.CallbackContext context)
    {
        if (context.started && !cappyExists) 
        {
            Instantiate(cappyGO, spawnpoint.transform.position, transform.rotation);
            cappyExists = true;
            Debug.Log("CappyLaunch");       
        }
    }

    public void Sprint(InputAction.CallbackContext context)
    {
        //Can't sprint while crouching
        if (!movement.isCrouching) {movement.isSprinting = context.started || context.performed;}
    }

    public void Crouch(InputAction.CallbackContext context)
    {
        //Hitbox modification & set bool
        if (context.started){ 
            transform.localScale = new Vector3(1, 0.5f, 1);
            movement.isCrouching = true;
        }

        if (context.canceled)
        {
            transform.localScale = new Vector3(1, 1, 1);
            movement.isCrouching = false;
        }
    }

    private void timerEnded()
    {
        //Value reset on end of jumpchain
        targetTime = OGtargetTime;
        jumpPower = OGjumpPower;
        jumpChain = 0;
    }

    private void setAcceleration()
    {
        //If you're actively pressing a movement key you'll accelerate otherwise you'll decelerate untill you stop moving

        if (movement.isMoving) {movement.currentAccel = movement.accel;}
        else if (!movement.isMoving)
        {
            if(movement.currentSpeed > 0)
            {
                movement.currentAccel = movement.decel;
            }
            else
            {
                movement.currentAccel = 0;
            }
        }
    }
    
    private void Jumpchain()
    {
        //Constantly reduce targetTime till it reaches 0
        if (targetTime >= -1)
        {
            targetTime -= Time.deltaTime;
        }

        if (targetTime <= 0)
        {
            timerEnded();
        }
    }

    private void Coyotetime()
    {
        //Count down from coyote time if not grounded and reset if grounded
        if (grounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
    }

    internal void Cappydied(){cappyExists = false;}

    //Getters for AnimationManager
    internal float GetCurrentSpeed(){return this.movement.currentSpeed;}

    internal bool GetJump(){return this.jumped;}

    internal bool GetCrouch(){return this.movement.isCrouching;}

    internal bool GetCrouchjumped() { return this.movement.isCrouching; }

    internal bool GetBackfliped() { return this.movement.isCrouching; }

    internal bool GetGrounded(){return this.grounded;}

    internal int GetJumpchain(){return this.jumpChain;}

    internal float GetCurrentYSpeed(){return this.verticalVelocity;}
}


//Handles frequently used values for the movement of the PC all packed together for convenience of use
//Should be private and have getters and setters but eh 

[Serializable]
 internal struct Movement
{
    //static values (set through Serializable for convenience)
    public float speed;
    public float sprintMultiplier;
    public float crouchMultiplier;
    public float accel;
    public float decel;
    //incase you break something and start going too quick
    public float speedcap;

    //changing values
    public bool isMoving;
    public bool isSprinting;
    public bool isCrouching;
    public float currentSpeed;
    public float currentAccel;
}
