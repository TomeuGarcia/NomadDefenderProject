using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DeckData", menuName = "Cards/DeckData")]
public class DeckData : ScriptableObject
{
    private List<BuildingCard> cards;

    [SerializeField] public List<BuildingCard.CardComponents> starterCardsComponents;
    private List<BuildingCard.CardComponents> savedCardsComponents;


    public void ReplaceFor(DeckData other)
    {
        starterCardsComponents = new List<BuildingCard.CardComponents>();
        for (int i = 0; i < other.starterCardsComponents.Count; ++i)
        {
            BuildingCard.CardComponents otherComponents = other.starterCardsComponents[i];
            TurretPartAttack turretPartAttack = ScriptableObject.CreateInstance("TurretPartAttack") as TurretPartAttack;
            turretPartAttack.InitAsCopy(otherComponents.turretPartAttack);

            TurretPartBody turretPartBody = ScriptableObject.CreateInstance("TurretPartBody") as TurretPartBody;
            turretPartBody.InitAsCopy(otherComponents.turretPartBody);

            TurretPartBase turretPartBase = ScriptableObject.CreateInstance("TurretPartBase") as TurretPartBase;
            turretPartBase.InitAsCopy(otherComponents.turretPartBase);

            starterCardsComponents.Add(new BuildingCard.CardComponents(turretPartAttack, turretPartBody, turretPartBase));
        }
    }

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
        starterCardsComponents.Add(new BuildingCard.CardComponents(card.GetTurretPartAttack(), card.GetTurretPartBody(), card.GetTurretPartBase()));
    }

    public void RemoveCard(int index)
    {
        cards.RemoveAt(index);
        starterCardsComponents.RemoveAt(index);
    }


    public void SetStarterCardComponentsAsSaved()
    {
        savedCardsComponents = starterCardsComponents;
    }

    public void Save()
    {
        starterCardsComponents = new List<BuildingCard.CardComponents>(savedCardsComponents);
    }


    private void AddToSavedCardsComponents(BuildingCard card)
    {
        savedCardsComponents.Add(new BuildingCard.CardComponents(card.GetTurretPartAttack(), card.GetTurretPartBody(), card.GetTurretPartBase()));
    }


    public void ShuffleCards()
    {
        List<BuildingCard> cardsCopy = new List<BuildingCard>();

        while (cards.Count > 0)
        {
            int randomI = Random.Range(0, cards.Count);
            cardsCopy.Add(cards[randomI]);

            cards.RemoveAt(randomI);
            Debug.Log(randomI);
        }

        cards = cardsCopy;
    }

}
