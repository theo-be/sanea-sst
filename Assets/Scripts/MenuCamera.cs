using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCamera : MonoBehaviour
{
    public GameObject cam;
    public float distance;

    private GameObject self;
    
    void Start()
    {
        self = gameObject;
    }

    
    void Update()
    {
        // self.transform.eulerAngles = cam.transform.eulerAngles;
        // self.transform.position = cam.transform.position + cam.transform.position.normalized * distance;
    }
}
