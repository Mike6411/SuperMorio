using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class Points : MonoBehaviour
{

    [SerializeField]
    LevelManager LM;
    [SerializeField]
    private float height;
    [SerializeField]
    private float speed;

    private float newY;
    private Vector3 pos;

    private void Start()
    {
        pos = transform.position;
        LM = GameObject.Find("LevelManager").GetComponent<LevelManager>();
    }

    void Update()
    {
        //Updown
        newY = Mathf.Sin(Time.time * speed) * height + pos.y;
        transform.position = new Vector3(pos.x, newY, pos.z);

        //Y rotation
        transform.rotation *= Quaternion.Euler(0f, 0.1f, 0f);
    }

    //When touched by player add points and autodestroy
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            LM.collectPoint();
            Destroy(gameObject);
        }
    }
}
