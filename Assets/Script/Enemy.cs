using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum Type { A, B ,C }
    public Type enemyType;

    public int curHp;
    public int maxHp;

    public bool isChase;
    public bool isAttack;

    public Transform target;

    public BoxCollider meleeArea;

    public GameObject bullet;

    Rigidbody rb;
    BoxCollider boxCollider;
    Material mat;
    Animator anim;

    NavMeshAgent nav;

    Vector3 moveVec;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mat = GetComponentInChildren<MeshRenderer>().material;
        anim = GetComponentInChildren<Animator>();
        nav = GetComponent<NavMeshAgent>();

        Invoke("startMove", 2f);
    }




    void Update()
    { 
        Move();
    }

    void startMove()
    {
        isChase = true;
        anim.SetBool("isWalk", true);
    }

    void Move()
    { 
        if (nav.enabled)
        {
            nav.SetDestination(target.position);
            nav.isStopped = !isChase;

        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            curHp -= weapon.damage;
            Vector3 reactVec = transform.position - other.transform.position;
            StartCoroutine(OnDamage(reactVec,false));
        }

        else if (other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curHp -= bullet.damage;
            Vector3 reactVec = transform.position - other.transform.position;
            Destroy(other.gameObject);
            StartCoroutine(OnDamage(reactVec,false));
        }
    }

    public void hitGrenade(Vector3 hitPos)
    {
        curHp -= 30;
        Vector3 reactVec = transform.position - hitPos;
        StartCoroutine(OnDamage(reactVec,true));
    }

    IEnumerator OnDamage(Vector3 reactVec,bool isGrenade)
    {

        mat.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        if (curHp > 0)
        {
            mat.color = Color.white;
            

            if (isGrenade)
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up * 20;
                rb.freezeRotation = false;
                rb.AddForce(reactVec * 20, ForceMode.Impulse);
                rb.AddTorque(reactVec * 15,  ForceMode.Impulse);
                

            }

            else
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up;
                rb.AddForce(reactVec * 5, ForceMode.Impulse);
            }
        }
        else
        {
            mat.color = Color.gray;
            nav.enabled = false;
            gameObject.layer = 10;
            anim.SetTrigger("Die");
            Destroy(gameObject, 1);
        }
    }

    void FreezeRotation()
    {

        
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        

    }

    void Targeting()
    {
        float targetRadius = 0f;
        float targetRange = 0f;

        switch (enemyType)
        {
            case Type.A:
                targetRadius = 1.5f;
                targetRange = 3f;
                break;
            case Type.B:
                targetRadius = 1.5f;
                targetRange = 12f;
                break;
            case Type.C:
                targetRadius = 0.5f;
                targetRange = 25f;
                break;
        }

        RaycastHit[] raycastHits = Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player"));

        if (raycastHits.Length > 0 && !isAttack)
        {
            StartCoroutine(Attack());
        }

    }

    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        anim.SetBool("isAttack", true);

        switch (enemyType)
        {
            case Type.A:
                yield return new WaitForSeconds(0.2f);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(1f);
                meleeArea.enabled = false;

                yield return new WaitForSeconds(1f);
                break;

            case Type.B:
                yield return new WaitForSeconds(0.1f);
                meleeArea.enabled = true;
                rb.AddForce(transform.forward* 20, ForceMode.Impulse);
                
                yield return new WaitForSeconds(0.5f);
                rb.velocity = Vector3.zero;
                meleeArea.enabled = false;

                yield return new WaitForSeconds(2f);
                break;

            case Type.C:
                yield return new WaitForSeconds(0.7f);
                GameObject instantBullet = Instantiate(bullet, transform.position, transform.rotation);
                Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
                rigidBullet.velocity = transform.forward * 20;

                yield return new WaitForSeconds(2f);
                break;
        }

        isChase = true;
        isAttack = false;
        anim.SetBool("isAttack", false);


    }

    private void FixedUpdate()
    {
        Targeting();
        FreezeRotation();
    }

}

