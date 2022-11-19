using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretAttack : MonoBehaviour
{
    public enum AttackType { BASIC, TESLA, LONG_RANGE }

    protected Enemy targetEnemy;
    protected int damage;
    protected Vector3 moveDirection;
    protected Collider lastHit;
    [SerializeField] protected AttackType attackType;
    [SerializeField] protected float moveSpeed = 5f;
    [SerializeField] protected float rotationSpeed;
    [SerializeField] protected float lifetime = 1f;

    [SerializeField] public Material materialForTurret;
    [SerializeField] public Collider attackCollider;

    protected bool disappearing = false;

    public virtual void Init(Enemy targetEnemy, Turret owner) 
    {
    }

    protected virtual void DoUpdate() 
    {
    }

    protected virtual void OnEnemyTriggerEnter(Enemy enemy)
    {
    }

    void Update()
    {
        if (!disappearing)
        {
            DoUpdate();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            lastHit = other;
            OnEnemyTriggerEnter(other.GetComponent<Enemy>());
        }
    }

    protected IEnumerator Lifetime()
    {
        yield return new WaitForSeconds(lifetime);
        Debug.Log("PROJECTILE DISABLED");
        Disable();
    }

    protected virtual void Disappear()
    {
        StartCoroutine(WaitToDisable());
    }

    private IEnumerator WaitToDisable()
    {
        disappearing = true;
        attackCollider.enabled = false;

        yield return new WaitForSeconds(0.5f);
        Disable();
    }

    private void Disable()
    {
        gameObject.SetActive(false);

        disappearing = false;
        attackCollider.enabled = true;
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
