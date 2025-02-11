using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ResultScreenStat : MonoBehaviour
{
    [SerializeField] private TextDecoder _nameText;
    [SerializeField] private TextDecoder _valueText;

    public void Init(string statName, string statValue)
    {
        _nameText.ClearText();
        _nameText.SetTextStrings(statName);
        
        _valueText.ClearText();
        _valueText.SetTextStrings(statValue);
    }

    public IEnumerator PlayAnimation()
    {
        _nameText.Activate();
        yield return new WaitUntil(() => _nameText.FinishedLine);
        
        _valueText.Activate();
        _valueText.transform.DOPunchScale(Vector3.one * 0.5f, 0.25f, 5);
        yield return new WaitUntil(() => _valueText.FinishedLine);

        yield return new WaitForSeconds(0.1f);
    }

    public void CompleteAnimation()
    {
        _nameText.SetStringInstantly();
        _valueText.SetStringInstantly();
    }
}
