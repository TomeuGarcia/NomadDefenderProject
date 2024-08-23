

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

    public void AddPassiveAbility(ATurretPassiveAbilityDataModel passiveAbilityModel)
    {
        PassiveAbilitiesController.AddPassiveAbility(passiveAbilityModel);
    }
    public void RemovePassiveAbility(ATurretPassiveAbilityDataModel passiveAbilityModel)
    {
        PassiveAbilitiesController.RemovePassiveAbility(passiveAbilityModel);
    }


    public TurretIconCanvasDisplay.ConfigData[] MakeIconsDisplayData()
    {
        TurretIconCanvasDisplay.ConfigData[] iconsDisplayData =
            new TurretIconCanvasDisplay.ConfigData[1 + PassiveAbilitiesController.PassiveAbilities.Count];

        iconsDisplayData[0] = new TurretIconCanvasDisplay.ConfigData(
            SharedPartsGroup.Projectile.abilitySprite, SharedPartsGroup.Projectile.materialColor);

        for (int i = 0; i < PassiveAbilitiesController.PassiveAbilities.Count; ++i)
        {
            ATurretPassiveAbilityDataModel.ViewConfig abilityView = PassiveAbilitiesController.PassiveAbilities[i]
                .OriginalModel.View;
            iconsDisplayData[1 + i] = new TurretIconCanvasDisplay.ConfigData(abilityView.Sprite, abilityView.Color);
        }

        return iconsDisplayData;
    }
}