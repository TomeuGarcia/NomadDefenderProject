using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDrawer : MonoBehaviour
{
    [SerializeField] private HandBuildingCards hand;
    [SerializeField] private DeckBuildingCards deck;

    [SerializeField, Min(0)] private int numCardsHandStart = 2;


    private void OnEnable()
    {
        HandBuildingCards.OnQueryDrawCard += TryDrawCard;
    }

    private void OnDisable()
    {
        HandBuildingCards.OnQueryDrawCard -= TryDrawCard;
    }

    private void Start()
    {
        deck.Init();      
        DrawStartHand();
        hand.Init();

        deck.GetDeckData().SetSavedCardsToStarterCards();
    }


    private void TryDrawCard()
    {
        if (deck.HasCardsLeft())
            DrawCard();
    }

    private void DrawCard()
    {
        hand.AddCard(deck.GetTopCard());
    }


    private void DrawStartHand()
    {
        for (int i = 0; i < numCardsHandStart; ++i)
        {
            TryDrawCard();
        }
    }

}
