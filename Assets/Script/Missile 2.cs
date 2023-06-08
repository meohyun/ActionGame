using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{

    public GameObject missileParticle;
    public GameObject EffectObj;
    public GameObject meshRen;


    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.back * 30 * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {

            // 반동
            Player player = other.GetComponent<Player>();
            Vector3 reactVec = other.transform.position - transform.position;
            reactVec += Vector3.up;
            player.rb.AddForce(reactVec * 5, ForceMode.Impulse);
            player.rb.velocity = Vector3.zero;
            
            
            StartCoroutine(MissileHit());
        }

        if (other.gameObject.tag == "Wall")
        {
            StartCoroutine(MissileHit());
        }

    }

    IEnumerator MissileHit()
    {
        missileParticle.SetActive(false);
        
        yield return new WaitForSeconds(0.1f);
        EffectObj.SetActive(true);

        Destroy(transform.gameObject,0.3f);

    }
}
