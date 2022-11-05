using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDrawer : MonoBehaviour
{
    [SerializeField] private HandBuildingCards hand;
    [SerializeField] private DeckBuildingCards deck;


    private void OnEnable()
    {
        HandBuildingCards.OnQueryDrawCard += TryDrawCard;
    }

    private void OnDisable()
    {
        HandBuildingCards.OnQueryDrawCard -= TryDrawCard;
    }



    private void TryDrawCard()
    {
        if (deck.HasCardsLeft())
            DrawCard();
    }

    public void DrawCard()
    {
        hand.AddCard(deck.GetTopCard());
    }



}
