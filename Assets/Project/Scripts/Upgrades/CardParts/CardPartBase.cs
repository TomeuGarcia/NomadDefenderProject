using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardPartBase : MonoBehaviour
{
    [Header("CANVAS COMPONENTS")]
    [SerializeField] private TextMeshProUGUI playCostText;
    [SerializeField] private TextMeshProUGUI rangeText;

    [Header("PART")]
    [SerializeField] public TurretPartBase turretPartBase;

    private void OnValidate()
    {
        InitTexts();
    }

    private void InitTexts()
    {
        playCostText.text = turretPartBase.cost.ToString();
        rangeText.text = turretPartBase.attackRange.ToString();
    }
}
