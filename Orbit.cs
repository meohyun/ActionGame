using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    public Transform target;
    public float orbitSpeed;
    Vector3 offSet;


    // Start is called before the first frame update
    void Start()
    {
        offSet = transform.position - target.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = target.position + offSet;
        // 사물 주변으로 돎
        transform.RotateAround(target.position, Vector3.up, orbitSpeed * Time.deltaTime);
        
        offSet = transform.position - target.position;
    }
}
