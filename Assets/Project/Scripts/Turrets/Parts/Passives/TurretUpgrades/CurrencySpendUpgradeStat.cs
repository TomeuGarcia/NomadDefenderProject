using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "CurrencySpendUpgradeStat", menuName = "TurretPassives/CurrencySpendUpgradeStat")]
public class CurrencySpendUpgradeStat : BasePassive
{
    [SerializeField] private CurrencyUpgradeStatLogic logicPrefab;


    [SerializeField, Min(0)] private int startRequiredSpendAmount = 300;
    [SerializeField, Min(0)] private int requiredSpendAmountIncrement = 100;



    public override void ApplyEffects(TurretBuilding owner)
    {
        CurrencyUpgradeStatLogic spawnedLogic = GameObject.Instantiate(logicPrefab, owner.transform);
        spawnedLogic.Init(owner, startRequiredSpendAmount, requiredSpendAmountIncrement);
    }


}
