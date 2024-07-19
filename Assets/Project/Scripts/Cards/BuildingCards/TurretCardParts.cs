using UnityEngine;


[CreateAssetMenu(fileName = "NewTurretBuilding", 
    menuName = SOAssetPaths.CARDS_BUILDINGS + "TurretCardParts")]
public class TurretCardParts : GroupedCardParts
{
    public TurretPartAttack turretPartAttack;
    public TurretPartBody turretPartBody;
    public TurretPartBase turretPartBase;
    public TurretPassiveBase turretPassiveBase;
    [Range(1, 3)] public int cardLevel = 1;
    public const int MAX_CARD_LEVEL = 3;

    public TurretCardStatsController StatsController { get; private set; }


    public void Init(int cardLevel, TurretPartAttack turretPartAttack, TurretPartBody turretPartBody,
                     TurretPartBase turretPartBase, TurretPassiveBase turretPassiveBase, int cardCost)
    {
        this.turretPartAttack = turretPartAttack;
        this.turretPartBody = turretPartBody;
        this.turretPartBase = turretPartBase;
        this.turretPassiveBase = turretPassiveBase;
        this.cardCost = cardCost;
        this.cardLevel = cardLevel;

        StatsController = new TurretCardStatsController(
            turretPartBody.DamageStat, turretPartBody.ShotsPerSecondStat, turretPartBase.RadiusRangeStat);
    }

    public void InitCopyingReferences(TurretCardParts other)
    {
        this.turretPartAttack = other.turretPartAttack;
        this.turretPartBody = other.turretPartBody;
        this.turretPartBase = other.turretPartBase;
        this.turretPassiveBase = other.turretPassiveBase;
        this.cardCost = other.cardCost;
        this.cardLevel = other.cardLevel;

        this.StatsController = StatsController;
    }

    private void OnValidate()
    {
        cardLevel = Mathf.Clamp(cardLevel, 1, MAX_CARD_LEVEL);
    }

    public int GetCostCombinedParts()
    {
        return turretPartAttack.cost + turretPartBody.cost + turretPartBase.cost + turretPassiveBase.cost;
    }

    public int GetCardCost()
    { return cardCost; }
}