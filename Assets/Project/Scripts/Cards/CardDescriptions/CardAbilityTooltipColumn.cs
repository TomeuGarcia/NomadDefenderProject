using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardAbilityTooltipColumn : MonoBehaviour
{
    [SerializeField] private RectTransform _tooltipsParent;

    private List<CardAbilityTooltip> _tooltips;

    public RectTransform BottomTooltipContentHolder
    {
        get
        {
            if (_tooltips.Count < 1)
            {
                return _tooltipsParent;
            }
            return _tooltips[^1].ContentHolder;
        }
    }

    private void Awake()
    {
        _tooltips = new List<CardAbilityTooltip>(4);
    }

    public void AddTooltip(CardAbilityTooltip cardAbilityTooltip)
    {
        cardAbilityTooltip.ParentToContainer(_tooltipsParent);
        _tooltips.Add(cardAbilityTooltip);
    }

    
    public void ShowTooltips()
    {
        foreach (CardAbilityTooltip tooltip in _tooltips)
        {
            tooltip.Show();
        }
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(_tooltipsParent);
    }
    public void HideAndClearTooltips()
    {
        foreach (CardAbilityTooltip tooltip in _tooltips)
        {
            tooltip.Hide();
        }

        _tooltips.Clear();
    }
    
}