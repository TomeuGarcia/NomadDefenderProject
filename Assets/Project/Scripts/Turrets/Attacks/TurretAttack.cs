using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretAttack : MonoBehaviour
{
    public enum AttackType { BASIC, TESLA, LONG_RANGE }

    protected Enemy targetEnemy;
    protected int damage;
    [SerializeField] protected AttackType attackType;
    [SerializeField] protected float bulletSpeed = 2.0f;

    [SerializeField] public Material materialForTurret;

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

    protected virtual void Disappear()
    {
        StartCoroutine(WaitToDisable());
    }

    private IEnumerator WaitToDisable()
    {
        disappearing = true;

        yield return new WaitForSeconds(0.5f);
        Disable();
    }

    private void Disable()
    {
        gameObject.SetActive(false);

        disappearing = false;
    }
}
