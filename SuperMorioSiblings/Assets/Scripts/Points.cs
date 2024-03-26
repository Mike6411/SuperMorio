using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using static UnityEditor.PlayerSettings;

public class Points : MonoBehaviour
{

    [SerializeField]
    LevelManager LM;
    [SerializeField]
    private float height;
    [SerializeField]
    private float speed;

    private Vector3 pos;

    private void Start()
    {
        pos = transform.localPosition;
        LM = GameObject.Find("LevelManager").GetComponent<LevelManager>();
    }

    void Update()
    {
        float newY = Mathf.Sin(Time.time * speed) * height + pos.y;
        transform.position = new Vector3(pos.x, newY, pos.z);

        transform.rotation *= Quaternion.Euler(0f, 0.1f, 0f);
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
