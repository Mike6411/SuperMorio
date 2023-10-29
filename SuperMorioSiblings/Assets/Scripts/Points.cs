using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Points : MonoBehaviour
{

    [SerializeField]
    LevelManager LM;
    [SerializeField]
    private float rotation = 70f;


    private void Start()
    {
        LM = GameObject.Find("LevelManager").GetComponent<LevelManager>();
    }

    void Update()
    {
        transform.Rotate(0, 0, rotation * Time.deltaTime);
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
