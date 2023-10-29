using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeScript : MonoBehaviour
{

    [SerializeField]
    LevelManager LM;


    private void Start()
    {
        LM = GameObject.Find("LevelManager").GetComponent<LevelManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            LM.restartLevel();
        }
    }
}
