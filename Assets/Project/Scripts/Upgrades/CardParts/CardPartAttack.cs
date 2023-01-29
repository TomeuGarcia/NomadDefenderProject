using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardPartAttack : CardPart
{
    [Header("CANVAS COMPONENTS")]
    [SerializeField] private TextMeshProUGUI playCostText;

    [Header("PART")]
    [SerializeField] public TurretPartAttack turretPartAttack;

    [Header("VISUALS")]
    //[SerializeField] private MeshRenderer attackMeshRenderer;
    [SerializeField] private Image attackImage;
    private Material attackMaterial;


    private void OnValidate()
    {
        InitTexts();
    }

    private void Awake()
    {
        //attackMaterial = attackMeshRenderer.material;
        attackMaterial = new Material(attackImage.material);
        attackImage.material = attackMaterial;
    }

    public override void Init()
    {
        InitTexts();

        attackMaterial.SetTexture("_Texture", turretPartAttack.materialTexture);
        attackMaterial.SetColor("_Color", turretPartAttack.materialColor);
    }

    private void InitTexts()
    {
        playCostText.text = turretPartAttack.cost.ToString();
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
