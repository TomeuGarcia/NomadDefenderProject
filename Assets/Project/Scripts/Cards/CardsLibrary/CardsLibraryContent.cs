using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CATEGORY_CardsLibraryContent", menuName = "Cards/CardsLibraryContent")]
public class CardsLibraryContent : ScriptableObject
{
    [System.Serializable]
    public class CardsPerProgression
    {
        [SerializeField] public NodeEnums.ProgressionState progressionState;
        [SerializeField] public TurretCardParts[] turretCards;
        [SerializeField] public TurretCardParts[] perfectTurretCards;
        [SerializeField] public SupportCardParts[] supportCards;
    }

    [Header("CARDS PER PROGRESSION")]
    [SerializeField] public CardsPerProgression earlyCards;
    [SerializeField] public CardsPerProgression midCards;
    [SerializeField] public CardsPerProgression lateCards;

    private Dictionary<NodeEnums.ProgressionState, CardsPerProgression> cardsByProgression;

    private void OnValidate()
    {
        ValidateCards();
    }

    private void OnEnable()
    {
        ValidateCards();
    }

    private void ValidateCards()
    {
        earlyCards.progressionState = NodeEnums.ProgressionState.EARLY;
        midCards.progressionState = NodeEnums.ProgressionState.MID;
        lateCards.progressionState = NodeEnums.ProgressionState.LATE;
    }

    public void Init()
    {
        ValidateCards();
        cardsByProgression = new Dictionary<NodeEnums.ProgressionState, CardsPerProgression> {
            { NodeEnums.ProgressionState.EARLY, earlyCards },
            { NodeEnums.ProgressionState.MID, midCards },
            { NodeEnums.ProgressionState.LATE, lateCards }
        };
    }


    public TurretCardParts[] GetRandomTurretCards(NodeEnums.ProgressionState progressionState, int totalAmount, int perfectAmount)
    {
        TurretCardParts[] tempCards = new TurretCardParts[totalAmount];

        List<TurretCardParts> availablePerfectCards = new List<TurretCardParts>(cardsByProgression[progressionState].perfectTurretCards);

        int i = 0;
        while (i < perfectAmount && availablePerfectCards.Count > 0)
        {
            int cardI = Random.Range(0, availablePerfectCards.Count);
            tempCards[i] = availablePerfectCards[cardI];

            availablePerfectCards.RemoveAt(cardI);

            ++i;
        }

        List<TurretCardParts> availableCards = new List<TurretCardParts>(cardsByProgression[progressionState].turretCards);
        while (i < totalAmount && availableCards.Count > 0)
        {
            int cardI = Random.Range(0, availableCards.Count);
            tempCards[i] = availableCards[cardI];

            availableCards.RemoveAt(cardI);

            ++i;
        }

        return tempCards;
    }
    
    public SupportCardParts[] GetRandomSupportCards(NodeEnums.ProgressionState progressionState, int totalAmount)
    {
        SupportCardParts[] tempCards = new SupportCardParts[totalAmount];

        int i = 0;

        List<SupportCardParts> availableCards = new List<SupportCardParts>(cardsByProgression[progressionState].supportCards);
        while (i < totalAmount && availableCards.Count > 0)
        {
            int cardI = Random.Range(0, availableCards.Count);
            tempCards[i] = availableCards[cardI];

            availableCards.RemoveAt(cardI);

            ++i;
        }

        return tempCards;
    }


}
