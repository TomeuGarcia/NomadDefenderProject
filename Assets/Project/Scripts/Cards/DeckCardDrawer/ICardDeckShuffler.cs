using System.Collections.Generic;
using UnityEngine;

public interface ICardDeckShuffler
{
    List<BuildingCard> ShuffleCards(List<BuildingCard> cards);
}