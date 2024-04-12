using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CappyScript : MonoBehaviour
{
    [SerializeField]
    PlayerController pc;

    //Controls the strength with witch the player will be forced to jump with
    [SerializeField]
    private float bouncePower;

    [SerializeField]
    private float speed = 2;

    private void Start()
    {
        pc = GameObject.Find("PC").GetComponent<PlayerController>();
        speed = speed + pc.GetCurrentSpeed();
        Invoke("Stop", 2f);
    }

    private void Update()
    {
        if (speed != 0) 
        {
            transform.position += transform.forward * Time.deltaTime * speed;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //When player touches you, make them jump
        if (other.tag == "Player")
        {
            pc.ApplyJump(bouncePower);
            pc.Cappydied();
            Destroy(gameObject);
        }
        else
        {
            Stop();
        }
    }

    //Stop moving
    private void Stop(){ speed = 0; }

}
