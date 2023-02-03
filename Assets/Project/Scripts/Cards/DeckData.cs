using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "DeckData", menuName = "Cards/DeckData")]
public class DeckData : ScriptableObject
{
    private List<BuildingCard> cards;

    [SerializeField] public List<TurretCardParts> starterTurretCardsComponents;
    private List<TurretCardParts> savedTurretCardsComponents;
    
    [SerializeField] public List<SupportCardParts> starterSupportCardsComponents;
    private List<SupportCardParts> savedSupportCardsComponents;



    public void ReplaceFor(DeckData other)
    {
        starterTurretCardsComponents = new List<TurretCardParts>();
        for (int i = 0; i < other.starterTurretCardsComponents.Count; ++i)
        {
            TurretCardParts otherComponents = other.starterTurretCardsComponents[i];

            TurretPartAttack turretPartAttack = ScriptableObject.CreateInstance("TurretPartAttack") as TurretPartAttack;
            turretPartAttack.InitAsCopy(otherComponents.turretPartAttack);

            TurretPartBody turretPartBody = ScriptableObject.CreateInstance("TurretPartBody") as TurretPartBody;
            turretPartBody.InitAsCopy(otherComponents.turretPartBody);

            TurretPartBase turretPartBase = ScriptableObject.CreateInstance("TurretPartBase") as TurretPartBase;
            turretPartBase.InitAsCopy(otherComponents.turretPartBase);

            TurretPassiveBase turretPassiveBase = ScriptableObject.CreateInstance("TurretPassiveBase") as TurretPassiveBase;
            turretPassiveBase.InitAsCopy(otherComponents.turretPassiveBase);

            starterTurretCardsComponents.Add(new TurretCardParts(otherComponents.cardLevel, turretPartAttack, turretPartBody,
                                                                                    turretPartBase, turretPassiveBase, otherComponents.cardCost));
        }

        starterSupportCardsComponents = new List<SupportCardParts>();
        for(int i = 0; i< other.starterSupportCardsComponents.Count; ++i)
        {
            SupportCardParts otherComponents = other.starterSupportCardsComponents[i];

            TurretPartBase supportPartBase = ScriptableObject.CreateInstance("TurretPartBase") as TurretPartBase;
            supportPartBase.InitAsCopy(otherComponents.turretPartBase);

            starterSupportCardsComponents.Add(new SupportCardParts(supportPartBase, otherComponents.cardCost));
        }
    }

    public void Init(BuildingCard[] starterCards)
    {
        cards = new List<BuildingCard>(starterCards);

        for (int i = 0; i < cards.Count; ++i)
        {
            cards[i].OnGetSaved += SaveCardComponents;
        }

        savedTurretCardsComponents = new List<TurretCardParts>();
        savedSupportCardsComponents = new List<SupportCardParts>();
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
        starterTurretCardsComponents.Add(new TurretCardParts(turretCard.turretCardParts));
    }
    public void AddSupportCard(SupportBuildingCard supportCard)
    {
        cards.Add(supportCard);
        starterSupportCardsComponents.Add(new SupportCardParts(supportCard.supportCardParts));
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
        starterTurretCardsComponents = new List<TurretCardParts>(savedTurretCardsComponents);
        starterSupportCardsComponents = new List<SupportCardParts>(savedSupportCardsComponents);
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
        savedTurretCardsComponents.Add(new TurretCardParts(turretCard.turretCardParts));
    }

    private void AddToSavedSupportCardsComponents(SupportBuildingCard supportCard)
    {
        savedSupportCardsComponents.Add(new SupportCardParts(supportCard.supportCardParts));
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
