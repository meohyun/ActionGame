using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public bool isMelee;
    public bool isRock;
    public bool isMissile;

    void Start()
    {
        if (!isMissile && !isMelee)
        StartCoroutine(selfDestory());
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (!isRock && collision.gameObject.tag == "Floor")
            Destroy(gameObject,2);
     
        if (!isMissile && collision.gameObject.tag == "WorldObject")
            Destroy(gameObject,isRock ? 2 : 0);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isMissile && !isMelee && !isRock && other.gameObject.tag == "WorldObject")
            Destroy(gameObject);

        // Boss의 Taunt 공겨
        if (isMelee && other.gameObject.tag == "Player")
        {
            Player player = other.GetComponent<Player>();
            player.Health -= damage;
        }

        // Enemy E의 총알 사라짐 
        if (!isMissile && !isMelee && other.gameObject.tag == "Player")
            Destroy(gameObject);
    }


    IEnumerator selfDestory()
    {
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }

}
