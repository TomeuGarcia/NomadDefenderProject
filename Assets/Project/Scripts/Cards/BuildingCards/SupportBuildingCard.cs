using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SupportBuildingCard : BuildingCard
{
    [System.Serializable]
    public class SupportCardParts
    {
        public TurretPartBase turretPartBase;


        public SupportCardParts(TurretPartBase turretPartBase)
        {

            this.turretPartBase = turretPartBase;
        }
      
        public SupportCardParts(SupportCardParts other)
        {
            this.turretPartBase = other.turretPartBase;
        }

        public int GetCostCombinedParts()
        {
            return turretPartBase.cost;
        }
    }


    public SupportCardParts supportCardParts { get; private set; }

    private SupportBuilding.SupportBuildingStats supportBuildingStats;

    [Header("VISUALS")]
    //[SerializeField] private MeshRenderer baseMeshRenderer;
    [SerializeField] private Image baseImage;
    private Material material;
    [SerializeField] private Image rangeFillImage;
    [SerializeField] private Image abilityImage;



    public void Awake()
    {
        AwakeInit(CardBuildingType.SUPPORT);
    }
    public override void CreateCopyBuildingPrefab(Transform spawnTransform, CurrencyCounter currencyCounter)
    {
        copyBuildingPrefab = Instantiate(buildingPrefab, Vector3.zero, Quaternion.identity);
        copyBuildingPrefab.transform.SetParent(spawnTransform);

        copyBuildingPrefab.GetComponent<SupportBuilding>().Init(supportBuildingStats, supportCardParts.turretPartBase, currencyCounter);
        copyBuildingPrefab.SetActive(false);
    }

    public override int GetCardPlayCost()
    {
        return supportBuildingStats.playCost;
    }
    public void ResetParts(SupportCardParts supportCardParts)
    {
        this.supportCardParts = new SupportCardParts(supportCardParts);
        Init();
    }
    protected override void GetMaterialsRefs()
    {
        //material = baseMeshRenderer.material;
        material = new Material(baseImage.material);
        baseImage.material = material;
    }

    protected override void InitStatsFromTurretParts()
    {
        supportBuildingStats.playCost = supportCardParts.GetCostCombinedParts();
        supportBuildingStats.range = supportCardParts.turretPartBase.Range;
    }

    protected override void InitVisuals()
    {
        TurretPartBase turretPartBase = supportCardParts.turretPartBase;
        //Mesh Materials
        material.SetTexture("_Texture", turretPartBase.materialTexture);
        material.SetColor("_Color", turretPartBase.materialColor);

        // Canvas
        rangeFillImage.fillAmount = turretPartBase.GetRangePer1();
        abilityImage.transform.parent.gameObject.SetActive(true);

        abilityImage.sprite = turretPartBase.abilitySprite;
        abilityImage.color = turretPartBase.spriteColor;
    }
    public void SetNewPartBase(TurretPartBase newPartBase)
    {
        int costHolder = supportCardParts.turretPartBase.cost;
        supportCardParts.turretPartBase = newPartBase;
        supportCardParts.turretPartBase.cost = costHolder;
        Init();
    }

    public override void ShowInfo()
    {
        base.ShowInfo();
        interfaceCanvasGroup.alpha = 0f;
    }
    public override void HideInfo()
    {
        base.HideInfo();
        interfaceCanvasGroup.alpha = 1f;
    }
}
