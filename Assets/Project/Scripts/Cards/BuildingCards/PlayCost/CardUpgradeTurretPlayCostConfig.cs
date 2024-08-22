using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "CardUpgradeTurretPlayCostConfig", 
    menuName = SOAssetPaths.CARDS_COST + "CardUpgradeTurretPlayCostConfig")]
public class CardUpgradeTurretPlayCostConfig : ScriptableObject
{
    [Header("CONFIGURATION")]
    [SerializeField] private bool _useCalculator = false;

    [ShowIf("_useCalculator")]
    [SerializeField] private TurretCardPlayCostCalculator _calculator;

    [HideIf("_useCalculator")]
    [SerializeField, Range(0, 100)] private int _incrementCost = 30;

    [SerializeField, Range(0, 100)] private int _decrementCost = 30;



    public int ComputeCardPlayCostIncrement(bool costNeedsToIncrement, TurretBuildingCard turretCard)
    {
        if (costNeedsToIncrement)
        {
            if (_useCalculator)
            {
                TurretPartBody body = turretCard.CardParts.Body;
                TurretPartProjectileDataModel attack = turretCard.CardParts.Projectile;
                TurretCardPlayCostCalculator.InputData turretData = new TurretCardPlayCostCalculator.InputData(
                    (int)body.DamageStat.ComputeValueByLevel(0), body.ShotsPerSecondStat.ComputeValueByLevel(0), 1);
                return _calculator.ComputePlayCost(turretData) - turretCard.GetCardPlayCost();
            }

            return _incrementCost;
        }

        return -_decrementCost;
    }

}
