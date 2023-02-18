using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;

    Rigidbody rigid;
    BoxCollider boxCollider;
    Material material;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        material = GetComponent<MeshRenderer>().material;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            currentHealth -= weapon.damage;
            StartCoroutine(OnDamage());
        }   
        else if(other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            currentHealth -= bullet.damage;
            StartCoroutine(OnDamage());
        }
    }
    IEnumerator OnDamage()
    {
        material.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        if(currentHealth > 0)
        {
            material.color = Color.white;
        }
        else
        {
            material.color = Color.gray;
            Destroy(gameObject, 4);
        }

    }
}
