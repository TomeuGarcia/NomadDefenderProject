
public class SupportCardData
{
    public SupportCardDataModel OriginalModel { get; private set; }
    public int PlayCost { get; private set; }
    public SupportCardPartsGroup SharedPartsGroup { get; private set; }
    public SupportCardStatsController StatsController { get; private set; }

    public readonly EditableCardAbilityDescription[] AbilityDescriptions;

    
    public SupportCardData(SupportCardDataModel model)
    {
        OriginalModel = model;
        PlayCost = model.CardPlayCost;
        SharedPartsGroup = model.MakePartsGroup();

        MakeStatsControllerFromParts();

        AbilityDescriptions = SharedPartsGroup.Base.MakeAbilityDescriptions();
    }
    
    public SupportCardData(SupportCardData other)
    {
        OriginalModel = other.OriginalModel;
        PlayCost = other.PlayCost;
        SharedPartsGroup = other.SharedPartsGroup;

        StatsController = other.StatsController;
        if (StatsController == null)
        {
            MakeStatsControllerFromParts();
        }
    }
    
    
    private void MakeStatsControllerFromParts()
    {
        StatsController = new SupportCardStatsController(
            SharedPartsGroup.Base.RadiusRangeStat
        );
    }
    
    public string GetUpgradeDescriptionByLevel(int level)
    {
        return AbilityDescriptions[level].Description;
    }
}