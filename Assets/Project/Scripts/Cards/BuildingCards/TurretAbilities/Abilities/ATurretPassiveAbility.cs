using System.Collections.Generic;
using UnityEngine;

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
        projectile.ViewAddOnsController.AddViewAddOn(viewAddOnConfig);
    }
    private void AddViewAddOnToTurret(TurretViewAddOnConfig viewAddOnConfig,
        TurretBuilding turretOwner)
    {
        OnTurretViewAddOnAdded(turretOwner.ViewAddOnController.AddViewAddOn(viewAddOnConfig));
    }

    protected virtual void OnTurretViewAddOnAdded(ATurretViewAddOn turretViewAddOn)
    {
        
    }
    
    
    
    public virtual void OnAddedToTurretCard(TurretCardData cardData) { } 
    public virtual void OnRemovedFromTurretCard() { } 
    public virtual void OnTurretCardUpgraded(TurretCardData cardData) { } 
    
    
    public virtual void OnTurretCreated(TurretBuilding turretOwner) { } 
    public virtual void OnTurretDestroyed() { } 
    
    
    public virtual void OnTurretPlacingStart() { } 
    public virtual void OnTurretPlacingFinish() { } 
    public virtual void OnTurretPlacingMove() { }
    public void OnTurretPlaced(TurretBuilding turretOwner)
    {
        if (OriginalModel.OptionalViewAddOns.TurretAddOn)
        {
            AddViewAddOnToTurret(OriginalModel.OptionalViewAddOns.TurretAddOn, turretOwner);
        }
        
        OnTurretPlaced();
    }
    protected virtual void OnTurretPlaced() { }

    
    public void OnBeforeShootingEnemy(ATurretProjectileBehaviour projectile)
    {
        DoOnBeforeShootingEnemyStart();
        
        if (OriginalModel.OptionalViewAddOns.ProjectileAddOn)
        {
            AddViewAddOnToProjectile(OriginalModel.OptionalViewAddOns.ProjectileAddOn, projectile);
        }
        
        DoOnBeforeShootingEnemyEnd(projectile);
    } 
    protected virtual void DoOnBeforeShootingEnemyStart() { } 
    protected virtual void DoOnBeforeShootingEnemyEnd(ATurretProjectileBehaviour projectile) { } 
    public virtual void OnBeforeDamagingEnemy(TurretDamageAttack damageAttack) { } 
    public virtual void OnAfterDamagingEnemy(TurretDamageAttackResult damageAttackResult) { } 
}