using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardPartBody : CardPart
{
    [Header("CANVAS COMPONENTS")]
    [SerializeField] private TextMeshProUGUI playCostText;
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private TextMeshProUGUI cadenceText;

    [Header("PART")]
    [SerializeField] public TurretPartBody turretPartBody;

    private void OnValidate()
    {
        InitTexts();
    }

    public override void Init()
    {
        InitTexts();
    }

    private void InitTexts()
    {
        playCostText.text = turretPartBody.cost.ToString();
        damageText.text = turretPartBody.damage.ToString();
        cadenceText.text = turretPartBody.attackSpeed.ToString();
    }
}
