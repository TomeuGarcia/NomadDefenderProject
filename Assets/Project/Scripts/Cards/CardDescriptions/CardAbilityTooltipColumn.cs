using System;
using System.Collections.Generic;
using UnityEngine;

public class CardAbilityTooltipColumn : MonoBehaviour
{
    [SerializeField] private Transform _tooltipsContainer;

    private List<CardAbilityTooltip> _tooltips;

    private void Awake()
    {
        _tooltips = new List<CardAbilityTooltip>(4);
    }

    public void AddTooltip(CardAbilityTooltip cardAbilityTooltip)
    {
        cardAbilityTooltip.ParentToContainer(_tooltipsContainer);
    }

    
    public void ShowTooltips()
    {
        foreach (CardAbilityTooltip tooltip in _tooltips)
        {
            tooltip.Show();
        }
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