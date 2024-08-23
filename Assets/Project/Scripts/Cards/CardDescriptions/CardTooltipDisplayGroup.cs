using UnityEngine;



public class CardTooltipDisplayGroup : MonoBehaviour
{
    [Header("COMPONENTS")]
    [SerializeField] private CardAbilityTooltipColumn _abilitiesColumn;
    [SerializeField] private CardAbilityTooltipColumn _keywordsColumn;


    private ICardAbilityTooltipFactory _cardAbilityTooltipFactory;
    

    public void Configure(ICardAbilityTooltipFactory cardAbilityTooltipFactory)
    {
        _cardAbilityTooltipFactory = cardAbilityTooltipFactory;
    }
    
    
    public void DisplayTooltips(ICardDescriptionProvider descriptionProvider)
    {
        // for all abilities - add to abilities column
            // for all keywords - add to keywords column


            descriptionProvider.GetAbilityDescriptionSetupData();
    }
    
}