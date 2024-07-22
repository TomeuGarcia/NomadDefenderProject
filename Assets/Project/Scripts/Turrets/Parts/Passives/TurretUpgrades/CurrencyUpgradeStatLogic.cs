using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyUpgradeStatLogic : MonoBehaviour
{
    private TurretBuilding owner;

    [SerializeField] private CurrencySpendUpgradeStatVisuals visuals;


    private int requiredSpendAmountIncrement;
    private int requiredSpendAmount;
    private int currentSpentAmount;

    private bool isPlaced;
    private bool isSubscribed;


    public void Init(TurretBuilding owner, int requiredSpendAmount, int requiredSpendAmountIncrement)
    {
        this.owner = owner;

        this.requiredSpendAmount = requiredSpendAmount;
        this.requiredSpendAmountIncrement = requiredSpendAmountIncrement;
        this.currentSpentAmount = 0;


        isPlaced = false;
        isSubscribed = false;

        owner.OnPlaced += OnOwnerPlaced;
        owner.OnDestroyed += UnsubscribeEvents;

        visuals.Init();
    }

    private void OnOwnerPlaced(Building owner)
    {
        isPlaced = true;

        owner.OnPlaced -= OnOwnerPlaced;

        
        visuals.Show();

        SubscribeEvents();
    }

    private void SubscribeEvents()
    {
        if (isSubscribed) return;
        isSubscribed = true;

        owner.OnBuildingUpgraded += CheckCanStillUpgrade;

        ServiceLocator.GetInstance().CurrencyCounter.OnCurrencyAmountSpent += OnCurrencyAmountSpentEvent;
    }
    private void UnsubscribeEvents()
    {
        if (!isSubscribed) return;
        isSubscribed = false;

        owner.OnDestroyed -= UnsubscribeEvents;

        if (!isPlaced)
        {
            owner.OnPlaced -= OnOwnerPlaced;
            return;
        }


        owner.OnBuildingUpgraded -= CheckCanStillUpgrade;

        ServiceLocator.GetInstance().CurrencyCounter.OnCurrencyAmountSpent -= OnCurrencyAmountSpentEvent;
    }



    private void OnCurrencyAmountSpentEvent(int amount)
    {
        currentSpentAmount += amount;
        visuals.QueueUpdateVisuals(Mathf.Min(currentSpentAmount, requiredSpendAmount), requiredSpendAmount);

        while (currentSpentAmount >= requiredSpendAmount)
        {

            currentSpentAmount -= requiredSpendAmount;
            requiredSpendAmount += requiredSpendAmountIncrement;

            UpgradeLowestStat();
            visuals.QueueUpdateVisuals(currentSpentAmount, requiredSpendAmount);
        }
    }

    private void UpgradeLowestStat()
    {
        owner.Upgrader.FreeTurretUpgrade();
    }

    private void CheckCanStillUpgrade()
    {
        if (!owner.Upgrader.StatsLevelBellowLimit(false))
        {
            visuals.Hide();
            UnsubscribeEvents();
        }
    }
}
