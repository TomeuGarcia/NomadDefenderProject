using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckSelector : MonoBehaviour
{
    [Header("DECK CREATOR")]
    [SerializeField] private DeckCreator deckCreator;

    [Header("CONFIGURATION")]
    [SerializeField] private SelectableDeck.ArrangeCardsData pileUpArrangeCardsData;
    [SerializeField] private SelectableDeck.ArrangeCardsData selectedArrangeCardsData;
    [SerializeField] private Transform selectedDeckHolder;    

    [Header("SELECTABLE DECKS")]
    [SerializeField] private SelectableDeck[] selectableDecks;
    private SelectableDeck currentlySelectedDeck;

    [Header("UI")]
    [SerializeField] private Button startSimulationButton;


    private void Awake()
    {
        Init();

        startSimulationButton.interactable = false;
        startSimulationButton.onClick.AddListener(OnStartSimulationButtonPressed);
    }


    private void Init()
    {
        for (int i = 0; i < selectableDecks.Length; ++i)
        {
            selectableDecks[i].InitReferences(this);
            selectableDecks[i].InitSpawnCards(deckCreator);
            selectableDecks[i].InitArrangeCards(pileUpArrangeCardsData);
        }
    }


    public void OnDeckSelected(SelectableDeck selectableDeck)
    {
        StartCoroutine(DoOnDeckSelected(selectableDeck));
    }

    private IEnumerator DoOnDeckSelected(SelectableDeck selectableDeck)
    {
        SelectableDeck previouslySelectedDeck = null;
        if (currentlySelectedDeck != null && currentlySelectedDeck != selectableDeck)
        {
            previouslySelectedDeck = currentlySelectedDeck;

            previouslySelectedDeck.DisableCardsMouseInteraction();
            yield return StartCoroutine(previouslySelectedDeck.ArrangeCardsFromFirst(0.2f, 0.0f, pileUpArrangeCardsData, previouslySelectedDeck.CardsHolder));            
        }

        currentlySelectedDeck = selectableDeck;
        currentlySelectedDeck.SetEnabledInteraction(false);

        yield return StartCoroutine(currentlySelectedDeck.ArrangeCardsFromLast(0.2f, 0.1f, selectedArrangeCardsData, selectedDeckHolder));
        currentlySelectedDeck.EnableCardsMouseInteraction();

        previouslySelectedDeck?.SetEnabledInteraction(true);

        startSimulationButton.interactable = true;
    }



    private void OnStartSimulationButtonPressed()
    {
        startSimulationButton.interactable = false;
        SceneLoader.GetInstance().StartLoadNormalGame(true);
    }

}
