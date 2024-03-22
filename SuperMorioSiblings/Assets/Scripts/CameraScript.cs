using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraScript : MonoBehaviour
{

    [SerializeField] private Transform target;
    [SerializeField] private MouseSensitivity mouseSens;
    [SerializeField] private CameraAngle camAngle;

    private float distanceToPlayer;

    private Vector2 input;
    private CameraRotation camRot;


    private void Awake()
    {
        distanceToPlayer = Vector3.Distance(transform.position, target.position);
    }

    public void Look(InputAction.CallbackContext context) 
    {
        input = context.ReadValue<Vector2>();
    }

    private void Update()
    {
        //Applying desired sensitivity values to mouse input 
        camRot.yaw += input.x * mouseSens.horizontal * Time.deltaTime;
        camRot.pitch += input.y * mouseSens.vertical * Time.deltaTime;
        camRot.pitch = Mathf.Clamp(camRot.pitch, camAngle.min, camAngle.max);
    }

    private void LateUpdate()
    {
        transform.eulerAngles = new Vector3(camRot.pitch, camRot.yaw, 0);
        transform.position = target.position - transform.forward * distanceToPlayer;
    }


}


[Serializable]
public struct MouseSensitivity
{
    public float horizontal,vertical;
}

[Serializable]
public struct CameraAngle
{
    public float min,max;
}

public struct CameraRotation
{
    public float pitch,yaw;
}


