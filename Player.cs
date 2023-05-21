using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    public GameObject[] weapons;
    public bool[] hasWeapon;
    public GameObject[] grenades;
    public int HasGrenades;

    public float jumpForce;

    public int Ammo;
    public int Coin;
    public int Heart;
    
    public int MaxAmmo;
    public int MaxCoin;
    public int MaxHeart;
    public int MaxHasGrenades;


    int equipWeaponIndex = -1;
    
    float hAxis;
    float vAxis;

    bool walkDown;
    bool jumpDown;
    bool iDown;
    bool isJump;
    bool isDodge;
    bool isSwap;

    bool swapDown1;
    bool swapDown2;
    bool swapDown3;

    Rigidbody rb;

    Vector3 moveVec;
    Vector3 dodgeVec;

    GameObject nearObject;
    GameObject equipWeapon;

    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // 자식오브젝트에 있는 컴포넌트 가져오기.
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        GetInput();
        Move();
        Turn();
        Jump();
        Dodge();
        Interaction();
        Swap();
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        walkDown = Input.GetButton("Walk");
        jumpDown = Input.GetButtonDown("Jump");
        iDown = Input.GetButtonDown("Interaction");
        swapDown1 = Input.GetButtonDown("Swap1");
        swapDown2 = Input.GetButtonDown("Swap2");
        swapDown3 = Input.GetButtonDown("Swap3");
    }

    void Move()
    {
        // normalized > 대각선 방향으로도 1로 움직이게 만듦
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        // 회피 중에는 그 방향 그대로 유지
        if (isDodge)
        {
            moveVec = dodgeVec;
        }

        // 무기 교체 중에는 움직이지 않도록
        if (isSwap)
        {
            moveVec = Vector3.zero;
        }
        transform.position += moveVec * speed * (walkDown ? 0.3f : 1f) * Time.deltaTime;

        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", walkDown);
    }

    void Turn()
    {
        transform.LookAt(transform.position + moveVec);
    }

    void Jump()
    {
        if (jumpDown && moveVec == Vector3.zero && !isJump && !isDodge && !isSwap)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isJump = true;
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
        }
    }

    void Dodge()
    {
        if (jumpDown && moveVec != Vector3.zero && !isJump && !isDodge && !isSwap)
        {
            dodgeVec = moveVec;

            speed *= 2;
            anim.SetTrigger("doDodge");
            isDodge = true;

            // 시간차를 두고 함수 실행
            Invoke("DodgeOut", 0.4f);

        }
    }

    void Swap()
    {

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
            
            if (equipWeapon != null) {
                equipWeapon.SetActive(false);
            }
            equipWeapon = weapons[weaponIndex];
            equipWeapon.SetActive(true);
            
        }
    }

    void SwapOut()
    {
        isSwap = false;
    }


    void Interaction()
    {
        if (iDown && nearObject != null && !isJump)
        {
            if(nearObject.tag == "Weapon")
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapon[weaponIndex] = true;

                Destroy(nearObject);
            }
        }
    }

    void DodgeOut()
    {

        speed *= 0.5f;
        isDodge = false;

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            anim.SetBool("isJump", false);
            isJump = false;
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

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Item")
        {
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
                    if (HasGrenades == MaxHasGrenades)
                        return;
                    grenades[HasGrenades].SetActive(true);
                    HasGrenades += item.value;
                    break;
            }
            Destroy(other.gameObject);
        }
    }

}



    
