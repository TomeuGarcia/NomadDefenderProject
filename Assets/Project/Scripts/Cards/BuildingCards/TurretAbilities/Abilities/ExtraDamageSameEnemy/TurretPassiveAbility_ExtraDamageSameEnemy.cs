public class TurretPassiveAbility_ExtraDamageSameEnemy : ATurretPassiveAbility, 
    TurretViewAddOn_ExtraDamageSameEnemy.IConfigurationSource
{
    private readonly float _damageMultiplierStep;
    private float _accumulatedDamageMultiplier;
    private int _accumulatedHits;

    private Enemy _previouslyHitEnemy;
    
    

    public TurretPassiveAbility_ExtraDamageSameEnemy(TPADataModel_ExtraDamageSameEnemy originalModel) 
        : base(originalModel)
    {
        _damageMultiplierStep = originalModel.DamagePercentBonus.Value / 100f;
        _accumulatedDamageMultiplier = 1f;
        _accumulatedHits = 0;
        
        UpdateDescriptionVariable(originalModel.DamagePercentBonus);
    }

    public override void OnCardDestroyed()
    {
        UnsubscribeEnemyDeath();
    }

    public override void OnBeforeDamagingEnemy(TurretDamageAttack damageAttack)
    {
        if (_previouslyHitEnemy == damageAttack.Target)
        {
            damageAttack.UpdateDamage((int)(damageAttack.Damage * _accumulatedDamageMultiplier));
            
            _projectileParticleFactory.CreateParticlesGameObject(ProjectileParticleType.CleanStrike_Hit, 
                damageAttack.Target.Position, damageAttack.ProjectileSource.Rotation);
        }
    }

    public override void OnAfterDamagingEnemy(TurretDamageAttackResult damageAttackResult)
    {
        if (damageAttackResult.HitKilled)
        {
            DoEnemyGotKilled();
            return;
        }
        
        if (_previouslyHitEnemy == damageAttackResult.Target)
        {
            DoSameEnemyGotHit();
        }
        else
        {
            DoNewEnemyGotHit(damageAttackResult.Target);
        }
    }

    private void DoEnemyGotKilled()
    {
        _previouslyHitEnemy = null;
        _accumulatedDamageMultiplier = 1f;
        _accumulatedHits = 0;
    }

    private void DoSameEnemyGotHit()
    {
        _accumulatedDamageMultiplier += _damageMultiplierStep;
        ++_accumulatedHits;
    }
    private void DoNewEnemyGotHit(Enemy newEnemy)
    {
        _previouslyHitEnemy = newEnemy;
        _accumulatedDamageMultiplier = 1f;
        _accumulatedHits = 0;
        SubscribeEnemyDeath();
    }


    private void SubscribeEnemyDeath()
    {
        _previouslyHitEnemy.OnEnemyDeactivated += OnEnemyDeactivated;
    }
    private void UnsubscribeEnemyDeath()
    {
        if (_previouslyHitEnemy != null)
        {
            _previouslyHitEnemy.OnEnemyDeactivated -= OnEnemyDeactivated;
        }
    }

    private void OnEnemyDeactivated(Enemy enemy)
    {
        UnsubscribeEnemyDeath();
        DoEnemyGotKilled();
    }
    
    
    protected override void OnTurretViewAddOnAdded(ATurretViewAddOn turretViewAddOn)
    {
        if(turretViewAddOn is TurretViewAddOn_ExtraDamageSameEnemy turretViewAddOnTyped)
        {
            turretViewAddOnTyped.ConfigurationSource = this;
        }
    }

    public int GetNumberOfAccumulatedHits()
    {
        return _accumulatedHits;
    }
}