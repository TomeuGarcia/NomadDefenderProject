using UnityEngine;


[CreateAssetMenu(fileName = "NewSupportBuilding", menuName = "Cards/SupportCardParts")]
public class SupportCardParts : ScriptableObject
{
    public TurretPartBase turretPartBase;
    public int cardCost;


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