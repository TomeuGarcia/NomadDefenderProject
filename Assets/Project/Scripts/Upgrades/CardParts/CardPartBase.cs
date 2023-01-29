using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static TurretBuildingCard;

public class CardPartBase : CardPart
{
    [Header("CANVAS COMPONENTS")]
    [SerializeField] private TextMeshProUGUI playCostText;
    [SerializeField] private TextMeshProUGUI rangeText;
    [SerializeField] private Image rangeFillImage;
    [SerializeField] private Image basePassiveImage;

    [Header("PART")]
    [SerializeField] public TurretPartBase turretPartBase;
    [SerializeField] public TurretPassiveBase turretPassiveBase;

    [Header("VISUALS")]
    //[SerializeField] private MeshRenderer baseMeshRenderer;
    [SerializeField] private Image baseImage;
    private Material baseMaterial;

    private void OnValidate()
    {
        InitTexts();
    }

    private void Awake()
    {
        //baseMaterial = baseMeshRenderer.material;
        baseMaterial = new Material(baseImage.material);
        baseImage.material = baseMaterial;
    }

    public override void Init()
    {
        InitTexts();

        baseMaterial.SetTexture("_Texture", turretPartBase.materialTexture);
        baseMaterial.SetColor("_Color", turretPartBase.materialColor);

        rangeFillImage.fillAmount = turretPartBase.GetRangePer1();

        if (turretPassiveBase.passive.GetType() != typeof(BaseNullPassive))
        {
            basePassiveImage.transform.parent.gameObject.SetActive(true);

            basePassiveImage.sprite = turretPassiveBase.visualInformation.sprite;
            basePassiveImage.color = turretPassiveBase.visualInformation.color;
        }
        else
        {
            basePassiveImage.transform.parent.gameObject.SetActive(false);
        }
    }

    private void InitTexts()
    {
        playCostText.text = turretPartBase.cost.ToString();
        rangeText.text = turretPartBase.Range.ToString();

        //TODO : set passive texts
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
