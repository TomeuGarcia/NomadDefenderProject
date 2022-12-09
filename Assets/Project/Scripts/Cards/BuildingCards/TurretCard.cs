using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurretCard : BuildingCard
{
    [Header("VISUALS")]
    [SerializeField] private MeshRenderer attackMeshRenderer;
    [SerializeField] private MeshRenderer bodyMeshRenderer;
    [SerializeField] private MeshRenderer baseMeshRenderer;
    private Material cardAttackMaterial, cardBodyMaterial, cardBaseMaterial;

    [SerializeField] private Image damageFillImage;
    [SerializeField] private Image cadenceFillImage;
    [SerializeField] private Image rangeFillImage;
    [SerializeField] private Image baseAbilityImage;



    protected override void GetMaterialsRefs() 
    {
        cardAttackMaterial = attackMeshRenderer.material;
        cardBodyMaterial = bodyMeshRenderer.material;
        cardBaseMaterial = baseMeshRenderer.material;
    }

    protected override void InitVisuals()
    {
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

        bool hasAbility = turretPartBase.HasAbilitySprite();
        baseAbilityImage.transform.parent.gameObject.SetActive(hasAbility);
        if (hasAbility)
        {
            baseAbilityImage.sprite = turretPartBase.abilitySprite;
            baseAbilityImage.color = turretPartBase.spriteColor;
        }

    }


}
