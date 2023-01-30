using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DeckData", menuName = "Cards/DeckData")]
public class DeckData : ScriptableObject
{
    private List<BuildingCard> cards;

    [SerializeField] public List<TurretBuildingCard.TurretCardParts> starterTurretCardsComponents;
    private List<TurretBuildingCard.TurretCardParts> savedTurretCardsComponents;
    
    [SerializeField] public List<SupportBuildingCard.SupportCardParts> starterSupportCardsComponents;
    private List<SupportBuildingCard.SupportCardParts> savedSupportCardsComponents;


    public void ReplaceFor(DeckData other)
    {
        starterTurretCardsComponents = new List<TurretBuildingCard.TurretCardParts>();
        for (int i = 0; i < other.starterTurretCardsComponents.Count; ++i)
        {
            TurretBuildingCard.TurretCardParts otherComponents = other.starterTurretCardsComponents[i];

            TurretPartAttack turretPartAttack = ScriptableObject.CreateInstance("TurretPartAttack") as TurretPartAttack;
            turretPartAttack.InitAsCopy(otherComponents.turretPartAttack);

            TurretPartBody turretPartBody = ScriptableObject.CreateInstance("TurretPartBody") as TurretPartBody;
            turretPartBody.InitAsCopy(otherComponents.turretPartBody);

            TurretPartBase turretPartBase = ScriptableObject.CreateInstance("TurretPartBase") as TurretPartBase;
            turretPartBase.InitAsCopy(otherComponents.turretPartBase);

            TurretPassiveBase turretPassiveBase = ScriptableObject.CreateInstance("TurretPassiveBase") as TurretPassiveBase;
            turretPassiveBase.InitAsCopy(otherComponents.turretPassiveBase);

            starterTurretCardsComponents.Add(new TurretBuildingCard.TurretCardParts(turretPartAttack, turretPartBody,
                                                                                    turretPartBase, turretPassiveBase, otherComponents.cardCost));
        }

        starterSupportCardsComponents = new List<SupportBuildingCard.SupportCardParts>();
        for(int i = 0; i< other.starterSupportCardsComponents.Count; ++i)
        {
            SupportBuildingCard.SupportCardParts otherComponents = other.starterSupportCardsComponents[i];

            TurretPartBase supportPartBase = ScriptableObject.CreateInstance("TurretPartBase") as TurretPartBase;
            supportPartBase.InitAsCopy(otherComponents.turretPartBase);

            starterSupportCardsComponents.Add(new SupportBuildingCard.SupportCardParts(supportPartBase, otherComponents.cardCost));
        }
    }

    public void Init(BuildingCard[] starterCards)
    {
        cards = new List<BuildingCard>(starterCards);

        for (int i = 0; i < cards.Count; ++i)
        {
            cards[i].OnGetSaved += SaveCardComponents;
        }

        savedTurretCardsComponents = new List<TurretBuildingCard.TurretCardParts>();
        savedSupportCardsComponents = new List<SupportBuildingCard.SupportCardParts>();
    }

    public BuildingCard[] GetCards()
    {
        return cards.ToArray();
    }

    public List<BuildingCard> GetCardsReference()
    {
        return cards;
    }


    public void AddTurretCard(TurretBuildingCard turretCard)
    {
        cards.Add(turretCard);
        starterTurretCardsComponents.Add(new TurretBuildingCard.TurretCardParts(turretCard.turretCardParts));
    }
    public void AddSupportCard(SupportBuildingCard supportCard)
    {
        cards.Add(supportCard);
        starterSupportCardsComponents.Add(new SupportBuildingCard.SupportCardParts(supportCard.supportCardParts));
    }

    public void RemoveCard(int index)
    {
        cards.RemoveAt(index);
        starterTurretCardsComponents.RemoveAt(index);
    }


    public void SetStarterCardComponentsAsSaved()
    {
        savedTurretCardsComponents = starterTurretCardsComponents;
        savedSupportCardsComponents = starterSupportCardsComponents;
    }

    public void Save()
    {
        starterTurretCardsComponents = new List<TurretBuildingCard.TurretCardParts>(savedTurretCardsComponents);
        starterSupportCardsComponents = new List<SupportBuildingCard.SupportCardParts>(savedSupportCardsComponents);
    }



    private void SaveCardComponents(BuildingCard card)
    {
        if (card.cardBuildingType == BuildingCard.CardBuildingType.TURRET)
        {
            TurretBuildingCard turretCard = card as TurretBuildingCard;
            AddToSavedTurretCardsComponents(turretCard);
        }
        else if (card.cardBuildingType == BuildingCard.CardBuildingType.SUPPORT)
        {
            Debug.Log(card.name);
            SupportBuildingCard supportCard = card as SupportBuildingCard;
            AddToSavedSupportCardsComponents(supportCard);
        }


        else
        {
            Debug.Log("!!!  CAN'T SAVE CARD OF THIS TYPE !!!");
        }

    }
    private void AddToSavedTurretCardsComponents(TurretBuildingCard turretCard)
    {
        savedTurretCardsComponents.Add(new TurretBuildingCard.TurretCardParts(turretCard.turretCardParts));
    }

    private void AddToSavedSupportCardsComponents(SupportBuildingCard supportCard)
    {
        savedSupportCardsComponents.Add(new SupportBuildingCard.SupportCardParts(supportCard.supportCardParts));
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
