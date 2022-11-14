using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DeckData", menuName = "Cards/DeckData")]
public class DeckData : ScriptableObject
{
    private List<BuildingCard> cards;

    [SerializeField] public List<BuildingCard.CardComponents> starterCardsComponents;
    private List<BuildingCard.CardComponents> savedCardsComponents;



    public void Init(BuildingCard[] starterCards)
    {
        cards = new List<BuildingCard>(starterCards);

        for (int i = 0; i < cards.Count; ++i)
        {
            cards[i].OnGetSaved += AddToSavedCardsComponents;
        }

        savedCardsComponents = new List<BuildingCard.CardComponents>();
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


    public void Save()
    {
        starterCardsComponents = new List<BuildingCard.CardComponents>(savedCardsComponents);
    }


    private void AddToSavedCardsComponents(BuildingCard card)
    {
        savedCardsComponents.Add(new BuildingCard.CardComponents(card.GetTurretPartAttack(), card.GetTurretPartBody(), card.GetTurretPartBase()));
    }

}
