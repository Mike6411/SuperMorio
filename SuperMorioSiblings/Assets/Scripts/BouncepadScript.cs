using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncepadScript : MonoBehaviour
{

    [SerializeField]
    PlayerController PC;


    private void Start()
    {
        PC = GameObject.Find("PC").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
