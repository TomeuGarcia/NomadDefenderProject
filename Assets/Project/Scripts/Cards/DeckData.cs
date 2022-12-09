using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DeckData", menuName = "Cards/DeckData")]
public class DeckData : ScriptableObject
{
    private List<BuildingCard> cards;

    [SerializeField] public List<TurretCard.TurretCardParts> starterTurretCardsComponents;
    private List<TurretCard.TurretCardParts> savedTurretCardsComponents;


    public void ReplaceFor(DeckData other)
    {
        starterTurretCardsComponents = new List<TurretCard.TurretCardParts>();
        for (int i = 0; i < other.starterTurretCardsComponents.Count; ++i)
        {
            TurretCard.TurretCardParts otherComponents = other.starterTurretCardsComponents[i];

            TurretPartAttack turretPartAttack = ScriptableObject.CreateInstance("TurretPartAttack") as TurretPartAttack;
            turretPartAttack.InitAsCopy(otherComponents.turretPartAttack);

            TurretPartBody turretPartBody = ScriptableObject.CreateInstance("TurretPartBody") as TurretPartBody;
            turretPartBody.InitAsCopy(otherComponents.turretPartBody);

            TurretPartBase turretPartBase = ScriptableObject.CreateInstance("TurretPartBase") as TurretPartBase;
            turretPartBase.InitAsCopy(otherComponents.turretPartBase);

            starterTurretCardsComponents.Add(new TurretCard.TurretCardParts(turretPartAttack, turretPartBody, turretPartBase));
        }
    }

    public void Init(BuildingCard[] starterCards)
    {
        cards = new List<BuildingCard>(starterCards);

        for (int i = 0; i < cards.Count; ++i)
        {
            cards[i].OnGetSaved += SaveCardComponents;
        }

        savedTurretCardsComponents = new List<TurretCard.TurretCardParts>();
    }

    public BuildingCard[] GetCards()
    {
        return cards.ToArray();
    }

    public List<BuildingCard> GetCardsReference()
    {
        return cards;
    }


    public void AddTurretCard(TurretCard turretCard)
    {
        cards.Add(turretCard);
        starterTurretCardsComponents.Add(new TurretCard.TurretCardParts(turretCard.turretCardParts));
    }

    public void RemoveCard(int index)
    {
        cards.RemoveAt(index);
        starterTurretCardsComponents.RemoveAt(index);
    }


    public void SetStarterCardComponentsAsSaved()
    {
        savedTurretCardsComponents = starterTurretCardsComponents;
    }

    public void Save()
    {
        starterTurretCardsComponents = new List<TurretCard.TurretCardParts>(savedTurretCardsComponents);
    }


    private void SaveCardComponents(BuildingCard card)
    {
        if (card.cardBuildingType == BuildingCard.CardBuildingType.TURRET)
        {
            TurretCard turretCard = card as TurretCard;
            AddToSavedTurretCardsComponents(turretCard);
        }
        else
        {
            Debug.Log("!!!  CAN'T SAVE CARD OF THIS TYPE !!!");
        }

    }
    private void AddToSavedTurretCardsComponents(TurretCard turretCard)
    {
        savedTurretCardsComponents.Add(new TurretCard.TurretCardParts(turretCard.turretCardParts));
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
