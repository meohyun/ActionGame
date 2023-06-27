using System.Collections;
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
    public bool isRestart;


    public int Ammo;
    public int Coin;
    public int Health;
    public int Life;
    public int score;

    public int MaxAmmo;
    public int MaxCoin;
    public int MaxHealth;
    public int MaxLife;
    public int MaxHasGrenades;

    int equipWeaponIndex = -1;

    public Rigidbody rb;

    bool sDown;
    bool jDown;
    bool eDown;
    bool aDown;
    bool rDown;
    bool gDown;
    bool backDown;
    bool isShop;
    bool isBorder;

    bool swapDown1;
    bool swapDown2;
    bool swapDown3;
    bool isJump;
    bool isDodge;
    bool isSwap;
    public bool isFireReady = true;
    bool isReload;
    public bool isDie;
    public bool isDamage;

    float hAxis;
    float vAxis;
    float fireDelay;

    Vector3 moveVec;
    Vector3 dodgeVec;

    public Animator anim;
    public GameObject grenadeGroup;
    GameObject nearObject;
    GameObject GM;

    GameManager manager;
    public Weapon equipWeapon;


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

    private void LateUpdate()
    {
        GM = GameObject.FindWithTag("Manager");
        manager = GM.GetComponentInChildren<GameManager>();

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
        backDown = Input.GetButtonDown("back");

    }

    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        if (isDodge)
        {
            moveVec = dodgeVec;
        }

        if (isSwap || !isFireReady || isDie)
        {
            moveVec = Vector3.zero;
        }

        if (!isBorder)
            transform.position += moveVec * moveSpeed * (isReload ? 0.5f :(sDown ? 0.3f : 1f))* Time.deltaTime;

        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", sDown);

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
        // 뒤돌 
        if (backDown)
        {
            Quaternion rot_Y = Quaternion.Euler(0, 180, 0);

            transform.rotation *= rot_Y;
            transform.LookAt(transform.position);
        }

    }

    void Jump()
    {
        if (jDown && moveVec == Vector3.zero && !isJump && !isDodge && !isReload)
        {

            rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            isJump = true;
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
        }
    }

    void Dodge()
    {
        if (jDown && moveVec != Vector3.zero && !isJump && !isDodge && !isReload)
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

        if (eDown && nearObject != null && !isJump)
        {
            if (nearObject.tag == "Weapon")
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapon[weaponIndex] = true;

                Destroy(nearObject);
            }

            else if (nearObject.tag == "Shop")
            {
                Shop shop = nearObject.GetComponent<Shop>();
                shop.Enter(this);
                isShop = true;
            }

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


        if ((swapDown1 || swapDown2 || swapDown3) && !isJump && !isDodge && !isSwap && hasWeapon[weaponIndex] )
        {
            anim.SetTrigger("doSwap");

            isSwap = true;

            Invoke("SwapOut", 0.4f);

            equipWeaponIndex = weaponIndex;

            if (equipWeapon != null)
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

        if (aDown && isFireReady && !isDodge && !isSwap && !isShop && !isJump)
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
        if (equipWeapon.curAmmo == equipWeapon.maxAmmo)
            return;

        if (rDown && !isJump && !isDodge && !isSwap && isFireReady && !isShop)
        {
            isReload = true;
            anim.SetTrigger("doReload");

            Invoke("ReloadOut", 3f);
        }
    }

    void ReloadOut()
    {
        int reAmmo = Ammo < equipWeapon.maxAmmo ? Ammo : equipWeapon.maxAmmo-equipWeapon.curAmmo;
        equipWeapon.curAmmo = equipWeapon.maxAmmo;
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
        stopToWall();
    }

    void Die()
    {
        if (Health <= 0 && !isDie)
        {
            isDie = true;
            Life -= 1;

            if (Life > 0)
                StartCoroutine(restart());
            else
                StartCoroutine(manager.GameOver());
        }
    }


    // 목숨이 0개가 아니라면 다시 시작
    public IEnumerator restart()
    {
        transform.gameObject.layer = 11;
        anim.SetTrigger("Die");
        
        // 사망시 플레이어 및 수류탄 그룹 제
        yield return new WaitForSeconds(1.5f);
        transform.gameObject.SetActive(false);
        grenadeGroup.SetActive(false); 

        // 살아나는 위치 초기화 및 다시 씬 불러옴
        transform.gameObject.layer = 3;
        Health = 100;
        Ammo = 300;
        if (equipWeapon != null)
            equipWeapon.curAmmo = equipWeapon.maxAmmo;
        isDie = false;
        rb.velocity = Vector3.zero;
        transform.position = new Vector3(0, 0, 0);
        SceneManager.LoadScene("Stage_" + manager.stage.ToString());

        transform.gameObject.SetActive(true);
        grenadeGroup.SetActive(true);
    }

    void Grenade()
    {
        if (HasGrenades == 0)
            return;

        if (gDown && !isJump && !isSwap && !isShop)
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

    void stopToWall()
    {
        isBorder = Physics.Raycast(transform.position, transform.forward,1,LayerMask.GetMask("GameObject"));
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon" || other.tag =="Shop")
        {
            nearObject = other.gameObject;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Shop")
        {
            Shop shop = nearObject.GetComponent<Shop>();
            shop.Exit();
            isShop = false;
            nearObject = null;
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            anim.SetBool("isJump", false);
            isJump = false;
        }

        if (collision.gameObject.tag == "Enemy")
        {
            Health -= 5;
            StartCoroutine(onDamage(false));
        }


        if (collision.gameObject.tag == "WorldObject")
            rb.velocity = Vector3.zero;

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
                case Item.Type.Life:
                    Life += item.value;
                    break;
                case Item.Type.Grenade:
                    grenades[HasGrenades].SetActive(true);
                    HasGrenades += item.value;
                    if (HasGrenades > MaxHasGrenades)
                        HasGrenades = MaxHasGrenades;
                    break;
            }
            
        }
        if (other.tag== "EnemyBullet")
        {
            if (!isDamage)
            {
                Bullet enemyBullet = other.GetComponent<Bullet>();
                Health -= enemyBullet.damage;
                bool isBossAtk = other.name == "Boss Melee Area";
                StartCoroutine(onDamage(isBossAtk));
            }  
        }
    }

    IEnumerator onDamage(bool isBossAtk)
    {
        isDamage = true;

        if (isDamage)
            foreach(MeshRenderer mesh in meshs)
            {
                mesh.material.color = Color.yellow;
            }
        yield return new WaitForSeconds(1f);

        if (isBossAtk)
            rb.AddForce(transform.forward * -25 , ForceMode.Impulse);

        yield return new WaitForSeconds(1f);

        if (isBossAtk)
            rb.velocity = Vector3.zero;

        isDamage = false;

        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.white;
        }
    }
}
