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

    [SerializeField] private float rotationSpeed = 50f;

    [SerializeField] private float speed;

    [SerializeField] private float gravMod;

    [SerializeField] private float jumpPower;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        myCamera = Camera.main;
    }

    private void Update()
    {        
        ApplyRotation();
        ApplyGravity();
        ApplyMovement();
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
        //Debug.Log(myinput);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (!context.started) {return;}
        if (isGrounded()) { verticalVelocity += jumpPower; }
    }

    private bool isGrounded() => characterController.isGrounded;
}
