using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{   
    public Camera Camera;
    Vector3 offSet;
    // Start is called before the first frame update
    void Start()
    {
        offSet = Camera.transform.position - transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        Camera.transform.position = transform.position + offSet;
    }
}
