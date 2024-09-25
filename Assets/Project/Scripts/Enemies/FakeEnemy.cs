using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeEnemy : Enemy
{
    private bool canBeTargeted = true;

    public delegate void FakeEnemyAction(ATurretProjectileBehaviour projectile, Action<TurretDamageAttackResult> takeDamageResultCallback);
    public event FakeEnemyAction OnAttackedByProjectile;

    public delegate void FakeEnemyComputeDamageAction(TurretDamageAttack damageAttack);
    public event FakeEnemyComputeDamageAction OnWillBeAttackedByTurret;

    public delegate void FakeEnemyGetPositionAction(out Vector3 position, out bool foundTarget);
    public event FakeEnemyGetPositionAction OnGetPosition;


    [SerializeField] private SphereCollider sphereCollider;
    public SphereCollider SphereCollider => sphereCollider;


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
    public override int GetTargetPriorityBonus()
    {
        return -10000;
    }

    public override void GetStunned(float duration)
    {
        return;
    }
    
    
    public override void OnWillBeAttacked(TurretDamageAttack damageAttack)
    {
        OnWillBeAttackedByTurret?.Invoke(damageAttack);
    }
    
    protected override void DoTakeDamage(TurretDamageAttack damageAttack, 
        Action<TurretDamageAttackResult> takeDamageResultCallback)
    {
        if (damageAttack.ProjectileSource != null && OnAttackedByProjectile != null)
        {
            OnAttackedByProjectile(damageAttack.ProjectileSource, takeDamageResultCallback);
        }
    }


    public override int QueueDamage(TurretDamageAttack damageAttack)
    {
        return 0;
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


    public override bool CanBeAttackedByMultiCastProjectiles()
    {
        return false;
    }

}
