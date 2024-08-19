
public class SupportCardData
{
    public SupportCardDataModel OriginalModel { get; private set; }
    public int PlayCost { get; private set; }
    public SupportCardPartsGroup SharedPartsGroup { get; private set; }
    public SupportCardStatsController StatsController { get; private set; }

    
    public SupportCardData(SupportCardDataModel model)
    {
        OriginalModel = model;
        PlayCost = model.cardCost;
        SharedPartsGroup = model.MakePartsGroup();

        MakeStatsControllerFromParts();
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
    
}