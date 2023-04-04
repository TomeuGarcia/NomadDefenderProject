using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TurretPartAttack_Prefab;

public delegate int PassiveDamageModifier(int damage, HealthSystem healthSystem);
public class TurretPartAttack_Prefab : MonoBehaviour
{
    public enum AttackType { BASIC, TESLA, LONG_RANGE, CLOSE_RANGE, PIERCING }

    protected Enemy targetEnemy;
    protected int damage;
    [SerializeField] protected AttackType attackType;
    public AttackType GetAttackType => attackType;
    [SerializeField] protected float bulletSpeed = 2.0f;

    [SerializeField] public Material materialForTurret;

    public Vector3 Position => transform.position;


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

    public virtual void ProjectileShotInit(Enemy targetEnemy, TurretBuilding owner)
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


    protected Enemy[] GetNearestEnemiesToTargetedEnemy(Enemy targetedEnemy, int maxEnemies, float radius, LayerMask enemyLayerMask)
    {
        Collider[] colliders = Physics.OverlapSphere(targetedEnemy.Position, radius, enemyLayerMask, QueryTriggerInteraction.Collide);


        List<Enemy> enemies = new List<Enemy>(colliders.Length);

        for (int collidersI = 0; collidersI < colliders.Length; ++collidersI)
        {
            Enemy enemy = colliders[collidersI].gameObject.GetComponent<Enemy>();

            if (enemy != targetedEnemy && !enemy.DiesFromQueuedDamage())
            {
                enemies.Add(enemy);
            }
        }

        if (enemies.Count == 0) return enemies.ToArray();

        enemies.Sort(SortByClosestToProjectile);

        Enemy[] nearestEnemies = new Enemy[Mathf.Min(maxEnemies, enemies.Count)];
        for (int i = 0; i < nearestEnemies.Length; ++i)
        {
            nearestEnemies[i] = enemies[i];
        }

        return nearestEnemies;
    }


    public int SortByClosestToProjectile(Enemy e1, Enemy e2)
    {
        return Vector3.Distance(e1.Position, Position).CompareTo(Vector3.Distance(e2.Position, Position));
    }

}
