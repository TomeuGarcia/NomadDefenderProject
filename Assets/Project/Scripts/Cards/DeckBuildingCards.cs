using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeckBuildingCards : MonoBehaviour
{
    [SerializeField] private DeckData deckData;
    
    private List<BuildingCard> cards;


    [Header("DECK")]
    [SerializeField] private Transform cardsHolder;

    public int NumCards => cards.Count;


    public void Init()
    {
        cards = new List<BuildingCard>(deckData.GetCards());

        float upStep = 0.1f;
        float numCards = cards.Count;

        for (int i = 0; i < cards.Count; ++i)
        {
            cards[i].CardTransform.SetParent(transform);

            Quaternion rotation = Quaternion.FromToRotation(cards[i].CardTransform.forward, -transform.up);
            cards[i].CardTransform.rotation = rotation;      
            cards[i].CardTransform.localPosition = Vector3.zero;
            cards[i].CardTransform.position += transform.up * (upStep * (numCards - (i+1)));

            cards[i].cardLocation = BuildingCard.CardLocation.DECK;

            cards[i].DisableMouseInteraction();
        }


        ArrangeCards();
    }

    private void ArrangeCards()
    {
        for (int i = 0; i < cards.Count; ++i)
        {
            Vector3 offset = cardsHolder.forward * (i * 0.15f);

            cards[i].CardTransform.SetParent(cardsHolder);
            cards[i].CardTransform.localPosition = offset;
            cards[i].CardTransform.localRotation = Quaternion.identity;
        }
    }


    public bool HasCardsLeft()
    {
        return cards.Count > 0;
    }

    public Transform GetTopCardTransform()
    {
        return cards[0].CardTransform;
    }

    public BuildingCard GetTopCard()
    {
        return GetCard(0);
    }
    public BuildingCard GetRandomCard()
    {
        return GetCard(Random.Range(0, cards.Count));
    }
    public BuildingCard GetCard(int cardI)
    {
        BuildingCard topCard = cards[cardI];

        cards.RemoveAt(cardI);

        ArrangeCards();

        return topCard;
    }

    public void AddCardToDeckBottom(BuildingCard card)
    {
        cards.Add(card);
        ArrangeCards();
    }



    public DeckData GetDeckData()
    {
        return deckData;
    }

}
