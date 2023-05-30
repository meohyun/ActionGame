using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam: MonoBehaviour
{

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
        cam.transform.position = transform.position + offset;
    }
}
