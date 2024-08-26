using UnityEngine;



public class CardTooltipDisplayGroup : MonoBehaviour
{
    [Header("COMPONENTS")]
    [SerializeField] private CardAbilityTooltipColumn _abilitiesColumn;
    [SerializeField] private CardAbilityTooltipColumn _keywordsColumn;

    public bool IsDisplaying { get; private set; }
    
    
    public void StartDisplayingTooltips(
        ICardAbilityTooltipFactory tooltipFactory,
        CardAbilityTooltip.Content[] abilityContents,
        CardAbilityTooltip.Content[] keywordContents,
        Camera displayCamera,
        CardTooltipDisplayData.Positioning displayPositioning)
    {
        gameObject.SetActive(true);
        SpawnTooltips(tooltipFactory, abilityContents, keywordContents);
        PositionTooltips(displayCamera, displayPositioning);
        IsDisplaying = true;
    }

    private void SpawnTooltips(
        ICardAbilityTooltipFactory tooltipFactory,
        CardAbilityTooltip.Content[] abilityContents,
        CardAbilityTooltip.Content[] keywordContents)
    {
        foreach (CardAbilityTooltip.Content abilityContent in abilityContents)
        {
            CardAbilityTooltip abilityTooltip = tooltipFactory.CreateAbilityTooltip();
            abilityTooltip.SetContent(abilityContent);
            _abilitiesColumn.AddTooltip(abilityTooltip);
        }
        
        foreach (CardAbilityTooltip.Content keywordContent in keywordContents)
        {
            CardAbilityTooltip keywordTooltip = tooltipFactory.CreateKeywordTooltip();
            keywordTooltip.SetContent(keywordContent);
            _keywordsColumn.AddTooltip(keywordTooltip);
        }
        
        _abilitiesColumn.ShowTooltips();
        _keywordsColumn.ShowTooltips();
    }

    private void PositionTooltips(Camera displayCamera, CardTooltipDisplayData.Positioning displayPositioning)
    {
        // TODO
        displayPositioning.TODO_GetCanvasDisplayPosition(displayCamera,
            out bool displayRightSide, out Vector3 displayPosition);
    }

    public void StopDisplayingTooltips()
    {
        _abilitiesColumn.HideAndClearTooltips();
        _keywordsColumn.HideAndClearTooltips();
        gameObject.SetActive(false);
        IsDisplaying = false;
    }
}