using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CardPartBonusStatsItem : MonoBehaviour
{
    [SerializeField] private TMP_Text _valueText;

    public void Init(TurretStatsUpgradeModel.StatString statString, string textSuffix = "")
    {
        if (statString.IsNull)
        {
            gameObject.SetActive(false);
            return;
        }

        _valueText.text = statString.Value + textSuffix;
    }


}
