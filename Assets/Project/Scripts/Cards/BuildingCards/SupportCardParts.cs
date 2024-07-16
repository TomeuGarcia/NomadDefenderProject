using UnityEngine;


[CreateAssetMenu(fileName = "NewSupportBuilding", 
    menuName = SOAssetPaths.CARDS_BUILDINGS + "SupportCardParts")]
public class SupportCardParts : GroupedCardParts
{
    public TurretPartBase turretPartBase;


    public void Init(TurretPartBase turretPartBase, int cardCost)
    {

        this.turretPartBase = turretPartBase;
        this.cardCost = cardCost;
    }

    public void Init(SupportCardParts other)
    {
        this.turretPartBase = other.turretPartBase;
        this.cardCost = other.cardCost;
    }


    public int GetCostCombinedParts()
    {
        return turretPartBase.cost;
    }

    public int GetCardCost()
    { return cardCost; }


}