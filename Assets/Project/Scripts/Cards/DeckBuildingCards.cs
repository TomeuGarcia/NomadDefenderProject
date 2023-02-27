using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeckBuildingCards : MonoBehaviour
{
    [SerializeField] private DeckData deckData;
    
    private List<BuildingCard> cards;

    public List<BuildingCard> Cards => cards;


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
            cards[i].RootCardTransform.SetParent(transform);

            Quaternion rotation = Quaternion.FromToRotation(cards[i].RootCardTransform.forward, -transform.up);
            cards[i].RootCardTransform.rotation = rotation;      
            cards[i].RootCardTransform.localPosition = Vector3.zero;
            cards[i].RootCardTransform.position += transform.up * (upStep * (numCards - (i+1)));

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

            cards[i].RootCardTransform.SetParent(cardsHolder);
            cards[i].RootCardTransform.localPosition = offset;
            cards[i].RootCardTransform.localRotation = Quaternion.identity;
        }
    }


    public bool HasCardsLeft()
    {
        return cards.Count > 0;
    }

    public Transform GetTopCardTransform()
    {
        return cards[0].RootCardTransform;
    }

    public BuildingCard GetTopCard()
    {
        return GetAndRemoveCard(0);
    }
    public BuildingCard GetRandomCard()
    {
        return GetAndRemoveCard(Random.Range(0, cards.Count));
    }
    public BuildingCard GetAndRemoveCard(int cardI)
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

    public void AddCardToDeckTop(BuildingCard card)
    {
        cards.Insert(0, card);
        ArrangeCards();
    }



    public DeckData GetDeckData()
    {
        return deckData;
    }

}
