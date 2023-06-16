using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossMissile : MonoBehaviour
{
    public Transform target;
    public GameObject particle;
    public GameObject explosionEffect;
    NavMeshAgent nav;

    // Start is called before the first frame update
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();

        StartCoroutine(selfDestory());
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.back * 30 * Time.deltaTime);
        nav.SetDestination(target.position);
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

        if (other.gameObject.tag == "Wall" || other.gameObject.tag == "Tree")
        {
            StartCoroutine(MissileHit());
        }

    }

    IEnumerator selfDestory()
    {
        yield return new WaitForSeconds(3f);
        particle.SetActive(false);

        yield return new WaitForSeconds(0.1f);
        explosionEffect.SetActive(true);

        Destroy(transform.gameObject, 0.3f);
    }

    IEnumerator MissileHit()
    {
        particle.SetActive(false);
        explosionEffect.SetActive(true);
        yield return new WaitForSeconds(0.1f);

        Destroy(transform.gameObject, 0.3f);

    }

}
