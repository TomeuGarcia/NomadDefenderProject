using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "CurrencySpendUpgradeStat", menuName = "TurretPassives/CurrencySpendUpgradeStat")]
public class CurrencySpendUpgradeStat : BasePassive
{
    private TurretBuilding owner;

    [SerializeField] private CurrencySpendUpgradeStatVisuals visualsPrefab;
    private CurrencySpendUpgradeStatVisuals spawnedVisuals;


    [SerializeField, Min(0)] private int startRequiredSpendAmount = 300;
    [SerializeField, Min(0)] private int requiredSpendAmountIncrement = 100;
    private int requiredSpendAmount;
    private int currentSpentAmount;

    private bool isPlaced;
    private bool isSubscribed;


    public override void ApplyEffects(TurretBuilding owner)
    {
        this.owner = owner;

        isSubscribed = false;
        isPlaced = false;

        owner.OnPlaced += OnOwnerPlaced;
        owner.OnDestroyed += UnsubscribeEvents;

        requiredSpendAmount = startRequiredSpendAmount;

        spawnedVisuals = GameObject.Instantiate(visualsPrefab, owner.transform);
    }

    private void OnOwnerPlaced(Building owner)
    {
        isPlaced = true;

        owner.OnPlaced -= OnOwnerPlaced;

        spawnedVisuals.Show();

        SubscribeEvents();
    }

    private void SubscribeEvents()
    {
        if (isSubscribed) return;
        isSubscribed = true;
        
        TDGameManager.OnGameFinishStart += UnsubscribeEvents;

        owner.OnBuildingUpgraded += CheckCanStillUpgrade;

        ServiceLocator.GetInstance().CurrencyCounter.OnCurrencyAmountSpent += OnCurrencyAmountSpentEvent;        
    }
    private void UnsubscribeEvents()
    {
        if (!isPlaced)
        {
            owner.OnPlaced -= OnOwnerPlaced;
            owner.OnDestroyed -= UnsubscribeEvents;
        }       


        if (!isSubscribed) return;
        isSubscribed = false;

        owner.OnDestroyed -= UnsubscribeEvents;
        owner.OnBuildingUpgraded -= CheckCanStillUpgrade;
        TDGameManager.OnGameFinishStart -= UnsubscribeEvents;

        ServiceLocator.GetInstance().CurrencyCounter.OnCurrencyAmountSpent -= OnCurrencyAmountSpentEvent;
    }



    private void OnCurrencyAmountSpentEvent(int amount)
    {
        currentSpentAmount += amount;
        spawnedVisuals.QueueUpdateVisuals(Mathf.Min(currentSpentAmount, requiredSpendAmount), requiredSpendAmount);

        while (currentSpentAmount >= requiredSpendAmount)
        {

            currentSpentAmount -= requiredSpendAmount;
            requiredSpendAmount += requiredSpendAmountIncrement;

            UpgradeLowestStat();
            spawnedVisuals.QueueUpdateVisuals(currentSpentAmount, requiredSpendAmount);
        }      
    }

    private void UpgradeLowestStat()
    {
        TurretUpgradeType lowestStat = owner.Upgrader.GetLowestStatUpgradeType(false);

        if (lowestStat == TurretUpgradeType.NONE) return;

        owner.Upgrader.FreeTurretUpgrade(lowestStat);         
    }

    private void CheckCanStillUpgrade()
    {
        if (!owner.Upgrader.StatsLevelBellowLimit(false))
        {
            spawnedVisuals.Hide();
            UnsubscribeEvents();
        }
    }

}
