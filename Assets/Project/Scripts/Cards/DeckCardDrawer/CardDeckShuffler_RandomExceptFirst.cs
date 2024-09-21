using System.Collections.Generic;
using UnityEngine;

public class CardDeckShuffler_RandomExceptFirst : ICardDeckShuffler
{

    public List<BuildingCard> ShuffleCards(List<BuildingCard> cards)
    {
        List<BuildingCard> shuffledCards = new List<BuildingCard>(cards.Count) { cards[0] };
        cards.RemoveAt(0);

        while (cards.Count > 0)
        {
            int randomIndex = Random.Range(0, cards.Count);
            BuildingCard card = cards[randomIndex];
            cards.RemoveAt(randomIndex);
            shuffledCards.Add(card);
        }

        return shuffledCards;
    }
    
}