
using UnityEngine;

[CreateAssetMenu(fileName = "CardDeck_NAME", 
    menuName = SOAssetPaths.CARDS_DECKS + "CardDeckAsset")]
public class CardDeckAsset : ScriptableObject
{
    [SerializeField] private TurretCardDataModel[] _turretCards;
    [SerializeField] private SupportCardDataModel[] _supportCards;

    public CardDeckContent MakeDeckContent()
    {
        return new CardDeckContent(_turretCards, _supportCards);
    }

    public TurretCardDataModel MainTurretCardDataModel()
    {
        return _turretCards[^1];
    }
    public SupportCardDataModel MainSupportCardDataModel()
    {
        return _supportCards[^1];
    }
}