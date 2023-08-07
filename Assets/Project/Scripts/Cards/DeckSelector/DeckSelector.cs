using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class DeckSelector : MonoBehaviour
{
    [Header("DECK CREATOR")]
    [SerializeField] private DeckCreator deckCreator;

    [Header("DECK LIBRARY")]
    [SerializeField] private DecksLibrary deckLibrary;

    [Header("CONFIGURATION")]
    [SerializeField] private DeckSelectorVisuals deckSelectorVisuals;
    [SerializeField] private SelectableDeck.ArrangeCardsData pileUpArrangeCardsData;
    [SerializeField] private SelectableDeck.ArrangeCardsData selectedArrangeCardsData;
    [SerializeField] private Transform selectedDeckHolder;    

    [Header("SELECTABLE DECKS")]
    [SerializeField] private SelectableDeck[] selectableDecks;
    private SelectableDeck currentlySelectedDeck;

    [Header("UI")]
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button startSimulationButton;


    private void Start()
    {
        Init();

        startSimulationButton.interactable = false;
        startSimulationButton.onClick.AddListener(OnStartSimulationButtonPressed);
        mainMenuButton.onClick.AddListener(OnMainMenuButtonPressed);

        CardDescriptionDisplayer.GetInstance()?.SetCamera(Camera.main);

        // Avoid null references
        deckCreator.DisableSaveOnDisable();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneLoader.GetInstance().StartLoadMainMenu();
        }
    }


    private void Init()
    {
        for (int i = 0; i < selectableDecks.Length; ++i)
        {
            selectableDecks[i].InitReferences(this);
            selectableDecks[i].InitSpawnCards(deckCreator);
            selectableDecks[i].InitArrangeCards(pileUpArrangeCardsData);
            selectableDecks[i].SetNotSelected();
        }

        deckSelectorVisuals.Init();
    }


    public void OnDeckSelected(SelectableDeck selectableDeck)
    {
        deckLibrary.SetStarterDeck(selectableDeck.DeckData);

        SelectableDeck.RunUpgradesContent runContent = selectableDeck.RunContent;

        LibrariesManager.GetInstance().CardsLibrary.SetContent(runContent.cardsContent);
        LibrariesManager.GetInstance().PartsLibrary.SetContent(runContent.attacksContent, runContent.bodiesContent, runContent.basesContent);

        StartCoroutine(DoOnDeckSelected(selectableDeck));
    }

    private IEnumerator DoOnDeckSelected(SelectableDeck selectableDeck)
    {
        SelectableDeck previouslySelectedDeck = null;
        if (currentlySelectedDeck != null && currentlySelectedDeck != selectableDeck)
        {
            previouslySelectedDeck = currentlySelectedDeck;

            previouslySelectedDeck.SetNotSelected();
            previouslySelectedDeck.DisableCardsMouseInteraction();


            yield return StartCoroutine(previouslySelectedDeck.ArrangeCardsFromFirst(0.2f, 0.0f, pileUpArrangeCardsData, previouslySelectedDeck.CardsHolder));
            yield return new WaitUntil(() => previouslySelectedDeck.FinishedArranging);
            GameAudioManager.GetInstance().PlayCardInfoMoveHidden();
        }

        deckSelectorVisuals.ShowWires(selectableDeck.Position);

        currentlySelectedDeck = selectableDeck;

        currentlySelectedDeck.SetSelected();
        currentlySelectedDeck.SetEnabledInteraction(false);


        yield return StartCoroutine(currentlySelectedDeck.ArrangeCardsFromLast(0.25f, 0.1f, selectedArrangeCardsData, selectedDeckHolder));

        currentlySelectedDeck.EnableCardsMouseInteraction();
        

        previouslySelectedDeck?.SetEnabledInteraction(true);

        startSimulationButton.interactable = true;
    }



    private async void OnStartSimulationButtonPressed()
    {
        startSimulationButton.enabled = false;

        float duration = 0.5f;
        startSimulationButton.transform.DOPunchScale(Vector3.one * 0.4f, duration, 6);
        GameAudioManager.GetInstance().PlayCardSelected();

        startSimulationButton.interactable = false;

        await Task.Delay((int)(duration * 1000));


        SceneLoader.GetInstance().StartLoadNormalGame(true);
    }

    private async void OnMainMenuButtonPressed()
    {
        mainMenuButton.enabled = false;

        float duration = 0.5f;
        mainMenuButton.transform.DOPunchScale(Vector3.one * 0.4f, duration, 6);
        GameAudioManager.GetInstance().PlayCardSelected();

        startSimulationButton.interactable = false;

        await Task.Delay((int)(duration * 1000));

        SceneLoader.GetInstance().StartLoadMainMenu();
    }


}
