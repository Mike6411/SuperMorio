using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class Points : MonoBehaviour
{

    [SerializeField]
    LevelManager LM;

    //Vertical movement range goes from height to -height
    [SerializeField]
    private float height;

    //Speed at which the GO will move up and down
    [SerializeField]
    private float speed;

    //Rotation speed
    [SerializeField]
    private float yRotation;

    private float newY;
    private Vector3 pos;

    private void Start()
    {
        pos = transform.position;
        LM = GameObject.Find("LevelManager").GetComponent<LevelManager>();
    }

    void Update()
    {
        //Updown movement
        newY = Mathf.Sin(Time.time * speed) * height + pos.y;
        transform.position = new Vector3(pos.x, newY, pos.z);

        //Y rotation
        transform.rotation *= Quaternion.Euler(0f, yRotation, 0f);
    }

    //When touched by player add points and die
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            LM.collectPoint();
            Destroy(gameObject);
        }
    }
}
