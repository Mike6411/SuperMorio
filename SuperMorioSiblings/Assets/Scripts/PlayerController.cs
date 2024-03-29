using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private Vector2 myinput;
    private Vector3 mydirection;
    private CharacterController characterController;
    private float grav = -9.81f;
    private float verticalVelocity;
    private Camera myCamera;
    private int jumpchain = 0;
    private float OGjumpPower;
    private float OGtargetTime;
    private Animator myAnimator;
    private bool jumped;
    private bool grounded;
    private float groundedCheckDistance;


    [SerializeField] private float rotationSpeed = 50f;

    [SerializeField] private float gravMod;

    //Initial jump strength
    [SerializeField] private float jumpPower;

    //Consecutive jump increase
    [SerializeField] private float jumpIncrement;

    //Time between jumps before jumpchain gets reset
    [SerializeField] private float targetTime;

    [SerializeField] private Movement movement;

    [SerializeField] private float groundedbuffer = 0.1f;


    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        myAnimator = GetComponent<Animator>();
        myCamera = Camera.main;

        //Saving important values for crossreferencing 
        OGjumpPower = jumpPower;
        OGtargetTime = targetTime;
        grounded = true;

        //Saving the ideal length of the grounding raycast
        groundedCheckDistance = (GetComponent<CapsuleCollider>().height / 2) + groundedbuffer;
    }

    private void Update()
    {        
        ApplyRotation();
        ApplyGravity();
        ApplyMovement();

        //Jump sequence buffer time handling
        if (targetTime >= -1) 
        {
            targetTime -= Time.deltaTime;
        }
        
        if (targetTime < 0)
        {
            timerEnded();
        }
    }

    private void ApplyGravity()
    {
        if (grounded && verticalVelocity < 0f)
        {
            verticalVelocity = -1.0f;
        }
        else
        {
            verticalVelocity += grav * gravMod * Time.deltaTime;        
        }
        mydirection.y = verticalVelocity;
    }

    private void ApplyRotation()
    {

        if (myinput.sqrMagnitude == 0) return;

        //Setting the camera relative movement
        mydirection = Quaternion.Euler(0.0f, myCamera.transform.eulerAngles.y, 0.0f) * new Vector3(myinput.x, 0.0f, myinput.y);

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

        //Applying acceleration
        movement.currentSpeed = Mathf.MoveTowards(movement.currentSpeed, targetSpeed, movement.currentAccel * Time.deltaTime);

        characterController.Move(mydirection * movement.currentSpeed * Time.deltaTime);
    }

    public void Move(InputAction.CallbackContext context)
    {
        myinput = context.ReadValue<Vector2>();
        mydirection = new Vector3(myinput.x, 0.0f, myinput.y);

        //Boolset
        if (context.started)
        {
            movement.isMoving = true;
        }        
        else if (context.canceled)
        {
            movement.isMoving = false;
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        //Check grounded status before jumping
        RaycastHit hit;

        if (Physics.Raycast(transform.position, -transform.up, out hit, groundedCheckDistance))
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }

        //Jumpchain handling
        if (!context.started) 
        {
            jumped = false;
            return;
        }
        else if (grounded) 
        {
            if (jumpchain < 3 && targetTime != 0)
            {
                jumpPower += jumpIncrement;
                jumpchain++;
            }
            verticalVelocity = 0f;
            verticalVelocity += jumpPower;
            jumped = true;
            targetTime = OGtargetTime;
        }
        
    }

    public void Sprint(InputAction.CallbackContext context)
    {
        //Can't sprint while crouching
        if (!movement.isCrouching) {movement.isSprinting = context.started || context.performed;}
    }

    public void Crouch(InputAction.CallbackContext context)
    {
        if (context.started)
        { 
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
        jumpchain = 0;
    }

    private void setAcceleration()
    {
        //If you're actively pressing a movement key you'll accelerate otherwise you'll decelerate untill you stop moving
        if (movement.isMoving) 
        {
            movement.currentAccel = movement.accel;
        }
        else if (!movement.isMoving)
        {
            if(movement.currentSpeed > 0 && grounded)
            {
                movement.currentAccel = movement.decel;
            }
            else
            {
                movement.currentAccel = 0;
            }
        }
    }

    //Getters for AnimationManager
    internal float GetCurrentSpeed(){return this.movement.currentSpeed;}

    internal bool GetJump(){return this.jumped;}

    internal bool GetCrouch(){return this.movement.isCrouching;}

    internal bool GetGrounded(){return this.grounded;}

    internal int GetJumpchain(){return this.jumpchain;}
}


//Handles frequently used values for the movement of the PC all packed together for convenience of use
//Should be private and have getters and setters 

[Serializable]
 public struct Movement
{
    //static values (set through inspector for convenience)
    public float speed;
    public float sprintMultiplier;
    public float crouchMultiplier;
    public float accel;
    public float decel;

    //changing values
    public bool isMoving;
    public bool isSprinting;
    public bool isCrouching;
    public float currentSpeed;
    public float currentAccel;
}
