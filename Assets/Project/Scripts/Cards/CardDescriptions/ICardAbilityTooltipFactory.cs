using UnityEngine;

public interface ICardAbilityTooltipFactory
{
    CardAbilityTooltip CreateAbilityTooltip();
    CardAbilityTooltip CreateKeywordTooltip();
}