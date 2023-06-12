using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossMissile : Bullet
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
        nav.SetDestination(target.position);
    }

    IEnumerator selfDestory()
    {
        yield return new WaitForSeconds(3f);
        particle.SetActive(false);
        
        yield return new WaitForSeconds(0.1f);
        explosionEffect.SetActive(true);
        
        Destroy(transform.gameObject, 0.3f);


    }

}
