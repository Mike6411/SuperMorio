using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class Points : MonoBehaviour
{

    [SerializeField]
    LevelManager LM;
    [SerializeField]
    private float rotation = 10f;


    private void Start()
    {
        LM = GameObject.Find("LevelManager").GetComponent<LevelManager>();
    }

    void FixedUpdate()
    {
        gameObject.transform.Rotate(0f, rotation*Time.deltaTime, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            LM.collectPoint();
            Destroy(gameObject);
        }
    }
}
