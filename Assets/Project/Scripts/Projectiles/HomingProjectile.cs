using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingProjectile : MonoBehaviour
{
    private Enemy targetEnemy;
    private int damage;

    private Vector3 moveDirection;


    private void Update()
    {
        moveDirection = targetEnemy.Position - transform.position;

        transform.position = transform.position + (moveDirection * (20f * Time.deltaTime));
    }

    public void Init(Enemy targetEnemy, int damage)
    {
        this.targetEnemy = targetEnemy;
        this.damage = damage;

        StartCoroutine(Lifetime());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy == targetEnemy)
            {
                enemy.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }

    private IEnumerator Lifetime()
    {
        yield return new WaitForSeconds(4f);
        Destroy(gameObject);
    }


}
