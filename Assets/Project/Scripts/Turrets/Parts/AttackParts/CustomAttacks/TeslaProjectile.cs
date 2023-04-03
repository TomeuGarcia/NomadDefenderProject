using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeslaProjectile : TurretPartAttack_Prefab
{
    [SerializeField] private Lerp lerp;

    [SerializeField] private LayerMask enemyLayerMask;
    [SerializeField, Range(0f, 1f)] private float damageMultiplier = 0.5f;
    [SerializeField] private int maxTargetAmount = 2;
    [SerializeField] private float range;
    private int currentTarget;
    private Enemy[] targetedEnemies;
    private bool attackChained = false;

    protected override void DoUpdate()
    {
    }

    public override void ProjectileShotInit(Enemy targetEnemy, TurretBuilding owner)
    {      
        this.damage = owner.stats.damage;
        this.damage = (int)((float)this.damage * damageMultiplier);

        attackChained = false;

        targetedEnemies = owner.GetNearestEnemies(maxTargetAmount, range);
        for (int i = 0; i < targetedEnemies.Length; i++)
        {
            targetedEnemies[i].QueueDamage(damage, passiveDamageModifier);
        }
        currentTarget = 0;
        this.targetEnemy = targetedEnemies[currentTarget];

        if (owner.baseDamagePassive != null)
            SetPassiveDamageModifier(owner.baseDamagePassive);

        lerp.LerpPosition(targetEnemy.MeshTransform, bulletSpeed);
        StartCoroutine(WaitForLerpFinish());
    }

    IEnumerator WaitForLerpFinish()
    {
        yield return new WaitUntil(() => lerp.finishedPositionLerp == true);
        EnemyHit();
    }

    void EnemyHit()
    {
        GameObject temp = ProjectileParticleFactory.GetInstance().GetAttackParticlesGameObject(attackType, targetEnemy.MeshTransform.position, Quaternion.identity);
        temp.gameObject.SetActive(true);
        temp.transform.parent = gameObject.transform.parent;

        targetEnemy.TakeDamage(damage, passiveDamageModifier);

        if (++currentTarget < targetedEnemies.Length)
        {
            Debug.Log(currentTarget);
            targetEnemy = targetedEnemies[currentTarget];

            lerp.LerpPosition(targetedEnemies[currentTarget].MeshTransform, bulletSpeed / 2.0f);
            StartCoroutine(WaitForLerpFinish());
        }
        else
        {
            //StopAllCoroutines();
            Disappear();
        }
    }


    Enemy[] GetNearestEnemiesToTargetedEnemy(Enemy targetedEnemy, int maxEnemies, float radius)
    {       
        Collider[]  colliders = Physics.OverlapSphere(targetedEnemy.Position, radius, enemyLayerMask, QueryTriggerInteraction.Collide);

        if (colliders.Length == 0) return new Enemy[0];


        List<Enemy> enemies = new List<Enemy>(colliders.Length);

        for (int collidersI = 0; collidersI < colliders.Length; ++collidersI)
        {
            Enemy enemy = colliders[collidersI].gameObject.GetComponent<Enemy>();
            enemies.Add(enemy);
        }


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
