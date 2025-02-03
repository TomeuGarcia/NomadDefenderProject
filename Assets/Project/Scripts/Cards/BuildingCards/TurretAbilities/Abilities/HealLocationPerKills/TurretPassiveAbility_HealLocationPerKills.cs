using System;
using System.Threading.Tasks;
using Project.Scripts.Turrets.Visuals;
using UnityEngine;

public class TurretPassiveAbility_HealLocationPerKills : ATurretPassiveAbility
{
    private TurretBuilding _turretOwner;

    private readonly TPADataModel_HealLocationPerKills _abilityDataModel;
    private int _totalKillsCount;
    private int _currentKillsCounter;

    private HealLocationPerKillsTurretBuildingVisuals _drainVisuals;
    
    public TurretPassiveAbility_HealLocationPerKills(TPADataModel_HealLocationPerKills originalModel) 
        : base(originalModel)
    {
        _abilityDataModel = originalModel;
        UpdateDescriptionVariable(_abilityDataModel.HealAmount);
        UpdateDescriptionVariable(_abilityDataModel.StartingKills);
        UpdateDescriptionVariable(_abilityDataModel.KillsIncrease);
    }



    public override void OnTurretCreated(TurretBuilding turretOwner)
    {
        _turretOwner = turretOwner;
    }


    public override void OnTurretPlacingStart()
    {
        _totalKillsCount = _abilityDataModel.StartingKills.Value;
        _currentKillsCounter = 0;
    }
    
    protected override void OnTurretPlaced()
    {
        if (_drainVisuals != null)
        {
            return;
        }
        
        _drainVisuals = GameObject.Instantiate(_abilityDataModel.VisualsPrefab, _turretOwner.transform);
        _drainVisuals.Init(_currentKillsCounter, _totalKillsCount);
    }

    public override void OnAfterDamagingEnemy(TurretDamageAttackResult damageAttackResult)
    {
        if (!damageAttackResult.HitKilled)
        {
            return;
        }

        IncrementKillsCounter();
    }

    private void IncrementKillsCounter()
    {
        ++_currentKillsCounter;
        _drainVisuals.UpdateCurrentKills(_currentKillsCounter);
        if (_currentKillsCounter < _totalKillsCount)
        {
            return;
        }

        OnKillsCounterReachedMax();
    }

    private void OnKillsCounterReachedMax()
    {
        _totalKillsCount += _abilityDataModel.KillsIncrease.Value;
        _currentKillsCounter = 0;

        const float delay = 0.5f;
        _drainVisuals.DelayedCompleteKills(_totalKillsCount, delay);
        DoHeal(delay);
    }

    private async void DoHeal(float delay)
    {
        await Task.Delay(TimeSpan.FromSeconds(delay));

        if (ServiceLocator.GetInstance().TDLocationsUtils.
            GetMostDamagedLocation(_turretOwner.Position, out PathLocation mostDamagedLocation))
        {
            mostDamagedLocation.Heal(_abilityDataModel.HealAmount.Value);
        }        
    }


    


}