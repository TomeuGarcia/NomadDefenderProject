using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardPartAttack : CardPart
{
    [Header("CANVAS COMPONENTS")]
    [SerializeField] private TextMeshProUGUI playCostText;

    [Header("PART")]
    [SerializeField] public TurretPartAttack turretPartAttack;

    [Header("VISUALS")]
    [SerializeField] private MeshRenderer attackMeshRenderer;
    private Material attackMaterial;


    private void OnValidate()
    {
        InitTexts();
    }

    private void Awake()
    {
        attackMaterial = attackMeshRenderer.material;
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

}
