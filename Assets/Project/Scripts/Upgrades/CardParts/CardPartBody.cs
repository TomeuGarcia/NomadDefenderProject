using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardPartBody : CardPart
{
    [Header("CANVAS COMPONENTS")]
    [SerializeField] private TextMeshProUGUI playCostText;
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private TextMeshProUGUI cadenceText;
    [SerializeField] private Image damageFillImage;
    [SerializeField] private Image cadenceFillImage;

    [Header("PART")]
    [SerializeField] public TurretPartBody turretPartBody;

    [Header("VISUALS")]
    //[SerializeField] private MeshRenderer bodyMeshRenderer;
    [SerializeField] private Image bodyImage;
    private Material bodyMaterial;



    private void OnValidate()
    {
        InitTexts();
    }

    private void Awake()
    {
        //bodyMaterial = bodyMeshRenderer.material;
        bodyMaterial = new Material(bodyImage.material);
        bodyImage.material = bodyMaterial;
    }

    public override void Init()
    {
        InitTexts();

        bodyMaterial.SetTexture("_MaskTexture", turretPartBody.materialTextureMap);
        //bodyMaterial.SetColor("_PaintColor", turretPartAttack.materialColor); // Projectile color     ???? WHAT TO DO ????

        damageFillImage.fillAmount = turretPartBody.GetDamagePer1();
        cadenceFillImage.fillAmount = turretPartBody.GetCadencePer1();
    }

    private void InitTexts()
    {
        playCostText.text = turretPartBody.cost.ToString();
        damageText.text = turretPartBody.Damage.ToString();
        cadenceText.text = turretPartBody.Cadence.ToString();
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
