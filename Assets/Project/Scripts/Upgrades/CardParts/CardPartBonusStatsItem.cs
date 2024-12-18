using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CardPartBonusStatsItem : MonoBehaviour
{
    [SerializeField] private TMP_Text _valueText;

    public void Init(TurretStatsUpgradeModel.StatString statString, bool isDebuff = false, string textSuffix = "")
    {
        if (statString.IsNull)
        {
            gameObject.SetActive(false);
            return;
        }

        _valueText.text = statString.Value + textSuffix;
        if (isDebuff)
        {
            _valueText.color = new Color(0.9f, 0.4f, 0.1f);  
        }
        else
        {
            transform.SetAsFirstSibling();
        }
        
    }


}
