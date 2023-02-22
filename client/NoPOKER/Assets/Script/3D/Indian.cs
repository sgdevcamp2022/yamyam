using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indian : MonoBehaviour
{
    public float speed;
    public GameObject[] weapons;
    public bool[] hasWeapons;

    public Camera followCamera;
    

    public int ammo;
    public int maxAmmo;
    public int health;
    public int maxHealth;

    float horizonAxis;
    float verticalAxis;

    bool walkDown;
    bool jumpDown;
    bool fireDown;
    bool reloadDown;
    bool interactDown;
    bool swapDown1;
    bool swapDown2;
    bool swapDown3;

    bool isJump;
    bool isDodge;
    bool isSwap;
    bool isReload;
    bool isFireReady;
    bool isDamage;

    Vector3 moveVector;
    Vector3 dodgeVector;


    Rigidbody rigid;
    Animator anime;
    MeshRenderer[] meshes;

    GameObject nearObject;
    Weapon equipWeapon;
    int equipWeaponIndex = -1;
    float fireDelay;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anime = GetComponentInChildren<Animator>();
        meshes = GetComponentsInChildren<MeshRenderer>();
    }

    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Attack();
        Reload();
        Dodge();
        Swap();
        Interaction();
    }

    void GetInput()
    {
        horizonAxis = Input.GetAxisRaw("Horizontal");
        verticalAxis = Input.GetAxisRaw("Vertical");
        walkDown = Input.GetButton("Walk");
        jumpDown = Input.GetButtonDown("Jump");
        fireDown = Input.GetButton("Fire1");
        reloadDown = Input.GetButtonDown("Reload");
        interactDown = Input.GetButtonDown("Interaction");
        swapDown1 = Input.GetButtonDown("Swap1");
        swapDown2 = Input.GetButtonDown("Swap2");
        swapDown3 = Input.GetButtonDown("Swap3");



    }

    void Move()
    {
        moveVector = new Vector3(horizonAxis, 0, verticalAxis).normalized;
        if (isDodge)
            moveVector = dodgeVector;

        transform.position += moveVector * speed * (walkDown ? 0.3f : 1f) * Time.deltaTime;

        anime.SetBool("isRun", moveVector != Vector3.zero);
        anime.SetBool("isWalk", walkDown);
    }

    void Turn()
    {
        // 키보드에 의한 회전
        transform.LookAt(transform.position + moveVector);
        // 마우스에 의한 회전
        if (fireDown) 
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 nextVector = rayHit.point - transform.position;
                nextVector.y = 0;
                transform.LookAt(transform.position + nextVector);
            }
        }
        
    }

    void Jump()
    {
        if(jumpDown && moveVector == Vector3.zero && !isJump &&!isDodge)
        {
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);
            anime.SetBool("isJump", true);
            anime.SetTrigger("doJump");
            isJump = true;
        }
    }

    void Attack()
    {
        if (equipWeapon == null)
            return;

        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.rate < fireDelay;

        if(fireDown && isFireReady && !isDodge && !isSwap)
        {
            equipWeapon.Use();
            anime.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot");
            fireDelay = 0;
        }
    }

    void Reload()
    {
        if (equipWeapon == null)
            return;
        if (equipWeapon.type == Weapon.Type.Melee)
            return;
        if (ammo == 0)
            return;
        if(reloadDown && !isJump && !isDodge && !isSwap && isFireReady)
        {
            anime.SetTrigger("doReload");
            isReload = true;
            Invoke("ReloadOut", 2f);
        }
    }
        
    void ReloadOut()
    {
        int reAmmo = ammo < equipWeapon.maxAmmo ? ammo : equipWeapon.maxAmmo;
        equipWeapon.currentAmmo = reAmmo;
        ammo -= reAmmo;
        isReload = false;
    }
    void Dodge()
    {
        if (jumpDown && moveVector != Vector3.zero && !isJump && !isDodge)
        {
            dodgeVector = moveVector;
            speed *= 2;
            anime.SetTrigger("doDodge");
            isDodge = true;

            Invoke("DodgeOut", 0.4f);
        }
    }

    void DodgeOut()
    {
        speed *= 0.5f;
        isDodge = false;
    }

    void Swap()
    {
        int weaponIndex = -1;
        if (swapDown1) weaponIndex = 0;
        if (swapDown2) weaponIndex = 1;
        if (swapDown3) weaponIndex = 2;
        if ((swapDown1 || swapDown2 || swapDown3) && !isJump && !isDodge)
        {
            if(equipWeapon != null)
                equipWeapon.gameObject.SetActive(false);

            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true);
        }
    }

    void Interaction()
    {
        if(interactDown && nearObject != null && !isJump && !isDodge)
        {
            if(nearObject.tag == "Weapon")
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;

                Destroy(nearObject);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            anime.SetBool("isJump", false);
            isJump = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            health -= weapon.damage;
            Vector3 reactVector = transform.position - other.transform.position;
            StartCoroutine(OnDamage());
        }
        else if (other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            health -= bullet.damage;
            Vector3 reactVector = transform.position - other.transform.position;
            Destroy(other.gameObject);
            StartCoroutine(OnDamage());
        }
        else if (other.tag == "EnemyBullet")
        {
            if (!isDamage)
            {
                Bullet enemyBullet = other.GetComponent<Bullet>();
                health -= enemyBullet.damage;
                StartCoroutine(OnDamage());
            }
        }
    }

    IEnumerator OnDamage()
    {
        isDamage = true;

        foreach(MeshRenderer mesh in meshes)
        {
            mesh.material.color = Color.yellow;
        }
        yield return new WaitForSeconds(1f);
        
        isDamage = false;

        foreach (MeshRenderer mesh in meshes)
        {
            mesh.material.color = Color.white;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = other.gameObject; 
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = null;
    }
}
