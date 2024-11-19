using System;
using System.Threading.Tasks;
using Project.Scripts.Turrets.Visuals;
using UnityEngine;

public class TurretPassiveAbility_ExtraSellValuePerHit : ATurretPassiveAbility
{
    private TurretBuilding _turretOwner;

    private readonly TPADataModel_ExtraSellValuePerHit _abilityDataModel;


    private float _currentSellIncrementPer1;
    private int _damageHitCount;

    private BuildingSellingConfig OwnerSellingConfig => _turretOwner.CardData.BuildingSellingConfig;
    
    
    public TurretPassiveAbility_ExtraSellValuePerHit(TPADataModel_ExtraSellValuePerHit originalModel) 
        : base(originalModel)
    {
        _abilityDataModel = originalModel;
        ApplyDescriptionCorrection(_abilityDataModel.HitsToIncrement);
        ApplyDescriptionCorrection(_abilityDataModel.SellValueIncrementAmount);
    }



    public override void OnTurretCreated(TurretBuilding turretOwner)
    {
        _turretOwner = turretOwner;
        _currentSellIncrementPer1 = 0f;
        _damageHitCount = 0;
    }
    
    
    protected override void OnTurretPlaced()
    {
        _currentSellIncrementPer1 = 0f;
        _damageHitCount = 0;
        OwnerSellingConfig.OverwriteExtraAddAmount(0);
    }

    public override void OnAfterDamagingEnemy(TurretDamageAttackResult damageAttackResult)
    {
        OnDamageDealt();
    }
    
    private void OnDamageDealt()
    {
        ++_damageHitCount;
        if (_damageHitCount < _abilityDataModel.HitsToIncrement.Value)
        {
            return;
        }

        _damageHitCount = 0;

        _currentSellIncrementPer1 += _abilityDataModel.SellValueIncrementAmount.Value / 100f;
        int currentExtraSellValue = Mathf.CeilToInt(_turretOwner.CardData.PlayCost * _currentSellIncrementPer1);
        
        OwnerSellingConfig.OverwriteExtraAddAmount(currentExtraSellValue);

        ServiceLocator.GetInstance().ParticleFactory.Create(ParticleTypes.IncreaseSellValue,
            _turretOwner.PlacingParticlesPosition, Quaternion.identity);
    }


}