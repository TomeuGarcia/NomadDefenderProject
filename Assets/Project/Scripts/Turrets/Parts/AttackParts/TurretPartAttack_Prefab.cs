using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TurretPartAttack_Prefab;

public delegate int PassiveDamageModifier(int damage, HealthSystem healthSystem);
public class TurretPartAttack_Prefab : MonoBehaviour
{
    public enum AttackType { BASIC, TESLA, LONG_RANGE, PIERCING }

    protected Enemy targetEnemy;
    protected int damage;
    [SerializeField] protected AttackType attackType;
    [SerializeField] protected float bulletSpeed = 2.0f;

    [SerializeField] public Material materialForTurret;


    protected PassiveDamageModifier passiveDamageModifier;

    protected bool disappearing = false;

    protected void Awake()
    {
        SetPassiveDamageModifier(DefaultDamage);
    }

    public int DefaultDamage(int damage, HealthSystem healthSystem)
    {
        return damage;
    }

    protected void SetPassiveDamageModifier(PassiveDamageModifier newPassiveDamageModifier)
    {
        passiveDamageModifier = newPassiveDamageModifier;
    }

    public virtual void Init(Enemy targetEnemy, TurretBuilding owner)
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

    protected void Disable()
    {
        SetPassiveDamageModifier(DefaultDamage);

        gameObject.SetActive(false);

        disappearing = false;
    }
}
