using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CATEGORY_CardsLibraryContent", menuName = 
    SOAssetPaths.CARDS_LIBRARIES + "CardsLibraryContent")]
public class CardsLibraryContent : ScriptableObject
{
    [System.Serializable]
    public class CardsPerProgression
    {
        [SerializeField] public NodeEnums.ProgressionState progressionState;
        [SerializeField] public TurretCardDataModel[] turretCards;
        [SerializeField] public TurretCardDataModel[] perfectTurretCards;
        [SerializeField] public SupportCardDataModel[] supportCards;
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


    public TurretCardDataModel[] GetRandomTurretCards(NodeEnums.ProgressionState progressionState, int totalAmount, int perfectAmount)
    {
        TurretCardDataModel[] tempCards = new TurretCardDataModel[totalAmount];

        List<TurretCardDataModel> availablePerfectCards = new List<TurretCardDataModel>(cardsByProgression[progressionState].perfectTurretCards);

        int i = 0;
        while (i < perfectAmount && availablePerfectCards.Count > 0)
        {
            int cardI = Random.Range(0, availablePerfectCards.Count);
            tempCards[i] = availablePerfectCards[cardI];

            availablePerfectCards.RemoveAt(cardI);

            ++i;
        }

        List<TurretCardDataModel> availableCards = new List<TurretCardDataModel>(cardsByProgression[progressionState].turretCards);
        while (i < totalAmount && availableCards.Count > 0)
        {
            int cardI = Random.Range(0, availableCards.Count);
            tempCards[i] = availableCards[cardI];

            availableCards.RemoveAt(cardI);

            ++i;
        }

        return tempCards;
    }
    
    public SupportCardDataModel[] GetRandomSupportCards(NodeEnums.ProgressionState progressionState, int totalAmount)
    {
        SupportCardDataModel[] tempCards = new SupportCardDataModel[totalAmount];

        int i = 0;

        List<SupportCardDataModel> availableCards = new List<SupportCardDataModel>(cardsByProgression[progressionState].supportCards);
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
