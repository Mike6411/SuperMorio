using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncepadScript : MonoBehaviour
{

    [SerializeField]
    PlayerController PC;

    [SerializeField]
    private float bouncePower;


    private void Start()
    {
        PC = GameObject.Find("PC").GetComponent<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            PC.ApplyJump(bouncePower);
        }
    }
}
