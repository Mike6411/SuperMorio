using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
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


    [SerializeField] private float rotationSpeed = 50f;

    [SerializeField] private float gravMod;

    [SerializeField] private float jumpPower;

    [SerializeField] private float targetTime;

    [SerializeField] private Movement movement;


    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        myAnimator = GetComponent<Animator>();
        myCamera = Camera.main;
        OGjumpPower = jumpPower;
        OGtargetTime = targetTime;
    }

    private void Update()
    {        
        ApplyRotation();
        ApplyGravity();
        ApplyMovement();

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
        if (isGrounded() && verticalVelocity < 0f)
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

        mydirection = Quaternion.Euler(0.0f, myCamera.transform.eulerAngles.y, 0.0f) * new Vector3(myinput.x, 0.0f, myinput.y);

        var targetRotation = Quaternion.LookRotation(mydirection, Vector3.up);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void ApplyMovement()
    {
        float targetSpeed = movement.isSprinting ? movement.speed * movement.multiplier : movement.speed;
        movement.currentSpeed = Mathf.MoveTowards(movement.currentSpeed, targetSpeed, movement.acceleration * Time.deltaTime);

        characterController.Move(mydirection * movement.currentSpeed * Time.deltaTime);
    }

    public void Move(InputAction.CallbackContext context)
    {
        myinput = context.ReadValue<Vector2>();
        mydirection = new Vector3(myinput.x, 0.0f, myinput.y);

        if (context.started)
        {
            myAnimator.SetBool("IsWalking", true);
        }
        else
        {
            myAnimator.SetBool("IsWalking", false);
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (!context.started) {return;}
        else if (isGrounded()) 
        {
            if (jumpchain < 3 && targetTime != 0)
            {
                jumpPower += 2;
                jumpchain++;
            }
            verticalVelocity += jumpPower;
            targetTime = OGtargetTime;
        }
        
    }

    public void Sprint(InputAction.CallbackContext context)
    {
        movement.isSprinting = context.started || context.performed;
    }

    public void Crouch(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            transform.localScale = new Vector3(1, 0.5f, 1);
        }

        if (context.canceled)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    private void timerEnded()
    {
        targetTime = OGtargetTime;
        jumpPower = OGjumpPower;
        jumpchain = 0;
    }

    private bool isGrounded() => characterController.isGrounded;
}

[Serializable]

public struct Movement
{
    public float speed;
    public float multiplier;
    public float acceleration;

    [HideInInspector] public bool isSprinting;
    [HideInInspector] public float currentSpeed;
}
