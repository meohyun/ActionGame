using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{

    public enum Type { Ammo,Grenade,Coin,Life,Weapon}
    public Type type;
    public int value;

    Rigidbody rb;
    SphereCollider sphereCollider;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * 20 * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Floor")
        {
            rb.isKinematic = true;
            sphereCollider.enabled = false;
        }
    }
}
