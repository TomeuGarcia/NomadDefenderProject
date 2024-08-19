

using UnityEngine;

public class TurretCardData
{
    public TurretCardDataModel OriginalModel { get; private set; }
    public int PlayCost { get; private set; }
    public int CardUpgradeLevel { get; private set; }
    public TurretCardPartsGroup SharedPartsGroup { get; private set; }
    public TurretCardStatsController StatsController { get; private set; }
    

    public TurretCardData(TurretCardDataModel model)
    {
        OriginalModel = model;
        PlayCost = model.cardCost;
        CardUpgradeLevel = model.cardLevel;
        SharedPartsGroup = model.MakePartsGroup();
        
        MakeStatsControllerFromParts();
    }
    
    public TurretCardData(TurretCardData other)
    {
        OriginalModel = other.OriginalModel;
        PlayCost = other.PlayCost;
        CardUpgradeLevel = other.CardUpgradeLevel;
        SharedPartsGroup = other.SharedPartsGroup;
        
        StatsController = other.StatsController;
        if (StatsController == null)
        {
            MakeStatsControllerFromParts();
        }
    }
    
    
    private void MakeStatsControllerFromParts()
    {
        StatsController = new TurretCardStatsController(
            SharedPartsGroup.Body.DamageStat,
            SharedPartsGroup.Body.ShotsPerSecondStat,
            SharedPartsGroup.Base.RadiusRangeStat
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
}