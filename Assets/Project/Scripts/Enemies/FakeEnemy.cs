using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeEnemy : Enemy
{
    private bool canBeTargeted = true;

    public delegate void FakeEnemyAction(ATurretProjectileBehaviour projectile);
    public event FakeEnemyAction OnAttackedByProjectile;

    public delegate void FakeEnemyComputeDamageAction(TurretDamageAttack damageAttack);
    public event FakeEnemyComputeDamageAction OnWillBeAttackedByTurret;

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
        return -10000f;
    }

    public override void GetStunned(float duration)
    {
        return;
    }
    
    
    public override void OnWillBeAttacked(TurretDamageAttack damageAttack)
    {
        OnWillBeAttackedByTurret?.Invoke(damageAttack);
    }
    
    protected override TurretDamageAttackResult DoTakeDamage(TurretDamageAttack damageAttack)
    {
        if (damageAttack.ProjectileSource != null && OnAttackedByProjectile != null)
        {
            OnAttackedByProjectile(damageAttack.ProjectileSource);
        }

        return null;
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

    public void SetupColliderBounds()
    {
        colliderBounds = new Bounds(boxCollider.transform.position, boxCollider.size);
    }
    
    public Bounds GetColliderBounds()
    {
        colliderBounds.center = boxCollider.transform.position;
        return colliderBounds;
    }

    public override bool CanBeAttackedByMultiCastProjectiles()
    {
        return false;
    }

}
