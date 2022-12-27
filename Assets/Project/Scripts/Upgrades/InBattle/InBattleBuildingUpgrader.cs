using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class InBattleBuildingUpgrader : MonoBehaviour
{
    [SerializeField] private RangeBuilding building;
    [SerializeField] private RectTransform UIParent;
    [SerializeField] private TMP_Text costText;

    [SerializeField] private List<int> upgradeCosts = new List<int>();
    [SerializeField] private List<Image> fillBars = new List<Image>();

    private CurrencyCounter currencyCounter;

    private float xOffset;
    private const int maxStatLevel = 5;
    private const int maxUpgradeCount = 3;
    private int currentLevel = 0;

    private int attackLvl;
    private int cadenceLvl;
    private int rangeLvl;
    private int passiveLvl;

    private void Awake()
    {
        xOffset = UIParent.anchoredPosition.x;

        UIParent.gameObject.SetActive(false);

        costText.text = upgradeCosts[0].ToString();
    }

    public void InitTurret(int newAttackLvl, int newCadenceLvl, int newRangeLvl, CurrencyCounter newCurrencyCounter)
    {
        attackLvl = newAttackLvl;
        cadenceLvl = newCadenceLvl;
        rangeLvl = newRangeLvl;

        UpdateAttackBar();
        UpdateCadenceBar();
        UpdateRangeBar();

        currencyCounter = newCurrencyCounter;
    }

    public void InitSupport(CurrencyCounter newCurrencyCounter)
    {
        passiveLvl = 0;

        UpdatePassiveBar(); //TODO: OUT OF 3 NOT 5

        currencyCounter = newCurrencyCounter;
    }

    public void Activate()
    {
        UIParent.gameObject.SetActive(true);

        UIParent.position = Camera.main.WorldToScreenPoint(building.transform.position) + Vector3.up * 150.0f + (Vector3.right * xOffset);

        //coroutine to deactivate(maybe)
        //waituntil click, move, zoom...?
    }

    public void Deactivate()
    {
        //click anywere else
        UIParent.gameObject.SetActive(false);
    }

    public void UpgradedAttack()
    {
        Debug.Log("Upgraded Attack");

        if(CanUpgrade(attackLvl))
        {
            currencyCounter.SubtractCurrency(upgradeCosts[currentLevel]);

            NextLevel();
            UpdateCost();

            attackLvl++;
            UpdateAttackBar();

            building.Upgrade(0);
        }
    }

    public void UpgradedCadence()
    {
        Debug.Log("Upgraded Cadence");

        if (CanUpgrade(cadenceLvl))
        {
            currencyCounter.SubtractCurrency(upgradeCosts[currentLevel]);

            NextLevel();
            UpdateCost();

            cadenceLvl++;
            UpdateCadenceBar();

            building.Upgrade(1);
        }
    }

    public void UpgradedRange()
    {
        Debug.Log("Upgraded Range");

        if (CanUpgrade(rangeLvl))
        {
            currencyCounter.SubtractCurrency(upgradeCosts[currentLevel]);

            NextLevel();
            UpdateCost();

            rangeLvl++;
            UpdateRangeBar();

            building.Upgrade(2);
        }
    }

    public void UpgradedSupport()
    {
        Debug.Log("Upgraded Support");

        if (CanUpgrade(passiveLvl))
        {
            currencyCounter.SubtractCurrency(upgradeCosts[currentLevel]);

            NextLevel();
            UpdateCost();

            passiveLvl++;
            UpdatePassiveBar();

            building.Upgrade(0);
        }
    }

    private bool CanUpgrade(int levelToCheck)
    {
        return (currentLevel < (maxUpgradeCount - 1) && levelToCheck < maxStatLevel
                && currencyCounter.HasEnoughCurrency(upgradeCosts[currentLevel]));
    }

    private void NextLevel()
    {
        currentLevel++;
    }
    private void UpdateCost()
    {
        costText.text = upgradeCosts[currentLevel].ToString();
    }

    private void UpdateAttackBar()
    {
        fillBars[0].fillAmount = (float)attackLvl * 0.2f;
    }
    private void UpdateCadenceBar()
    {
        fillBars[1].fillAmount = (float)cadenceLvl * 0.2f;
    }
    private void UpdateRangeBar()
    {
        fillBars[2].fillAmount = (float)rangeLvl * 0.2f;
    }
    private void UpdatePassiveBar()
    {
        fillBars[0].fillAmount = (float)passiveLvl * 0.33f;
    }
}
