

using UnityEngine;

public class TurretCardData
{
    public readonly TurretCardDataModel OriginalModel;
    public readonly TurretCardPartsGroup SharedPartsGroup;
    public int PlayCost { get; private set; }
    public int CardUpgradeLevel { get; private set; }
    public TurretCardStatsController StatsController { get; private set; }

    public TurretPassiveAbilitiesController PassiveAbilitiesController { get; private set; }


    public TurretCardData(TurretCardDataModel model)
    {
        OriginalModel = model;
        PlayCost = model.CardPlayCost;
        CardUpgradeLevel = model.CardLevel;
        SharedPartsGroup = model.MakePartsGroup();
        
        MakeStatsControllerFromParts();

        PassiveAbilitiesController = new TurretPassiveAbilitiesController(
            this, OriginalModel.PassiveAbilityModels);
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
        
        PassiveAbilitiesController = new TurretPassiveAbilitiesController(
            this, other.PassiveAbilitiesController);
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

    
    
    public void SetProjectilePart(TurretPartProjectileDataModel projectileDataModel)
    {
        SharedPartsGroup.SetProjectile(projectileDataModel);
    }
    public void SetPassiveAbility(TurretPassiveBase passiveAbility)
    {
        SharedPartsGroup.SetPassiveAbility(passiveAbility);
    }
    
    public void AddPassiveAbility(ATurretPassiveAbilityDataModel passiveAbilityModel)
    {
        PassiveAbilitiesController.AddPassiveAbility(passiveAbilityModel);
    }
    public void RemovePassiveAbility(ATurretPassiveAbilityDataModel passiveAbilityModel)
    {
        PassiveAbilitiesController.RemovePassiveAbility(passiveAbilityModel);
    }
}