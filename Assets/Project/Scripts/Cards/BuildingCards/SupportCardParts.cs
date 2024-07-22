using UnityEngine;


[CreateAssetMenu(fileName = "NewSupportBuilding", 
    menuName = SOAssetPaths.CARDS_BUILDINGS + "SupportCardParts")]
public class SupportCardParts : GroupedCardParts
{
    public TurretPartBase turretPartBase;
    public SupportCardStatsController StatsController { get; private set; }


    public void Init(TurretPartBase turretPartBase, int cardCost)
    {
        this.turretPartBase = turretPartBase;
        this.cardCost = cardCost;

        StatsController = new SupportCardStatsController(
            turretPartBase.RadiusRangeStat
        );
    }

    public void InitCopyingReferences(SupportCardParts other)
    {
        this.turretPartBase = other.turretPartBase;
        this.cardCost = other.cardCost;

        this.StatsController = other.StatsController;
        if (this.StatsController == null)
        {
            StatsController = new SupportCardStatsController(
                turretPartBase.RadiusRangeStat
            );
        }
    }


    public int GetCostCombinedParts()
    {
        return turretPartBase.cost;
    }

    public int GetCardCost()
    { return cardCost; }


}