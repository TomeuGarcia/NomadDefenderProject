

using UnityEngine;

public class TurretCardData
{
    public readonly TurretCardDataModel OriginalModel;
    public readonly TurretCardPartsGroup SharedPartsGroup;
    public int PlayCost { get; private set; }
    public int CardUpgradeLevel { get; private set; }
    public TurretCardStatsController StatsController { get; private set; }

    
    private readonly TurretPassiveAbilitiesController _passiveAbilitiesController;
    

    public TurretCardData(TurretCardDataModel model)
    {
        OriginalModel = model;
        PlayCost = model.CardPlayCost;
        CardUpgradeLevel = model.CardLevel;
        SharedPartsGroup = model.MakePartsGroup();
        
        MakeStatsControllerFromParts();

        _passiveAbilitiesController = new TurretPassiveAbilitiesController(
            this, OriginalModel.PassiveAbilityModels, ServiceLocator.GetInstance().TurretAbilityFactory);
    }
    
    public TurretCardData(TurretCardData other)
    {
        OriginalModel = other.OriginalModel;
        PlayCost = other.PlayCost;
        CardUpgradeLevel = other.CardUpgradeLevel;
        SharedPartsGroup = new TurretCardPartsGroup(other.SharedPartsGroup);
        
        StatsController = other.StatsController;
        if (StatsController == null)
        {
            MakeStatsControllerFromParts();
        }
        
        _passiveAbilitiesController = new TurretPassiveAbilitiesController(
            this, other._passiveAbilitiesController, ServiceLocator.GetInstance().TurretAbilityFactory);
    }
    
    
    private void MakeStatsControllerFromParts()
    {
        StatsController = new TurretCardStatsController(
            SharedPartsGroup.Body.DamageStat,
            SharedPartsGroup.Body.ShotsPerSecondStat,
            SharedPartsGroup.Body.RadiusRangeStat
        );
    }


    
    public void SetPlayCost(int value)
    {
        PlayCost = value;
    }
    public void IncrementPlayCost(int incrementAmount)
    {
        SetPlayCost(PlayCost + incrementAmount);
    }

    public void SetCardUpgradeLevel(int value)
    {
        CardUpgradeLevel = Mathf.Clamp(value, 1, TurretCardDataModel.MAX_CARD_LEVEL);;
    }
    public void IncrementUpgradeLevel(int incrementAmount)
    {
        SetCardUpgradeLevel(CardUpgradeLevel + incrementAmount);
    }
    public bool IsCardUpgradeLevelMaxed()
    {
        return CardUpgradeLevel == TurretCardDataModel.MAX_CARD_LEVEL;
    }

    
    
    public void SetPassiveAbility(TurretPassiveBase passiveAbility)
    {
        SharedPartsGroup.SetPassiveAbility(passiveAbility);
    }
    
    public void AddPassiveAbility(TurretPassiveAbilityDataModel passiveAbilityModel)
    {
        _passiveAbilitiesController.AddPassiveAbility(passiveAbilityModel);
    }
    public void RemovePassiveAbility(TurretPassiveAbilityDataModel passiveAbilityModel)
    {
        _passiveAbilitiesController.RemovePassiveAbility(passiveAbilityModel);
    }
}