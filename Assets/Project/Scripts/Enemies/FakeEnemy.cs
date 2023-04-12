using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeEnemy : Enemy
{
    private bool canBeTargeted = true;

    public delegate void FakeEnemyAction(TurretPartAttack_Prefab projectile);
    public event FakeEnemyAction OnAttackedByProjectile;

    public delegate void FakeEnemyComputeDamageAction(int damageAmount, PassiveDamageModifier modifier, out int resultDamage, TurretPartAttack_Prefab projectileSource);
    public event FakeEnemyComputeDamageAction OnDamageCompute;
    


    private void Awake()
    {
        healthSystem = new HealthSystem(1000);

        pathFollower.paused = true;
        pathFollower.enabled = false;

        MeshTransform.gameObject.SetActive(false);

        IsFakeEnemy = true;
    }


    public void SetCanBeTargeted(bool canBeTargeted)
    {
        this.canBeTargeted = canBeTargeted;
    }

    public override bool CanBeTargeted()
    {
        return canBeTargeted;
    }
    public override float GetTargetNegativePriorityBonus()
    {
        return 10000f;
    }

    public override void GetStunned(float duration)
    {
        return;
    }

    public override int ComputeDamageWithPassive(TurretPartAttack_Prefab projectileSource, int damageAmount, PassiveDamageModifier modifier)
    {
        int resultDamage = 0;
        if (OnDamageCompute != null) OnDamageCompute(damageAmount, modifier, out resultDamage, projectileSource);
        
        return resultDamage;
    }

    public override void TakeDamage(TurretPartAttack_Prefab projectileSource, int damageAmount)
    {
        if (projectileSource != null && OnAttackedByProjectile != null) OnAttackedByProjectile(projectileSource);
        return;
    }


    public override int QueueDamage(int amount)
    {
        return amount;
    }

    public override void RemoveQueuedDamage(int amount)
    {
        return;
    }

    public override bool DiesFromQueuedDamage()
    {
        return false;
    }

    public override void SetMoveSpeed(float speedCoef)
    {
        return;
    }

    public override void ApplyWaveStatMultiplier(float multiplier)
    {
        return;
    }

    public override bool IsDead()
    {
        return false;
    }

    public override void AddHealth(int healthToAdd)
    {
        return;
    }

    public override void AddArmor(int armorToAdd)
    {
        return;
    }
}
