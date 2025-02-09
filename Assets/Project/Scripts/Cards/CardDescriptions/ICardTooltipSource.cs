using UnityEngine;

public interface ICardTooltipSource
{
    CardTooltipDisplayData MakeTooltipDisplayData();
    bool WithKeywords();
}
