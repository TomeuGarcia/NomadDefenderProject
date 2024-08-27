using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


public class CardTooltipDisplayGroup : MonoBehaviour
{
    [Header("COMPONENTS")]
    [SerializeField] private HorizontalLayoutGroup _columnsLayoutGroup;
    [SerializeField] private RectTransform _columnsHolder;
    [SerializeField] private CanvasGroup _positioningHideCanvasGroup;
    [SerializeField] private CardAbilityTooltipColumn _abilitiesColumn;
    [SerializeField] private CardAbilityTooltipColumn _keywordsColumn;

    public bool IsDisplaying { get; private set; }

    private static AnchorPositioning RIGHT_POSITIONING = new AnchorPositioning(
        new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1));
    private static AnchorPositioning LEFT_POSITIONING = new AnchorPositioning(
        new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1));
    
    private class AnchorPositioning
    {
        private readonly Vector2 _pivot;
        private readonly Vector2 _anchorMin;
        private readonly Vector2 _anchorMax;

        public AnchorPositioning(Vector2 pivot, Vector2 anchorMin, Vector2 anchorMax)
        {
            _pivot = pivot;
            _anchorMin = anchorMin;
            _anchorMax = anchorMax;
        }

        public void Apply(RectTransform transform)
        {
            transform.pivot = _pivot;
            transform.anchorMin = _anchorMin;
            transform.anchorMax = _anchorMax;
        }
    }
    
    
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
        displayPositioning.TODO_GetCanvasDisplayPosition(displayCamera,
            out bool displayRightSide, out Vector3 displayPosition);

        if (displayRightSide)
        {
            _columnsLayoutGroup.childAlignment = TextAnchor.UpperLeft;
            _columnsLayoutGroup.reverseArrangement = false;
            RIGHT_POSITIONING.Apply(_columnsHolder);
        }
        else 
        {
            _columnsLayoutGroup.childAlignment = TextAnchor.UpperRight;
            _columnsLayoutGroup.reverseArrangement = true;
            LEFT_POSITIONING.Apply(_columnsHolder);
        }
        
        FixPositioning(displayPosition);
    }

    private async void FixPositioning(Vector3 displayPosition)
    {
        _positioningHideCanvasGroup.alpha = 0;
        await Task.Yield();
        await Task.Yield();
        _positioningHideCanvasGroup.alpha = 1;
        

        float screenOverflowMargin = 32;
        float screenOverflow = GetLowestContentPosition(displayPosition).y - screenOverflowMargin;

        if (screenOverflow < 0)
        {
            displayPosition += Vector3.up * (-screenOverflow );
        }
        
        transform.position = displayPosition;
    }

    private Vector2 GetLowestContentPosition(Vector3 displayPosition)
    {
        RectTransform abilitiesBottomContent = _abilitiesColumn.BottomTooltipContentHolder;
        RectTransform keywordsBottomContent = _keywordsColumn.BottomTooltipContentHolder;
        RectTransform lowestContent = abilitiesBottomContent.localPosition.y < keywordsBottomContent.localPosition.y
            ? abilitiesBottomContent
            : keywordsBottomContent;
        
        Vector3 lowestPosition = lowestContent.localPosition + displayPosition;
        Vector2 min = lowestContent.rect.min + new Vector2(lowestPosition.x, lowestPosition.y);
        return min;
    }
    
    
    public void StopDisplayingTooltips()
    {
        _abilitiesColumn.HideAndClearTooltips();
        _keywordsColumn.HideAndClearTooltips();
        gameObject.SetActive(false);
        IsDisplaying = false;
    }
}