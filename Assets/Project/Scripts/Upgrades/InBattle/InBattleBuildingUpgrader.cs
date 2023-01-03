using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public enum TurretUpgradeType { ATTACK, CADENCE, RANGE, SUPPORT };

public class InBattleBuildingUpgrader : MonoBehaviour
{
    [SerializeField] private Transform building;
    [SerializeField] private RectTransform UIParent;
    [SerializeField] private RectTransform mouseDetectionPanel;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private TMP_Text lvlText;

    [SerializeField] private List<int> upgradeCosts = new List<int>();
    [SerializeField] private List<Image> fillBars = new List<Image>();
    
    [SerializeField] private Color32 disalbedTextColor;

    private CurrencyCounter currencyCounter;

    private float xOffset;
    private const int maxStatLevel = 5;
    private const int maxSupportStatLevel = 3;
    private const int maxUpgradeCount = 3;
    private int currentLevel = 0;

    private int attackLvl;
    private int cadenceLvl;
    private int rangeLvl;
    private int supportLvl;

    private float turretFillBarCoef;
    private float supportFillBarCoef;

    private bool visible;

    private float lastScroll;

    private void Awake()
    {
        xOffset = UIParent.anchoredPosition.x;

        UIParent.gameObject.SetActive(false);

        costText.text = upgradeCosts[0].ToString();

        turretFillBarCoef = 100.0f / ((float)maxStatLevel * 100.0f);
        supportFillBarCoef = 100.0f / ((float)maxSupportStatLevel * 100.0f);

        visible = false;
    }

    private void Update()
    {
        bool outOfArea = !RectTransformUtility.RectangleContainsScreenPoint(mouseDetectionPanel, Input.mousePosition);

        if (outOfArea && Input.GetMouseButtonDown(0) && visible || Input.GetMouseButtonDown(1) || Input.mouseScrollDelta.y != lastScroll)
        {
            CloseWindow();
        }
    }

    private void LateUpdate()
    {
        lastScroll = Input.mouseScrollDelta.y;
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
        supportLvl = 0;

        UpdateSupportBar();

        currencyCounter = newCurrencyCounter;
    }

    public void OpenWindow()
    {
        UIWindowManager.GetInstance().OpenedWindow(this);

        UIParent.gameObject.SetActive(true);

        UIParent.position = Camera.main.WorldToScreenPoint(building.position) + Vector3.up * 150.0f + (Vector3.right * xOffset);
        StartCoroutine(SetVisible());
    }

    public void CloseWindow()
    {
        UIWindowManager.GetInstance().ClosedWindow(this);

        visible = false;
        UIParent.gameObject.SetActive(false);
    }

    private IEnumerator SetVisible()
    {
        yield return null;
        visible = true;
    }

    public void UpgradedAttack()
    {
        if(CanUpgrade(attackLvl))
        {
            currencyCounter.SubtractCurrency(upgradeCosts[currentLevel]);

            NextLevel();

            attackLvl++;
            UpdateAttackBar();

            building.gameObject.GetComponent<TurretBuilding>().Upgrade(TurretUpgradeType.ATTACK, attackLvl);
        }
    }

    public void UpgradedCadence()
    {
        if (CanUpgrade(cadenceLvl))
        {
            currencyCounter.SubtractCurrency(upgradeCosts[currentLevel]);

            NextLevel();

            cadenceLvl++;
            UpdateCadenceBar();

            building.gameObject.GetComponent<TurretBuilding>().Upgrade(TurretUpgradeType.CADENCE, cadenceLvl);
        }
    }

    public void UpgradedRange()
    {
        if (CanUpgrade(rangeLvl))
        {
            currencyCounter.SubtractCurrency(upgradeCosts[currentLevel]);

            NextLevel();

            rangeLvl++;
            UpdateRangeBar();

            building.gameObject.GetComponent<TurretBuilding>().Upgrade(TurretUpgradeType.RANGE, rangeLvl);
        }
    }

    public void UpgradedSupport()
    {
        if (CanUpgrade(supportLvl))
        {
            currencyCounter.SubtractCurrency(upgradeCosts[currentLevel]);

            NextLevel();

            supportLvl++;
            UpdateSupportBar();

            building.gameObject.GetComponent<SupportBuilding>().Upgrade(TurretUpgradeType.SUPPORT, supportLvl);
        }
    }

    private bool CanUpgrade(int levelToCheck)
    {
        return (currentLevel < maxUpgradeCount && levelToCheck < maxStatLevel
                && currencyCounter.HasEnoughCurrency(upgradeCosts[currentLevel]));
    }

    private void NextLevel()
    {
        currentLevel++;
        
        if(currentLevel < maxUpgradeCount)
        {
            lvlText.text = "LVL " + currentLevel.ToString();
            costText.text = upgradeCosts[currentLevel].ToString();
        }
        else
        {
            lvlText.text = "LVL MAX";
            //costText.text = "NULL";
            costText.text = "0";
            costText.color = disalbedTextColor;
        }
    }

    private void UpdateAttackBar()
    {
        fillBars[(int)TurretUpgradeType.ATTACK].fillAmount = (float)attackLvl * turretFillBarCoef;
    }
    private void UpdateCadenceBar()
    {
        fillBars[(int)TurretUpgradeType.CADENCE].fillAmount = (float)cadenceLvl * turretFillBarCoef;
    }
    private void UpdateRangeBar()
    {
        fillBars[(int)TurretUpgradeType.RANGE].fillAmount = (float)rangeLvl * turretFillBarCoef;
    }
    private void UpdateSupportBar()
    {
        fillBars[0].fillAmount = (float)supportLvl * supportFillBarCoef;
    }
}
