using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DeckData", menuName = "Cards/DeckData")]
public class DeckData : ScriptableObject
{
    private List<BuildingCard> cards;

    [SerializeField] public List<BuildingCard.CardComponents> starterCardsComponents;



    public void Init(BuildingCard[] starterCards)
    {
        cards = new List<BuildingCard>(starterCards);
    }

    public BuildingCard[] GetCards()
    {
        return cards.ToArray();
    }

    public List<BuildingCard> GetCardsReference()
    {
        return cards;
    }

    public void AddCard(BuildingCard card)
    {
        cards.Add(card);
    }

    public void RemoveCard(int index)
    {
        cards.RemoveAt(index);
        starterCardsComponents.RemoveAt(index);
    }


    public void ResetComponents(List<BuildingCard.CardComponents> cardsComponentsTemp)
    {
        starterCardsComponents = new List<BuildingCard.CardComponents>(cardsComponentsTemp);
    }

}
