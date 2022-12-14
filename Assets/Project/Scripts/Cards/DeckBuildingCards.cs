using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeckBuildingCards : MonoBehaviour
{
    [SerializeField] private DeckData deckData;
    
    private List<BuildingCard> cards;


    [Header("Fake Deck")]
    [SerializeField] private Transform fakeDeck;
    [SerializeField] private GameObject fakeCardPrefab;
    private GameObject[] fakeCards;
    private int lastFakeCardI;
    //[SerializeField] private TextMeshPro cardCountText;


    public void Init()
    {
        cards = new List<BuildingCard>(deckData.GetCards());

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


        fakeCards = new GameObject[cards.Count];
        for (int i = 0; i < fakeCards.Length; i++)
        {
            Vector3 offset = -fakeDeck.forward * (i * 0.15f);

            fakeCards[i] = Instantiate(fakeCardPrefab, fakeDeck);
            fakeCards[i].transform.localPosition = offset;
        }
        lastFakeCardI = fakeCards.Length - 1;

        //cardCountText.text = cards.Count.ToString();
    }


    public bool HasCardsLeft()
    {
        return cards.Count > 0;
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

        fakeCards[lastFakeCardI--].SetActive(false);
        //cardCountText.text = cards.Count.ToString();
        //if (cards.Count == 0) cardCountText.gameObject.SetActive(false);

        return topCard;
    }



    public DeckData GetDeckData()
    {
        return deckData;
    }

}
