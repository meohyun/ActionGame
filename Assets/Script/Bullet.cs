using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public bool isMelee;

    private void OnCollisionEnter(Collision collision)
    {
        if (!isMelee && collision.gameObject.tag == "Floor")
        {
            Destroy(gameObject,2);
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isMelee && other.gameObject.tag == " Wall")
        {
            Destroy(gameObject);
        }
    }

}
