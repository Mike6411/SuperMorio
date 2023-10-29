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

    [SerializeField] private float rotationSpeed = 50f;

    [SerializeField] private float speed;

    [SerializeField] private float gravMod;

    [SerializeField] private float jumpPower;

    [SerializeField] private float targetTime;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
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
        characterController.Move(mydirection * speed * Time.deltaTime);
    }

    public void Move(InputAction.CallbackContext context)
    {
        myinput = context.ReadValue<Vector2>();
        mydirection = new Vector3(myinput.x, 0.0f, myinput.y);
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

    public void Crouch(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            transform.localScale = new Vector3(1, 0.5f, 1);
            Debug.Log("Started crouching");
        }

        if (context.canceled)
        {
            transform.localScale = new Vector3(1, 1, 1);
            Debug.Log("Finished crouching");
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
