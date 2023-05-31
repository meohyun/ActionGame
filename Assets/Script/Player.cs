using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float moveSpeed;
    public float jumpPower;
    public GameObject[] weapons;
    public bool[] hasWeapon;
    public GameObject[] grenades;
    public GameObject grenadeObject;
    public int HasGrenades;
    public Camera followCamera;


    public int Ammo;
    public int Coin;
    public int Heart;

    public int MaxAmmo;
    public int MaxCoin;
    public int MaxHeart;
    public int MaxHasGrenades;

    int equipWeaponIndex = -1;

    public Rigidbody rb;

    bool wDown;
    bool sDown;
    bool jDown;
    bool eDown;
    bool aDown;
    bool rDown;
    bool gDown;

    bool swapDown1;
    bool swapDown2;
    bool swapDown3;
    bool isJump;
    bool isDodge;
    bool isSwap;
    bool isFireReady = true;
    bool isReload;
    bool isDie;
    bool isDamage;

    float hAxis;
    float vAxis;
    float fireDelay;


    Vector3 moveVec;
    Vector3 dodgeVec;

    Animator anim;
    GameObject nearObject;
    Weapon equipWeapon;
    MeshRenderer[] meshs;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        meshs = GetComponentsInChildren<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Dodge();
        Equip();
        Swap();
        Attack();
        Reload();
        Die();
        Grenade();
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        sDown = Input.GetButton("Walk");
        jDown = Input.GetButtonDown("Jump");
        eDown = Input.GetButtonDown("Equip");
        swapDown1 = Input.GetButtonDown("Swap1");
        swapDown2 = Input.GetButtonDown("Swap2");
        swapDown3 = Input.GetButtonDown("Swap3");
        aDown = Input.GetButton("Fire1");
        gDown = Input.GetButtonDown("Fire2");
        rDown = Input.GetButtonDown("Reload");

    }

    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        if (isDodge)
        {
            moveVec = dodgeVec;
        }

        if (isSwap || !isFireReady || isReload || isDie)
        {
            moveVec = Vector3.zero;
        }

        transform.position += moveVec * moveSpeed *(sDown ? 0.3f : 1f) * Time.deltaTime;

        anim.SetBool("isRun",moveVec!= Vector3.zero);
        anim.SetBool("isWalk",sDown);
        
    }

    void Turn()
    {
        transform.LookAt(transform.position + moveVec);

        if (aDown)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;

            if (Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 0;
                transform.LookAt(transform.position + nextVec);
            }
        }
    }

    void Jump()
    {
        if (jDown && moveVec == Vector3.zero && !isJump && !isDodge)
        {
            
            rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            isJump = true;
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
        }
    }

    void Dodge()
    {
        if (jDown && moveVec!= Vector3.zero && !isJump && !isDodge)
        {
            dodgeVec = moveVec;

            moveSpeed *= 2;
            isDodge = true;
            anim.SetTrigger("doDodge");
            

            Invoke("DodgeOut", 0.4f);
        }
        
    }

    void DodgeOut()
    {
        moveSpeed *= 0.5f;
        isDodge = false;
    }

    void Equip()
    {

        if (eDown && nearObject != null && !isJump && nearObject.tag == "Weapon")
        {
           
             Item item = nearObject.GetComponent<Item>();
             int weaponIndex = item.value;
             hasWeapon[weaponIndex] = true;

             Destroy(nearObject);
           
        }
    }

    void Swap()
    {
        // 무기를 끼고있다면
        if (swapDown1 && equipWeaponIndex == 0)
            return;
        if (swapDown2 && equipWeaponIndex == 1)
            return;
        if (swapDown3 && equipWeaponIndex == 2)
            return;

        int weaponIndex = -1;

        if (swapDown1) weaponIndex = 0;
        if (swapDown2) weaponIndex = 1;
        if (swapDown3) weaponIndex = 2;


        if ((swapDown1 || swapDown2 || swapDown3) && !isJump && !isDodge && hasWeapon[weaponIndex])
        {
            anim.SetTrigger("doSwap");
            
            isSwap = true;

            Invoke("SwapOut", 0.4f);

            equipWeaponIndex = weaponIndex;

            if (equipWeapon!= null)
            {
                equipWeapon.gameObject.SetActive(false);
            }
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true);
           
        }

    }

    void SwapOut()
    {
        isSwap = false;
    }

    void Attack()
    {
        if (equipWeapon == null)
            return;

        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.attackSpeed < fireDelay;

        if (aDown && isFireReady && !isDodge && !isSwap )
        {
            if (equipWeapon.type == Weapon.Type.Range && equipWeapon.curAmmo == 0)
                return;   
                
            equipWeapon.Use();
            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot");


            if (equipWeapon.type == Weapon.Type.Range)
            {
                //반동
                Vector3 reboundVec = transform.position - equipWeapon.bulletPos.position;
                rb.AddForce(reboundVec * equipWeapon.reboundForce, ForceMode.Impulse);
                rb.velocity = Vector3.zero;
            }

            fireDelay = 0;
        }
    }

    public void Reload()
    {
        if (equipWeapon == null)
            return;
        if (equipWeapon.type == Weapon.Type.Melee)
            return;
        if (Ammo == 0)
            return;

        if (rDown && !isJump && !isDodge && !isSwap && isFireReady)
        {
            isReload = true;
            anim.SetTrigger("doReload");

            Invoke("ReloadOut", 3f);
        }
    }

    void ReloadOut()
    {
        int reAmmo = Ammo < equipWeapon.maxAmmo ? Ammo : equipWeapon.maxAmmo;
        equipWeapon.curAmmo = reAmmo;
        Ammo -= reAmmo;
        isReload = false;
    }

    void FreezeRotation()
    {
        rb.angularVelocity = Vector3.zero;
    }

    private void FixedUpdate()
    {
        FreezeRotation();
    }

    void Die()
    {
        if (Heart <= 0)
        {

            isDie = true;
            StartCoroutine(restart());
            
        }
    }

    IEnumerator restart()
    {
        transform.gameObject.layer = 11;
        anim.SetTrigger("Die");

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene("MainScene");

    }

    void Grenade()
    {
        if (HasGrenades == 0)
            return;

        if (gDown && !isJump && !isSwap)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;

            if (Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 8;

                GameObject grenade = Instantiate(grenadeObject, transform.position, transform.rotation);
                Rigidbody rbGrenade = grenade.GetComponent<Rigidbody>();
                rbGrenade.AddForce(nextVec, ForceMode.Impulse);
                rbGrenade.AddTorque(Vector3.back * 10, ForceMode.Impulse);

                HasGrenades--;
                grenades[HasGrenades].SetActive(false);
            }

            
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon")
        {
            nearObject = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
        {
            nearObject = null;
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name == "Floor")
        {
            anim.SetBool("isJump", false);
            isJump = false;
        }

        if (collision.gameObject.tag == "Enemy")
        {
            Heart -= 1;
            Vector3 reactVec = (transform.position - collision.transform.position).normalized;
            reactVec += Vector3.up;
            rb.AddForce(reactVec * 10, ForceMode.Impulse);
            rb.velocity = Vector3.zero;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item")
        {
            Destroy(other.gameObject);

            Item item = other.GetComponent<Item>();

            switch (item.type)
            {
                case Item.Type.Ammo:
                    Ammo += item.value;
                    if (Ammo > MaxAmmo)
                        Ammo = MaxAmmo;
                    break;
                case Item.Type.Coin:
                    Coin += item.value;
                    if (Coin > MaxCoin)
                        Coin = MaxCoin;
                    break;
                case Item.Type.Heart:
                    Heart += item.value;
                    if (Heart > MaxHeart)
                        Heart = MaxHeart;
                    break;
                case Item.Type.Grenade:
                    grenades[HasGrenades].SetActive(true);
                    HasGrenades += item.value;
                    if (HasGrenades > MaxHasGrenades)
                        HasGrenades = MaxHasGrenades;
                    break;
            }
            
        }
        else if (other.tag== "EnemyBullet")
        {
            if (!isDamage)
            {
                Bullet enemyBullet = other.GetComponent<Bullet>();
                Heart -= enemyBullet.damage;
                StartCoroutine(onDamage());


            }
            
        }
    }

    IEnumerator onDamage()
    {
        isDamage = true;

        foreach(MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.yellow;
        }
        yield return new WaitForSeconds(1f);

        isDamage = false;
        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.white;
        }
    }
}
