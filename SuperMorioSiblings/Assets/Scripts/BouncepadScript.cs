using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncepadScript : MonoBehaviour
{

    [SerializeField]
    PlayerController PC;

    //Controls the strength with witch the player will be forced to jump with
    [SerializeField]
    private float bouncePower;


    private void Start()
    {
        PC = GameObject.Find("PC").GetComponent<PlayerController>();
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
