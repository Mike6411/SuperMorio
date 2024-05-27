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

    [SerializeField]
    private float stopseconds;

    [SerializeField]
    private float dieseconds;

    private void Start()
    {
        //Stop moving after stopseconds, despawn after dieseconds (settable through inspector)
        pc = GameObject.Find("PC").GetComponent<PlayerController>();
        speed = speed + pc.GetCurrentSpeed();
        Invoke("Stop", stopseconds);
        Invoke("Die", dieseconds);
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
            Die();
        }
        else
        {
            Stop();
        }
    }

    private void Die()
    {
        pc.Cappydied();
        Destroy(gameObject);
    }

    //Stop moving
    private void Stop(){ speed = 0; }

}
