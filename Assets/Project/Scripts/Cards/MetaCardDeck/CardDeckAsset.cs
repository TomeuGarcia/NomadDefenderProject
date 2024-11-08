
using UnityEngine;

[CreateAssetMenu(fileName = "CardDeck_NAME", 
    menuName = SOAssetPaths.CARDS_DECKS + "CardDeckAsset")]
public class CardDeckAsset : ScriptableObject
{
    [SerializeField] private string _deckName;
    
    [SerializeField] private TurretCardDataModel[] _turretCards;
    [SerializeField] private SupportCardDataModel[] _supportCards;

    public string DeckName => _deckName;
    
    public CardDeckContent MakeDeckContent()
    {
        return new CardDeckContent(_turretCards, _supportCards);
    }
    
    public SupportCardDataModel MainSupportCardDataModel()
    {
        return _supportCards[^1];
    }
}