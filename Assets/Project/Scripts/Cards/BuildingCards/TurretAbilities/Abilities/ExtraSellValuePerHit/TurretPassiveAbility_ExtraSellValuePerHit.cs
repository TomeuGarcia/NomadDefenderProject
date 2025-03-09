using System;
using System.Threading.Tasks;
using Project.Scripts.Turrets.Visuals;
using UnityEngine;

public class TurretPassiveAbility_ExtraSellValuePerHit : ATurretPassiveAbility
{
    private TurretBuilding _turretOwner;

    private readonly TPADataModel_ExtraSellValuePerHit _abilityDataModel;


    private int _currentSellIncrements;
    private int _damageHitCount;

    private BuildingSellingConfig OwnerSellingConfig => _turretOwner.CardData.BuildingSellingConfig;
    
    
    public TurretPassiveAbility_ExtraSellValuePerHit(TPADataModel_ExtraSellValuePerHit originalModel) 
        : base(originalModel)
    {
        _abilityDataModel = originalModel;
        UpdateDescriptionVariable(_abilityDataModel.HitsToIncrement);
        UpdateDescriptionVariable(_abilityDataModel.SellValueIncrementAmount);
    }



    public override void OnTurretCreated(TurretBuilding turretOwner)
    {
        _turretOwner = turretOwner;
        _currentSellIncrements = 0;
        _damageHitCount = 0;
    }
    
    
    protected override void OnTurretPlaced()
    {
        _currentSellIncrements = 0;
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
        _currentSellIncrements++;


        int extraSellValue = _currentSellIncrements * _abilityDataModel.SellValueIncrementAmount.Value;
        OwnerSellingConfig.OverwriteExtraAddAmount(extraSellValue);

        ServiceLocator.GetInstance().ParticleFactory.Create(ParticleTypes.IncreaseSellValue,
            _turretOwner.PlacingParticlesPosition, Quaternion.identity);
    }


}