using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public bool isMelee;
    public bool isRock;

    private void OnCollisionEnter(Collision collision)
    {
        if (!isRock && collision.gameObject.tag == "Floor")
            Destroy(gameObject,2);
     

        if (collision.gameObject.tag == "Wall")
            Destroy(gameObject);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isMelee && other.gameObject.tag == " Wall")
            Destroy(gameObject);

        if (!isMelee && other.gameObject.tag == "Tree")
            Destroy(gameObject);
    }

}
