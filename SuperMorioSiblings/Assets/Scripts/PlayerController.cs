using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Vector2 myinput;
    private CharacterController characterController;
    private Vector3 mydirection;

    [SerializeField] private float speed;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        characterController.Move(mydirection * speed * Time.deltaTime);
    }

    public void Move(InputAction.CallbackContext context)
    {
        myinput = context.ReadValue<Vector2>();
        mydirection = new Vector3(myinput.x, 0.0f, myinput.y);
        Debug.Log(myinput);
    }
}
