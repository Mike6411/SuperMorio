using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Vector2 myinput;
    private Vector3 mydirection;
    private CharacterController characterController;
    private float grav = -9.81f;
    private float verticalVelocity;
    private float currentVelocity;

    [SerializeField] private float smoothTime = 0.05f;

    [SerializeField] private float speed;

    [SerializeField] private float gravMod;

    [SerializeField] private float jumpPower;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        ApplyGravity();
        ApplyRotation();
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
        float targetAngle = Mathf.Atan2(mydirection.x, mydirection.z) * Mathf.Rad2Deg;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref currentVelocity, smoothTime);
        transform.rotation = Quaternion.Euler(0, angle, 0);
    }

    private void ApplyMovement()
    {
        characterController.Move(mydirection * speed * Time.deltaTime);
    }

    public void Move(InputAction.CallbackContext context)
    {
        myinput = context.ReadValue<Vector2>();
        mydirection = new Vector3(myinput.x, 0.0f, myinput.y);
        Debug.Log(myinput);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (!context.started) {return;}
        if (!isGrounded()) {return;}

        verticalVelocity += jumpPower;
    }

    private bool isGrounded() => characterController.isGrounded;
}
