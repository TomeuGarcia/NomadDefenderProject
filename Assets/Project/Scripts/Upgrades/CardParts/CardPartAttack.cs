using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardPartAttack : CardPart
{
    [Header("CANVAS COMPONENTS")]
    [SerializeField] private TextMeshProUGUI playCostText;
    [SerializeField] private TextMeshProUGUI targetAmountText;

    [Header("PART")]
    [SerializeField] public TurretPartAttack turretPartAttack;

    private void OnValidate()
    {
        InitTexts();
    }

    private void InitTexts()
    {
        playCostText.text = turretPartAttack.cost.ToString();
        targetAmountText.text = turretPartAttack.targetAmount.ToString();
    }

}
