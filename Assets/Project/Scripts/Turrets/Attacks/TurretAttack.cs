using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretAttack : MonoBehaviour
{
    protected Enemy targetEnemy;
    protected int damage;
    protected Vector3 moveDirection;
    [SerializeField] protected float moveSpeed = 5f;
    [SerializeField] protected float rotationSpeed;
    [SerializeField] protected float lifetime = 1f;

    [SerializeField] public Material materialForTurret;

    public virtual void Init(Enemy targetEnemy, Turret owner) 
    {
    }

    protected virtual void OnEnemyTriggerEnter(Enemy enemy)
    {
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            OnEnemyTriggerEnter(other.GetComponent<Enemy>());
        }
    }

    protected IEnumerator Lifetime()
    {
        yield return new WaitForSeconds(lifetime);
        Disappear();
    }

    protected virtual void Disappear()
    {
        //PARTICLES
        gameObject.SetActive(false);
    }


    protected void MoveTowardsEnemyTarget()
    {
        moveDirection = targetEnemy.Position - transform.position;
        transform.position = transform.position + (moveDirection.normalized * (moveSpeed * Time.deltaTime));
    }
    
    protected void RotateTowardsEnemyTarget()
    {
        Quaternion lookRotation = Quaternion.LookRotation(moveDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }


}
