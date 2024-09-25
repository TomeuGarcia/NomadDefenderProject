using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class DeckSelector : MonoBehaviour
{
    [Header("DECK LIBRARY")]
    [SerializeField] private DecksLibrary deckLibrary;

    [Header("CONFIGURATION")]
    [SerializeField] private DeckSelectorVisuals deckSelectorVisuals;
    [SerializeField] private SelectableDeck.ArrangeCardsData pileUpArrangeCardsData;
    [SerializeField] private SelectableDeck.ArrangeCardsData selectedArrangeCardsData;
    [SerializeField] private Transform selectedDeckHolder;
    [SerializeField] private CardMotionConfig _cardsMotionConfig;

    [Header("SELECTABLE DECKS")]
    [SerializeField] private SelectableDeck[] selectableDecks;
    private SelectableDeck currentlySelectedDeck;

    [Header("UI")]
    [SerializeField] private Button startSimulationButton;
    [SerializeField] private Light _startButtonLight;
    private float _startButtonLightIntensity;
    [SerializeField] private MeshRenderer runButtonMesh;
    [SerializeField] private MeshRenderer runInnerButtonMesh;
    [SerializeField] private MeshRenderer startSimulationFlashMesh;
    [SerializeField] private MeshRenderer startSimulationFlashMesh2;
    private Material startSimulationFlashMaterial;

    private float currentFill = 0.0f;

    private void Awake()
    {
        ServiceLocator.GetInstance().CameraHelp.SetCardsCamera(Camera.main);
        _cardsMotionConfig.SetUpgradeSceneMode();

        _startButtonLightIntensity = _startButtonLight.intensity;
        _startButtonLight.intensity = 0;
    }

    private void Start()
    {
        Init();

        startSimulationButton.interactable = false;
        startSimulationButton.onClick.AddListener(OnStartSimulationButtonPressed);

        CardTooltipDisplayManager.GetInstance()?.SetDisplayCamera(Camera.main);
        

        startSimulationFlashMaterial = startSimulationFlashMesh.material;
        startSimulationFlashMesh.material = startSimulationFlashMaterial;
        startSimulationFlashMesh2.material = startSimulationFlashMaterial;

        PauseMenu.GetInstance().GameCanBePaused = true;
    }

    private void Update()
    {
        UpdateCheatInputs();
    }


    private void Init()
    {
        int numberOfUnlockedDecks = StarterDecksUnlocker.GetInstance().GetNumberofUnlockedDecks();
        ICardSpawnService cardSpawnService = ServiceLocator.GetInstance().CardSpawnService;

        for (int i = 0; i < selectableDecks.Length; ++i)
        {
            SelectableDeck selectableDeck = selectableDecks[i];
            selectableDeck.InitReferences(this);
            selectableDeck.InitSpawnCards(cardSpawnService);
            selectableDeck.InitArrangeCards(pileUpArrangeCardsData);
            selectableDeck.SetNotSelected();
            selectableDeck.SetNotSelectedFinished(0);

            bool isUnlocked = i < numberOfUnlockedDecks;
            selectableDeck.InitState(isUnlocked);
        }

        deckSelectorVisuals.Init();
    }


    public void OnDeckSelected(SelectableDeck selectableDeck)
    {
        deckLibrary.SetStarterDeck(selectableDeck.Deck);

        SelectableDeck.RunUpgradesContent runContent = selectableDeck.RunContent;

        LibrariesManager.GetInstance().CardsLibrary.SetContent(runContent.cardsContent);
        LibrariesManager.GetInstance().PartsLibrary.SetContent(runContent.attacksContent, runContent.bodiesContent, 
            runContent.basesContent, runContent.bonusStatsContent);

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


            yield return StartCoroutine(previouslySelectedDeck.ArrangeCardsFromFirst(0.2f, 0.0f, pileUpArrangeCardsData, previouslySelectedDeck.CardsHolder, false));
            yield return new WaitUntil(() => previouslySelectedDeck.FinishedArranging);
            GameAudioManager.GetInstance().PlayCardInfoMoveHidden();
            previouslySelectedDeck.SetNotSelectedFinished(0.1f);
        }

        deckSelectorVisuals.ShowWires(selectableDeck.Position);

        currentlySelectedDeck = selectableDeck;

        currentlySelectedDeck.SetSelected();

        foreach (var selectableDeckIt in selectableDecks)
        {
            selectableDeckIt.SetEnabledInteraction(false);
        }


        yield return StartCoroutine(currentlySelectedDeck.ArrangeCardsFromLast(0.25f, 0.1f, selectedArrangeCardsData, selectedDeckHolder, true));

        currentlySelectedDeck.EnableCardsMouseInteraction();


        foreach (var selectableDeckIt in selectableDecks)
        {
            if (selectableDeckIt != currentlySelectedDeck)
            {
                selectableDeckIt.SetEnabledInteraction(true);
            }
        }

        startSimulationButton.interactable = true;
        if (currentFill == 0)
        {
            ChangeBorderLight(runButtonMesh, "_FillCoef", 0.0f, 1.0f);
            currentFill = 1.0f;
            _startButtonLight.DOIntensity(_startButtonLightIntensity, 0.5f).SetEase(Ease.InOutSine);
        }
    }

    private void ChangeBorderLight(MeshRenderer mr, string reference, float init, float goal)
    {
        Material material = mr.materials[1];
        material.SetFloat(reference, init);
        material.DOFloat(goal, reference, 0.25f);
    }

    private async void OnStartSimulationButtonPressed()
    {
        startSimulationButton.enabled = false;

        runInnerButtonMesh.transform.DOBlendableLocalMoveBy(Vector3.down * 0.3f, 0.25f);
        startSimulationButton.transform.DOBlendableLocalMoveBy(Vector3.forward * 6.0f, 0.25f);


        foreach (SelectableDeck selectableDeck in selectableDecks)
        {
            selectableDeck.SetEnabledInteraction(false);
        }
        currentlySelectedDeck.DisableShowInfo();

        if (false)
        {
            Color flashColor = currentlySelectedDeck.DeckColor;
            flashColor *= 5.0f;
            startSimulationFlashMaterial.SetColor("_FlashColor", flashColor);
        }
        
        startSimulationFlashMaterial.SetFloat("_StartTimeFlashAnimation", Time.time);

        float duration = 0.5f;
        //startSimulationButton.transform.DOPunchScale(Vector3.one * 0.4f, duration, 6);
        //GameAudioManager.GetInstance().PlayCardSelected();
        GameAudioManager.GetInstance().PlayNodeSelectedSound();

        startSimulationButton.interactable = false;

        await Task.Delay((int)(duration * 1000));


        SceneLoader.GetInstance().StartLoadNormalGame(true);
    }


    private void UpdateCheatInputs()
    {
        if (Input.GetKeyDown(KeyCode.L) && Input.GetKey(KeyCode.LeftShift))
        {
            StarterDecksUnlocker.GetInstance().ResetUnlockedCount();
            int numberOfUnlockedDecks = StarterDecksUnlocker.GetInstance().GetNumberofUnlockedDecks();
            for (int i = 0; i < selectableDecks.Length; ++i)
            {
                selectableDecks[i].InitState(i < numberOfUnlockedDecks);
            }
        }
        if (Input.GetKeyDown(KeyCode.U) && Input.GetKey(KeyCode.LeftShift))
        {
            int numberOfUnlockedDecks = StarterDecksUnlocker.GetInstance().GetNumberofUnlockedDecks();
            Debug.Log(numberOfUnlockedDecks);

            for (int i = numberOfUnlockedDecks-1; i < selectableDecks.Length; ++i)
            {
                selectableDecks[i].InitState(true);
                StarterDecksUnlocker.GetInstance().UnlockNextDeck();
            }
            StarterDecksUnlocker.GetInstance().UnlockRemainengDecks();
        }
    }

}
