using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum Type { A, B ,C ,D}
    public Type enemyType;

    public int curHp;
    public int maxHp;

    public bool isChase;
    public bool isAttack;
    public bool isDead;

    public Transform target;

    public BoxCollider meleeArea;

    public GameObject bullet;

    public Rigidbody rb;
    public BoxCollider boxCollider;
    public MeshRenderer[] meshs;
    public Animator anim;

    public NavMeshAgent nav;

    public GameManager manager;

    Vector3 moveVec;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        anim = GetComponentInChildren<Animator>();
        nav = GetComponent<NavMeshAgent>();


        if (enemyType != Type.D)
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
        if (nav.enabled && enemyType != Type.D)
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

        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.red;
        }
        yield return new WaitForSeconds(0.1f);

        if (curHp > 0)
        {
            foreach (MeshRenderer mesh in meshs)
            {
                mesh.material.color = Color.white;
            }


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
                rb.AddForce(reactVec * 3, ForceMode.Impulse);
            }
        }
        else
        {
            switch (enemyType)
            {
                case Type.A:
                    manager.enemyCntA--;
                    break;
                case Type.B:
                    manager.enemyCntB--;
                    break;
                case Type.C:
                    manager.enemyCntC--;
                    break;
            }
            foreach (MeshRenderer mesh in meshs)
            {
                mesh.material.color = Color.gray;
            }
            isDead = true;
            nav.enabled = false;
            gameObject.layer = 10;
            anim.SetTrigger("Die");

            if (enemyType != Type.D)
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
        if (!isDead && enemyType != Type.D)
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
                    targetRadius = 0.1f;
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
                rb.AddForce(transform.forward* 25, ForceMode.Impulse);
                
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

