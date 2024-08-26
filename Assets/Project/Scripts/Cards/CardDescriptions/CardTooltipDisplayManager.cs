using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CardTooltipDisplayManager : MonoBehaviour
{
    [SerializeField] private CardAbilityTooltipFactory _tooltipFactory;
    [SerializeField] private CardTooltipDisplayGroup _tooltipDisplayGroup;
    
    private static CardTooltipDisplayManager _instance;
    private Camera _displayCamera;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }
    
    public static CardTooltipDisplayManager GetInstance()
    {
        return _instance;
    }
    
    public void SetDisplayCamera(Camera displayCamera)
    {
        _displayCamera = displayCamera;
    }

    
    public void StartDisplayingTooltip(ICardTooltipSource descriptionProvider)
    {
        CardTooltipDisplayData displayData = descriptionProvider.MakeTooltipDisplayData();
            
        MakeTooltipContentsForAbilitiesAndKeywords(displayData.Elements,
            out CardAbilityTooltip.Content[] abilityContents,
            out CardAbilityTooltip.Content[] keywordContents);
        
        _tooltipDisplayGroup.StartDisplayingTooltips(_tooltipFactory, abilityContents, keywordContents, 
            _displayCamera, displayData.DisplayPositioning);
    }

    public void StopDisplayingTooltip()
    {
        if (_tooltipDisplayGroup.IsDisplaying)
        {
            _tooltipDisplayGroup.StopDisplayingTooltips();
        }
    }
    

    private void MakeTooltipContentsForAbilitiesAndKeywords(CardTooltipDisplayData.Element[] displayDataElements,
        out CardAbilityTooltip.Content[] abilityContents, 
        out CardAbilityTooltip.Content[] keywordContents)
    {
        HashSet<CardAbilityKeyword> keywordsSet = new HashSet<CardAbilityKeyword>();
        
        abilityContents = new CardAbilityTooltip.Content[displayDataElements.Length];
        for (int i = 0; i < displayDataElements.Length; ++i)
        {
            CardTooltipDisplayData.Element displayDataElement = displayDataElements[i];
            CardAbilityTooltip.Content tooltipContent = CardAbilityTooltip.Content.MakeForPassiveAbility(
                displayDataElement.AbilityDescription.Name, displayDataElement.AbilityDescription.Description,
                displayDataElement.AbilitySprite, displayDataElement.AbilityColor);
            
            abilityContents[i] = tooltipContent;

            foreach (CardAbilityKeyword abilityKeyword in displayDataElement.AbilityDescription.Keywords)
            {
                keywordsSet.Add(abilityKeyword);
            }
        }
        
        
        keywordContents = new CardAbilityTooltip.Content[keywordsSet.Count];
        int keywordsI = 0;
        foreach (CardAbilityKeyword abilityKeyword in keywordsSet)
        {
            keywordContents[keywordsI++] = 
                CardAbilityTooltip.Content.MakeForAbilityKeyword(abilityKeyword.Name, abilityKeyword.Description);
        }
    }
    
}