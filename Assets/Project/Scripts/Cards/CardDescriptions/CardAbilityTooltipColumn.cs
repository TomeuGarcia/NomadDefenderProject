using UnityEngine;

public class CardAbilityTooltipColumn : MonoBehaviour
{
    [SerializeField] private Transform _tooltipsContainer;



    public void AddTooltip(CardAbilityTooltip cardAbilityTooltip)
    {
        cardAbilityTooltip.ParentToContainer(_tooltipsContainer);
    }
    
    
}