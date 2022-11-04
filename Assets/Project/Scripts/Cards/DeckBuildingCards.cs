using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckBuildingCards : MonoBehaviour
{
    [SerializeField] private List<BuildingCard> cards;


    private void Awake()
    {
        InitCardsInDeck();
    }


    private void InitCardsInDeck()
    {

        float upStep = 0.1f;
        float numCards = cards.Count;

        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].transform.SetParent(transform);

            Quaternion rotation = Quaternion.FromToRotation(cards[i].transform.forward, -transform.up);
            cards[i].transform.rotation = rotation;      
            cards[i].transform.localPosition = Vector3.zero;
            cards[i].transform.position += transform.up * (upStep * (numCards - (i+1)));
        }

    }


    public bool HasCardsLeft()
    {
        return cards.Count > 0;
    }

    public BuildingCard GetTopCard()
    {
        BuildingCard topCard = cards[0];

        cards.RemoveAt(0);

        return topCard;
    }





}
