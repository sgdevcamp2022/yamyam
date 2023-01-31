using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    public GameObject[] weapons;
    public bool[] hasWeapons;

    float horizonAxis;
    float verticalAxis;
    bool walkDown;
    bool jumpDown;
    bool interactDown;

    bool isJump;
    bool isDodge;
    

    Vector3 moveVector;
    Vector3 dodgeVector;


    Rigidbody rigid;
    Animator anime;

    GameObject nearObject;


    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anime = GetComponentInChildren<Animator>();   
    }

    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Dodge();
        Interaction();
    }

    void GetInput()
    {
        horizonAxis = Input.GetAxisRaw("Horizontal");
        verticalAxis = Input.GetAxisRaw("Vertical");
        walkDown = Input.GetButton("Walk");
        jumpDown = Input.GetButtonDown("Jump");
        interactDown = Input.GetButtonDown("Interaction");
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
        transform.LookAt(transform.position + moveVector);
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
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            anime.SetBool("isJump", false);
            isJump = false;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = other.gameObject;

        Debug.Log(nearObject.name);
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
            nearObject = null;
    }
}
