using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingProjectile : TurretAttack
{
    private Enemy targetEnemy;
    private int damage;

    private Vector3 moveDirection;

    [SerializeField] private float speed;

    private void Update()
    {
        moveDirection = targetEnemy.Position - transform.position;

        transform.position = transform.position + (moveDirection.normalized * (speed * Time.deltaTime));

        //IF ROTATION IS NEEDED
        //Quaternion lookRotation = Quaternion.LookRotation(direction);
        //transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    override public void Init(Enemy targetEnemy, int damage)
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
                StopAllCoroutines();
                Disappear();
            }
        }
    }

    private IEnumerator Lifetime()
    {
        yield return new WaitForSeconds(4f);
        Disappear();
    }

    private void Disappear()
    {
        //PARTICLES
        gameObject.SetActive(false);
    }
}
