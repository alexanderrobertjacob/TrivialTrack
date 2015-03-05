using UnityEngine;
using System.Collections;

public class HealthScript : MonoBehaviour {

    public bool isEnemy = true;
    public float health = 2;


    public void Damage(float incomingDamage)
    {
        health -= incomingDamage;
        if (health <= 0)
        {
            if (health <= 0 && this.gameObject.tag == "Player")
            {
                GameObject.Find("PlayerGrunt").GetComponent<AudioSource>().Play();
                Application.LoadLevel(Application.loadedLevel);
            }
            else
            {
                Destroy(this.gameObject.GetComponentInParent<Rigidbody2D>().gameObject);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D otherCollider)
    {
        //DamageScript attack = otherCollider.GetComponent<DamageScript>();
        //if (attack != null)
        //{
        //    if (attack.isEnemyAttack != isEnemy)
        //    {
        //        Debug.Log("It's a hit!");
        //        Destroy(attack.gameObject);
        //        Destroy(this.gameObject);

        //    }
        //}

        WeaponScript incomingWeapon = otherCollider.gameObject.GetComponent<WeaponScript>();
        if (incomingWeapon != null)
        {
            //Avoid friendly fire
            if (incomingWeapon.isEnemyAttack != isEnemy)
            {
                Damage(incomingWeapon.damage);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D otherCollider)
    {

        WeaponScript incomingWeapon = otherCollider.gameObject.GetComponent<WeaponScript>();
        if (incomingWeapon != null)
        {
            //Avoid friendly fire
            if (incomingWeapon.isEnemyAttack != isEnemy)
            {
                Damage(incomingWeapon.damage);
            }
        }

    }
}
