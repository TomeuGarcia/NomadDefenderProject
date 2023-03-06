using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Enemy;

public enum TurretUpgradeType { ATTACK, CADENCE, RANGE, SUPPORT };

public class InBattleBuildingUpgrader : MonoBehaviour
{
    [SerializeField] private Transform building;
    [SerializeField] private RectTransform mouseDetectionPanel;
    [SerializeField] private TMP_Text lvlText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private Image costCurrencyImage;

    [SerializeField] private List<int> upgradeCosts = new List<int>();
    [SerializeField] protected List<Image> fillBars = new List<Image>();
    
    [SerializeField] private Color32 disalbedTextColor;


    [Header("BUILDING TYPE")]
    [SerializeField] private BuildingCard.CardBuildingType buildingType = BuildingCard.CardBuildingType.NONE;
    private TurretBuilding turretBuilding;
    private SupportBuilding supportBuilding;
    private int maxLevels;


    [Header("NEW UI")]
    [Header("General")]
    [SerializeField] protected RectTransform newUiParent;
    [SerializeField] protected Image barImage;
    [SerializeField] protected Image backgroundImage;
    [SerializeField] protected CanvasGroup cgLvlText;
    [SerializeField] protected CanvasGroup cgCostText;
    protected Coroutine openAnimationCoroutine = null;
    protected Coroutine closeAnimationCoroutine = null;



    private CurrencyCounter currencyCounter;

    private float xOffset;
    private const int maxStatLevel = 5;
    private const int maxSupportStatLevel = 3;
    private const int maxUpgradeCount = 3;
    protected int currentLevel = 0;

    protected int attackLvl;
    protected int cadenceLvl;
    protected int rangeLvl;
    private int supportLvl;

    protected float turretFillBarCoef;
    protected float supportFillBarCoef;

    private bool visible;

    private float lastScroll;

    public delegate void TurretUpgradeEvent(int newLevel);
    public static TurretUpgradeEvent OnTurretUpgrade;

    private void Awake()
    {
        AwakeInit();
    }

    protected virtual void AwakeInit()
    {
        xOffset = newUiParent.anchoredPosition.x;

        newUiParent.gameObject.SetActive(false);



        costText.text = upgradeCosts[0].ToString();

        turretFillBarCoef = 100.0f / ((float)maxStatLevel * 100.0f);
        supportFillBarCoef = 100.0f / ((float)maxSupportStatLevel * 100.0f);

        visible = false;
    }

    private void Start()
    {
        if (buildingType == BuildingCard.CardBuildingType.TURRET)
        {
            turretBuilding = building.gameObject.GetComponent<TurretBuilding>();
            maxLevels = turretBuilding.CardLevel;
            UpdateAttackBar();
            UpdateCadenceBar();
            UpdateRangeBar();
        }
        else if (buildingType == BuildingCard.CardBuildingType.SUPPORT)
        {
            supportBuilding = building.gameObject.GetComponent<SupportBuilding>();
            maxLevels = maxUpgradeCount;
            UpdateSupportBar();
        }
        UpdateLevelText();
    }

    private void Update()
    {
        bool outOfArea = !IsHoveringWindow();

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

        currencyCounter = newCurrencyCounter;
    }

    public void InitSupport(CurrencyCounter newCurrencyCounter)
    {
        supportLvl = 0;

        currencyCounter = newCurrencyCounter;
    }

    public void OpenWindow()
    {
        if(!UIWindowManager.GetInstance().IsHoveringOtherWindow(this))
        {
            UIWindowManager.GetInstance().OpenedWindow(this);

            newUiParent.gameObject.SetActive(true);

            newUiParent.position = Camera.main.WorldToScreenPoint(building.position) + Vector3.up * 50.0f + (Vector3.right * xOffset);
            StartCoroutine(SetVisible());

            PlayOpenAnimation();
        }
    }

    public void CloseWindow()
    {
        UIWindowManager.GetInstance().ClosedWindow(this);

        visible = false;
        //UIParent.gameObject.SetActive(false);
        

        PlayCloseAnimation();
    }

    public bool IsHoveringWindow()
    {
        return RectTransformUtility.RectangleContainsScreenPoint(mouseDetectionPanel, Input.mousePosition);
    }

    private IEnumerator SetVisible()
    {
        yield return null;
        visible = true;
    }

    public void UpgradedAttack() // Called by button
    {
        if(CanUpgrade(attackLvl))
        {
            currencyCounter.SubtractCurrency(upgradeCosts[currentLevel]);

            NextLevel();

            attackLvl++;
            UpdateAttackBar();

            turretBuilding.Upgrade(TurretUpgradeType.ATTACK, attackLvl);
        }
    }

    public void UpgradedCadence() // Called by button
    {
        if (CanUpgrade(cadenceLvl))
        {
            currencyCounter.SubtractCurrency(upgradeCosts[currentLevel]);

            NextLevel();

            cadenceLvl++;
            UpdateCadenceBar();

            turretBuilding.Upgrade(TurretUpgradeType.CADENCE, cadenceLvl);
        }
    }

    public void UpgradedRange() // Called by button
    {
        if (CanUpgrade(rangeLvl))
        {
            currencyCounter.SubtractCurrency(upgradeCosts[currentLevel]);

            NextLevel();

            rangeLvl++;
            UpdateRangeBar();

            turretBuilding.Upgrade(TurretUpgradeType.RANGE, rangeLvl);
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

            supportBuilding.Upgrade(TurretUpgradeType.SUPPORT, supportLvl);
        }
    }

    private bool CanUpgrade(int levelToCheck)
    {
        if (IsCardUpgradedToMax(currentLevel)) return false;

        return currentLevel < maxUpgradeCount && !IsStatMaxed(levelToCheck) && HasEnoughCurrencyToLevelUp();
    }
    protected bool IsCardUpgradedToMax(int levelToCheck)
    {
        return levelToCheck >= maxLevels;
    }
    protected bool IsStatMaxed(int levelToCheck)
    {
        return levelToCheck >= maxStatLevel;
    }
    protected bool HasEnoughCurrencyToLevelUp()
    {
        return currencyCounter.HasEnoughCurrency(upgradeCosts[currentLevel]);
    }

    private void NextLevel()
    {
        currentLevel++;
        UpdateLevelText();

        if(OnTurretUpgrade != null) { OnTurretUpgrade(currentLevel); }
    }

    private void UpdateLevelText()
    {
        lvlText.text = "LVL " + currentLevel.ToString() + "/" + maxLevels.ToString(); // Tomeu: I moved this here and commented if-else (a b)

        //if (currentLevel < maxUpgradeCount)
        if (currentLevel < maxLevels)
        {
            //lvlText.text = "LVL " + currentLevel.ToString() + "/" + maxLevels.ToString(); // a
            costText.text = upgradeCosts[currentLevel].ToString();
        }
        else
        {
            //lvlText.text = "LVL MAX"; // b
            //costText.text = "NULL";
            costText.text = "0";
            costText.color = disalbedTextColor;
            costCurrencyImage.color = disalbedTextColor;
        }
    }

    protected virtual void UpdateAttackBar()
    {
    }
    protected virtual void UpdateCadenceBar()
    {
    }
    protected virtual void UpdateRangeBar()
    {
    }
    protected virtual void UpdateSupportBar()
    {
        fillBars[0].fillAmount = (float)supportLvl * supportFillBarCoef;        
    }


    protected virtual void DisableButtons()
    {
    }



    // Animations
    protected virtual void PlayOpenAnimation()
    {
    }


    protected virtual void PlayCloseAnimation()
    {
    }
    


    protected void FillStatBar(Image bar, Image button, Image backFillBar, float backFill)
    {
        bar.DOComplete();
        bar.DOFillAmount(1f, 0.2f);

        button.transform.DOComplete();
        button.transform.DOScale(Vector3.one * 1.1f, 0.2f);

        backFillBar.fillAmount = backFill;
    }


    protected void EmptyStatBar(Image bar, Image button, Image backFillBar, float backFill)
    {
        bar.DOComplete();
        bar.DOFillAmount(0f, 0.2f);

        button.transform.DOComplete();
        button.transform.DOScale(Vector3.one, 0.2f);

        backFillBar.fillAmount = backFill;
    }


}
