using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardPartBase : CardPart
{
    [Header("CANVAS COMPONENTS")]
    [SerializeField] private TextMeshProUGUI playCostText;
    [SerializeField] private TextMeshProUGUI rangeText;
    [SerializeField] private Image rangeFillImage;
    [SerializeField] private Image baseAbilityImage;

    [Header("PART")]
    [SerializeField] public TurretPartBase turretPartBase;

    [Header("VISUALS")]
    [SerializeField] private MeshRenderer baseMeshRenderer;
    private Material baseMaterial;

    private void OnValidate()
    {
        InitTexts();
    }

    private void Awake()
    {
        baseMaterial = baseMeshRenderer.material;
    }

    public override void Init()
    {
        InitTexts();

        baseMaterial.SetTexture("_Texture", turretPartBase.materialTexture);
        baseMaterial.SetColor("_Color", turretPartBase.materialColor);

        bool hasAbility = turretPartBase.HasAbilitySprite();
        baseAbilityImage.transform.parent.gameObject.SetActive(hasAbility);
        if (hasAbility)
        {
            baseAbilityImage.sprite = turretPartBase.abilitySprite;
            baseAbilityImage.color = turretPartBase.spriteColor;
        }
    }

    private void InitTexts()
    {
        playCostText.text = turretPartBase.cost.ToString();
        rangeText.text = turretPartBase.Range.ToString();
    }
}
