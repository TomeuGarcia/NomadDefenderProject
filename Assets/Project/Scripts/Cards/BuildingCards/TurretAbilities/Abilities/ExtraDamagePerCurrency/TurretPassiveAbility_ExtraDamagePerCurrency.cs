

using UnityEngine;

public class TurretPassiveAbility_ExtraDamagePerCurrency : ATurretPassiveAbility, 
    ProjectileViewAddOn_ExtraDamagePerCurrency.IConfigurationSource
{
    private readonly TPADataModel_ExtraDamagePerCurrency _abilityDataModel;
    private int _bonusDamageSteps;
    private float DamagePer1Bonus => _abilityDataModel.DamagePercentBonus.Value / 100f;
    

    public TurretPassiveAbility_ExtraDamagePerCurrency(TPADataModel_ExtraDamagePerCurrency originalModel) 
        : base(originalModel)
    {
        _abilityDataModel = originalModel;

        UpdateDescriptionVariable(originalModel.CurrencyStepForBonus);
        UpdateDescriptionVariable(originalModel.DamagePercentBonus);
    }

    
    protected override void DoOnBeforeShootingEnemyStart()
    {
        _bonusDamageSteps = ServiceLocator.GetInstance().CurrencyCounter.CurrencyCount /
                            _abilityDataModel.CurrencyStepForBonus.Value;
        
        ProjectileViewAddOn_ExtraDamagePerCurrency.ConfigurationSource = this;
    }
    
    public override void OnBeforeDamagingEnemy(TurretDamageAttack damageAttack)
    {
        float damageMultiplier = 1f + (_bonusDamageSteps * DamagePer1Bonus);
        int damage = Mathf.RoundToInt((int)(damageAttack.Damage * damageMultiplier));
        
        damageAttack.UpdateDamage(damage);
    }

    public int BonusDamageSteps()
    {
        return _bonusDamageSteps;
    }
}