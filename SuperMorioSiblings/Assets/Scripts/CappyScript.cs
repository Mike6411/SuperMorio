using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CappyScript : MonoBehaviour
{

    [SerializeField]
    PlayerController PC;

    //Controls the strength with witch the player will be forced to jump with
    [SerializeField]
    private float bouncePower;

    //Rotation speed
    [SerializeField]
    private float yRotation;


    private void Start()
    {
        PC = GameObject.Find("PC").GetComponent<PlayerController>();
    }

    private void Update()
    {
        //Y rotation
        transform.rotation *= Quaternion.Euler(0f, yRotation, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        //When player touches you, make them jump
        if (other.tag == "Player")
        {
            PC.ApplyJump(bouncePower);
        }
    }
}
