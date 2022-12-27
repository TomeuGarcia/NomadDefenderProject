using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static BuildingCard;

public class TurretBuildingCard : BuildingCard
{
    [System.Serializable]
    public class TurretCardParts
    {
        public TurretCardParts(TurretPartAttack turretPartAttack, TurretPartBody turretPartBody,
               TurretPartBase turretPartBase, TurretPassiveBase turretPassiveBase)
        {
            this.turretPartAttack = turretPartAttack;
            this.turretPartBody = turretPartBody;
            this.turretPartBase = turretPartBase;
            this.turretPassiveBase = turretPassiveBase;
        }

        public TurretCardParts(TurretCardParts other)
        {
            this.turretPartAttack = other.turretPartAttack;
            this.turretPartBody = other.turretPartBody;
            this.turretPartBase = other.turretPartBase;
            this.turretPassiveBase = other.turretPassiveBase;
        }

        public TurretPartAttack turretPartAttack;
        public TurretPartBody turretPartBody;
        public TurretPartBase turretPartBase;
        public TurretPassiveBase turretPassiveBase;

        public int GetCostCombinedParts()
        {
            return turretPartAttack.cost + turretPartBody.cost + turretPartBase.cost + turretPassiveBase.cost;
        }
    }


    public TurretCardParts turretCardParts { get; private set; }

    private TurretBuilding.TurretBuildingStats turretStats;


    [Header("VISUALS")]
    [SerializeField] private MeshRenderer attackMeshRenderer;
    [SerializeField] private MeshRenderer bodyMeshRenderer;
    [SerializeField] private MeshRenderer baseMeshRenderer;
    private Material cardAttackMaterial, cardBodyMaterial, cardBaseMaterial;

    [SerializeField] private Image damageFillImage;
    [SerializeField] private Image cadenceFillImage;
    [SerializeField] private Image rangeFillImage;
    [SerializeField] private Image basePassiveImage;


    private void Awake()
    {
        AwakeInit(CardBuildingType.TURRET);
    }

    protected override void GetMaterialsRefs() 
    {
        cardAttackMaterial = attackMeshRenderer.material;
        cardBodyMaterial = bodyMeshRenderer.material;
        cardBaseMaterial = baseMeshRenderer.material;
    }

    protected override void InitVisuals()
    {
        TurretPartAttack turretPartAttack = turretCardParts.turretPartAttack;
        TurretPartBody turretPartBody = turretCardParts.turretPartBody;
        TurretPartBase turretPartBase = turretCardParts.turretPartBase;

        // Mesh Materials
        cardAttackMaterial.SetTexture("_Texture", turretPartAttack.materialTexture);
        cardAttackMaterial.SetColor("_Color", turretPartAttack.materialColor);

        cardBodyMaterial.SetTexture("_MaskTexture", turretPartBody.materialTextureMap);
        cardBodyMaterial.SetColor("_PaintColor", turretPartAttack.materialColor); // Projectile color

        cardBaseMaterial.SetTexture("_Texture", turretPartBase.materialTexture);
        cardBaseMaterial.SetColor("_Color", turretPartBase.materialColor);


        // Canvas
        damageFillImage.fillAmount = turretPartBody.GetDamagePer1();
        cadenceFillImage.fillAmount = turretPartBody.GetCadencePer1();
        rangeFillImage.fillAmount = turretPartBase.GetRangePer1();

        if (turretCardParts.turretPassiveBase.passive.GetType() != typeof(BaseNullPassive))
        {
            basePassiveImage.transform.parent.gameObject.SetActive(true);

            basePassiveImage.sprite = turretCardParts.turretPassiveBase.visualInformation.sprite;
            basePassiveImage.color = turretCardParts.turretPassiveBase.visualInformation.color;
        }
        else {
            basePassiveImage.transform.parent.gameObject.SetActive(false);
        }
    }

    protected override void InitStatsFromTurretParts()
    {
        turretStats.playCost = turretCardParts.GetCostCombinedParts();
        turretStats.damage = turretCardParts.turretPartBody.Damage;
        turretStats.range = turretCardParts.turretPartBase.Range;
        turretStats.cadence = turretCardParts.turretPartBody.Cadence;
    }

    public override void CreateCopyBuildingPrefab(CurrencyCounter currencyCounter)
    {
        copyBuildingPrefab = Instantiate(buildingPrefab, Vector3.zero, Quaternion.identity);
        copyBuildingPrefab.GetComponent<TurretBuilding>().Init(turretStats, turretCardParts, currencyCounter);
        copyBuildingPrefab.SetActive(false);
    }

    public override int GetCardPlayCost()
    {
        return turretStats.playCost;
    }


    public void ResetParts(TurretCardParts turretCardParts)
    {
        this.turretCardParts = new TurretCardParts(turretCardParts);
        Init();
    }


    public void SetNewPartAttack(TurretPartAttack newTurretPartAttack)
    {
        int costHolder = turretCardParts.turretPartAttack.cost;
        turretCardParts.turretPartAttack = newTurretPartAttack;
        turretCardParts.turretPartAttack.cost = costHolder;
        Init();
    }

    public void SetNewPartBody(TurretPartBody newTurretPartBody)
    {
        int costHolder = turretCardParts.turretPartBody.cost;
        turretCardParts.turretPartBody = newTurretPartBody;
        turretCardParts.turretPartBody.cost = costHolder;
        Init();
    }

    public void SetNewPartBase(TurretPartBase newTurretPartBase, TurretPassiveBase newTurretPassiveBase)
    {
        //int costHolder = turretCardParts.turretPartBase.cost + turretCardParts.turretPassiveBase.cost;
        int costHolder = turretCardParts.turretPartBase.cost + turretCardParts.turretPassiveBase.cost;

        turretCardParts.turretPartBase = newTurretPartBase;
        turretCardParts.turretPartBase.cost = costHolder;

        turretCardParts.turretPassiveBase = newTurretPassiveBase;
        turretCardParts.turretPassiveBase.cost = 0;

        Init();
    }

}
