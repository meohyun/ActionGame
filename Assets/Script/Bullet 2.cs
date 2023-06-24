using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public bool isMelee;
    public bool isRock;
    public bool isMissile;

    private void OnCollisionEnter(Collision collision)
    {
        if (!isRock && collision.gameObject.tag == "Floor")
            Destroy(gameObject,2);
     
        if (!isMissile && collision.gameObject.tag == "Wall")
            Destroy(gameObject,isRock ? 2 : 0);

        if (collision.gameObject.tag == "Tree")
            Destroy(gameObject, 2);


    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isMissile && !isMelee && !isRock && other.gameObject.tag == "Wall")
            Destroy(gameObject);

        if (!isMissile && !isMelee && !isRock && other.gameObject.tag == "Tree")
            Destroy(gameObject);

        if (isMelee && other.gameObject.tag == "Player")
        {
            Player player = other.GetComponent<Player>();
            player.Health -= damage;
        }
    }

}
