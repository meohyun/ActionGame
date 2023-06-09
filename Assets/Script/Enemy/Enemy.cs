using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum Type { A, B ,C ,D, E, Boss , Turret}
    public Type enemyType;

    public int curHp;
    public int maxHp;
    public int score;

    public bool isChase;
    public bool isAttack;
    public bool isDead;

    public Transform target;
    public Transform EnemyEbulletPos;

    public BoxCollider meleeArea;

    public GameObject bullet;
    public GameObject[] coins;
    public GameObject explosionEffect;
    GameObject instantBullet;

    public Rigidbody rb;
    public BoxCollider boxCollider;
    public MeshRenderer[] meshs;
    public Animator anim;

    public NavMeshAgent nav;

    public GameManager manager;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        anim = GetComponentInChildren<Animator>();

        if (enemyType != Type.Turret)
            nav = GetComponent<NavMeshAgent>();

        target = GameObject.Find("Player").transform;

        if (enemyType != Type.Boss && enemyType != Type.Turret)
            Invoke("startMove", 2f);
        
    }

    void Update()
    {
        if (enemyType != Type.Turret)
            Move();
    }

    void startMove()
    {
        isChase = true;
        anim.SetBool("isWalk", true);
    }

    void Move()
    { 
        if (nav.enabled && enemyType != Type.Boss)
        {
            nav.SetDestination(target.position);
            nav.isStopped = !isChase;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Melee" && !isDead && enemyType != Type.Turret)
        {
            Weapon weapon = other.GetComponent<Weapon>();
            curHp -= weapon.damage;
            Vector3 reactVec = transform.position - other.transform.position;
            StartCoroutine(OnDamage(reactVec,false));
        }

        if (other.tag == "Bullet" && !isDead && enemyType != Type.Turret)
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

    IEnumerator OnDamage(Vector3 reactVec, bool isGrenade)
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
                rb.AddTorque(reactVec * 15, ForceMode.Impulse);

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
            gameObject.layer = 10;
            isDead = true;
            nav.enabled = false;
            anim.SetTrigger("Die");

            switch (enemyType)
            {
                case Type.A:
                    manager.enemyCntA--;
                    break;
                case Type.B:
                    manager.enemyCntB--;
                    break;
                case Type.C:
                    Destroy(instantBullet);
                    manager.enemyCntC--;
                    break;
                case Type.D:
                    manager.enemyCntD--;
                    break;
                case Type.E:
                    manager.enemyCntE--;
                    break;

            }
            foreach (MeshRenderer mesh in meshs)
            {
                mesh.material.color = Color.gray;
            }


            Player player = target.GetComponent<Player>();
            player.score += score;
            int ranCoin = Random.Range(0, 3);

            if (enemyType == Type.Boss)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector3 ranVec = Vector3.forward * Random.Range(-i + 1, i + 1) + Vector3.right * Random.Range(-i + 1, i + 1);
                    Instantiate(coins[i], transform.position + ranVec, Quaternion.identity);
                }
            }
            else
            {
                Instantiate(coins[ranCoin], transform.position, Quaternion.identity);

            }

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
        if (!isDead && enemyType != Type.Boss)
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
                case Type.D:
                    targetRadius = 1.5f;
                    targetRange = 3f;
                    break;
                case Type.E:
                    targetRadius = 1.5f;
                    targetRange = 15f;
                    break;
            }

            RaycastHit[] raycastHits = Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player"));

            if (raycastHits.Length > 0 && !isAttack)
                StartCoroutine(Attack());
            
            else if (!isAttack && enemyType == Type.Turret)
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
                rb.AddForce(transform.forward* 25, ForceMode.Impulse);
                
                yield return new WaitForSeconds(0.5f);
                rb.velocity = Vector3.zero;
                meleeArea.enabled = false;

                yield return new WaitForSeconds(2f);
                break;

            case Type.C:
                yield return new WaitForSeconds(0.7f);
                instantBullet = Instantiate(bullet, transform.position, transform.rotation);
                Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
                rigidBullet.velocity = transform.forward * 20;

                yield return new WaitForSeconds(2f);
                break;

            case Type.D:

                foreach (MeshRenderer mesh in meshs)
                {
                    mesh.material.color = Color.red;
                }
                gameObject.layer = 10;
                isDead = true;
                nav.enabled = false;

                yield return new WaitForSeconds(0.2f);
                explosionEffect.SetActive(true);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(1f);
                Destroy(gameObject);
                manager.enemyCntD--;

                yield return new WaitForSeconds(1f);
                break;

            case Type.Turret:
                yield return new WaitForSeconds(0.7f);
                GameObject instantMissile = Instantiate(bullet, transform.position, transform.rotation);
                Rigidbody rigidMissile = instantMissile.GetComponent<Rigidbody>();
                rigidMissile.velocity = transform.forward * 20;

                yield return new WaitForSeconds(2f);
                break;
            case Type.E:
                yield return new WaitForSeconds(0.7f);
                instantBullet = Instantiate(bullet, EnemyEbulletPos.position, EnemyEbulletPos.rotation);
                Rigidbody rigid = instantBullet.GetComponent<Rigidbody>();
                rigid.velocity = EnemyEbulletPos.forward * 50;

                yield return new WaitForSeconds(1f);

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

