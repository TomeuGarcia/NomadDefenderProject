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

    public delegate void FakeEnemyGetPositionAction(out Vector3 position, out bool foundTarget);
    public event FakeEnemyGetPositionAction OnGetPosition;


    [SerializeField] private BoxCollider boxCollider;
    private Bounds colliderBounds;


    private void Awake()
    {
        healthSystem = new HealthSystem(1000);

        PathFollower.paused = true;
        PathFollower.enabled = false;

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
    public override float GetTargetPriorityBonus()
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

    public override void DoTakeDamage(TurretPartAttack_Prefab projectileSource, int damageAmount, out bool hitArmor)
    {
        hitArmor = false;
        if (projectileSource != null && OnAttackedByProjectile != null) OnAttackedByProjectile(projectileSource);
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

    public override Vector3 GetPosition()
    {
        Vector3 delegatedPosition = Vector3.zero;
        bool foundTarget = false;

        if (OnGetPosition != null) OnGetPosition(out delegatedPosition, out foundTarget);

        if (!foundTarget)
        {
            return base.GetPosition();
        }

        return delegatedPosition;
    }

    public void SetupColliderBounds()
    {
        colliderBounds = new Bounds(boxCollider.transform.position, boxCollider.size);
    }
    
    public Bounds GetColliderBounds()
    {
        colliderBounds.center = boxCollider.transform.position;
        return colliderBounds;
    }

}
