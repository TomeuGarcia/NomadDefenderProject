using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardPartBonusStatsItem : MonoBehaviour
{
    [SerializeField] private TMP_Text _valueText;


    public void Init(TurretStatsUpgradeModel.StatString statString)
    {
        if (statString.IsNull)
        {
            gameObject.SetActive(false);
            return;
        }

        _valueText.text = statString.Value;
    }


}
