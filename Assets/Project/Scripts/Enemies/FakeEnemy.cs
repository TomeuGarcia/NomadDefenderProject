using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeEnemy : Enemy
{
    private bool canBeTargeted = true;

    public delegate void FakeEnemyAction(TurretPartAttack_Prefab projectile);
    public event FakeEnemyAction OnAttackedByProjectile;


    private void Awake()
    {
        healthSystem = new HealthSystem(1000);

        pathFollower.paused = true;
        pathFollower.enabled = false;

        MeshTransform.gameObject.SetActive(false);
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


    public override void TakeDamage(TurretPartAttack_Prefab projectileDamageDealer_Ref, int damageAmount, PassiveDamageModifier modifier)
    {
        if (projectileDamageDealer_Ref != null && OnAttackedByProjectile != null) OnAttackedByProjectile(projectileDamageDealer_Ref);
        return;
    }

    public override void GetStunned(float duration)
    {
        return;
    }

    public override int QueueDamage(int amount, PassiveDamageModifier modifier)
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
