using System.Collections.Generic;
using UnityEngine;

public class CardDeckShuffler_HardcodedIndex : MonoBehaviour, ICardDeckShuffler
{
    [SerializeField] private int[] _indices;

    public List<BuildingCard> ShuffleCards(List<BuildingCard> cards)
    {
        List<BuildingCard> shuffledCards = new List<BuildingCard>(cards.Count);

        foreach (int index in _indices)
        {
            shuffledCards.Add(cards[index]);
        }
        
        return shuffledCards;
    }
}