using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam: MonoBehaviour
{
    bool backDown;

    public Camera cam;
    Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {

        offset = cam.transform.position - transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        backDown = Input.GetButtonDown("back");

        cam.transform.position = transform.position + offset;
        
        if (backDown)
        {
            cam.transform.position *= -1;
        }    
    }
}
