using System.Collections.Generic;

public abstract class ATurretPassiveAbility
{
    public const int MAX_AMOUNT_FOR_TURRET = 3;
    
    
    public readonly ATurretPassiveAbilityDataModel OriginalModel;
    public string Name => _abilityDescription.NameForDisplay;
    public string Description
    {
        get
        {
            _abilityDescription.ApplyDescriptionModifications(_descriptionCorrections);
            _descriptionCorrections.Clear();
            return _abilityDescription.Description;
        }
    }

    private readonly EditableCardAbilityDescription _abilityDescription;
    private readonly Dictionary<string, string> _descriptionCorrections;

    protected ProjectileParticleFactory _projectileParticleFactory;

    public EditableCardAbilityDescription GetAbilityDescription()
    {
        _abilityDescription.ApplyDescriptionModifications(_descriptionCorrections);
        _descriptionCorrections.Clear();
        return _abilityDescription;
    }
    
    protected ATurretPassiveAbility(ATurretPassiveAbilityDataModel originalModel)
    {
        OriginalModel = originalModel;
        
        _abilityDescription = OriginalModel.MakeDescription();
        _descriptionCorrections = new Dictionary<string, string>();

        _projectileParticleFactory = ProjectileParticleFactory.GetInstance();
    }

    protected void ApplyDescriptionCorrection(string name, int value)
    {
        ApplyDescriptionCorrection(name, value.ToString());
    }
    protected void ApplyDescriptionCorrection(AbilityDescriptionVariable constantVariable)
    {
        ApplyDescriptionCorrection(constantVariable.Name, constantVariable.ValueAsString());
    }
    private void ApplyDescriptionCorrection(string name, string value)
    {
        if (_descriptionCorrections.ContainsKey(name))
        {
            _descriptionCorrections[name] = value;
            return;
        }
        
        _descriptionCorrections.Add(name, value);
    }

    protected void AddViewAddOnToProjectile(ProjectileViewAddOnConfig viewAddOnConfig,
        ATurretProjectileBehaviour projectile)
    {
        AProjectileViewAddOn projectileViewAddOn = _projectileParticleFactory.CreateProjectileAddOn(viewAddOnConfig);
        projectile.View.AddViewAddOn(projectileViewAddOn);
    }
    
    
    
    public virtual void OnAddedToTurretCard(TurretCardData cardData) { } 
    public virtual void OnRemovedFromTurretCard() { } 
    public virtual void OnTurretCardUpgraded(TurretCardData cardData) { } 
    
    
    public virtual void OnTurretCreated(TurretBuilding turretOwner) { } 
    public virtual void OnTurretDestroyed() { } 
    
    
    public virtual void OnTurretPlacingStart() { } 
    public virtual void OnTurretPlacingFinish() { } 
    public virtual void OnTurretPlacingMove() { } 
    public virtual void OnTurretPlaced() { }

    
    public void OnBeforeShootingEnemy(ATurretProjectileBehaviour projectile)
    {
        if (OriginalModel.OptionalViewAddOns.ProjectileAddOn)
        {
            AddViewAddOnToProjectile(OriginalModel.OptionalViewAddOns.ProjectileAddOn, projectile);
        }
        
        DoOnBeforeShootingEnemy(projectile);
    } 
    protected virtual void DoOnBeforeShootingEnemy(ATurretProjectileBehaviour projectile) { } 
    public virtual void OnBeforeDamagingEnemy(TurretDamageAttack damageAttack) { } 
    public virtual void OnAfterDamagingEnemy(TurretDamageAttackResult damageAttackResult) { } 
}