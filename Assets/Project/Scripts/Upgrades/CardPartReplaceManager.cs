using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class CardPartReplaceManager : MonoBehaviour
{
    [Header("CAMERA")]
    [SerializeField] private Camera mouseDragCamera;

    [Header("UPGRADE SETUP")]
    [SerializeField] private UpgradeSceneSetupInfo upgradeSceneSetupInfo;

    [Header("SCENE MANAGEMENT")]
    [SerializeField] private MapSceneNotifier mapSceneNotifier;

    [Header("DECK DATA")]
    [SerializeField] private DeckData deckData;
    private List<BuildingCard> deckCards;

    [Header("MACHINE")]
    [SerializeField] private UpgradeMachineControl upgradeMachineControl;

    [Header("HOLDERS")]
    [SerializeField] private UpgradeCardHolder upgradeCardHolder;
    [SerializeField] private CardPartHolder cardPartHolder;
    public UpgradeCardHolder UpgradeCardHolder => upgradeCardHolder;
    public CardPartHolder CardPartHolder => cardPartHolder;

    public enum PartType { ATTACK, BODY, BASE }
    [Header("TYPE")]
    [SerializeField] private int numCards = 3;
    [SerializeField] private int numParts = 3;
    [SerializeField] private PartType partType;

    [Header("PARTS")]
    [SerializeField] private PartsLibrary partsLibrary;
    [Header("ATTACK")]
    [SerializeField] private GameObject cardPartAttackPrefab;
    [Header("BODY")]
    [SerializeField] private GameObject cardPartBodyPrefab;
    [Header("BASE")]
    [SerializeField] private GameObject cardPartBasePrefab;

    private TurretPartAttack[] tutorialTurretPartAttacks;
    private TurretPartBody[] tutorialTurretPartBodies;
    private PartsLibrary.BaseAndPassive[] tutorialTurretPartBases;


    [Header("SUBTRACT CARD PLAY COST")]
    [SerializeField, Range(5, 50)] private readonly int playCostSubtractAmountSamePart = 20;


    [Header("COMPONENTS")]
    [SerializeField] private MouseOverNotifier buttonMouseOverNotifier;
    [SerializeField] private Transform cardPlacerToParent;
    [SerializeField] private Transform cardPartPlacerToParent;
    [SerializeField] private Transform resultCardPlacerToParent;
    [SerializeField] private TurretBuildingCard previewTurretCard;

    [Header("TEXT")]
    [SerializeField] ConsoleDialogSystem consoleDialog;
    [SerializeField] TextLine textLine;
    [SerializeField] bool isTutorial;

    private bool replacementDone = false;
    public bool ReplacementDone => replacementDone;

    private bool cardIsReady = false;
    private bool partIsReady = false;
    [HideInInspector] public bool canFinalRetrieveCard = true;


    private int numPartsIfPerfect = 1;
    private bool lastBattleWasDefendedPerfectly = false;
    NodeEnums.ProgressionState progressionState = NodeEnums.ProgressionState.EARLY;


    public delegate void CarPartReplaceManagerAction();
    public static event CarPartReplaceManagerAction OnReplacementStart;
    public static event CarPartReplaceManagerAction OnReplacementDone;


    // TUTORIAL
    private bool partsCreatedByTutorial = false;

    private void Awake()
    {
        BuildingCard.MouseDragCamera = mouseDragCamera;
        CardPart.MouseDragCamera = mouseDragCamera;

        CardDescriptionDisplayer.GetInstance().SetCamera(Camera.main);
    }
    

    private void Start()
    {
        deckCards = deckData.GetCardsReference();

        SetButtonNotReady();
        Init();

        PlayCardAndCardPartsAppearAnimation();

        UpdatePreviewCard_MissingParts(previewTurretCard, true, true);
    }

    private void OnEnable()
    {
        buttonMouseOverNotifier.OnMousePressed += ProceedReplace;

        upgradeCardHolder.OnCardSelected += CardWasSelected;
        upgradeCardHolder.OnCardUnselected += CardWasUnselected;
        cardPartHolder.OnPartSelected += PartWasSelected;
        cardPartHolder.OnPartUnselected += PartWasUnselected;

        upgradeMachineControl.OnReplaceStart += AttachSelectedCardsToMachine;
        upgradeMachineControl.OnReplaceCardPrinted += AttachResultCardToMachine;
    }

    private void OnDisable()
    {
        buttonMouseOverNotifier.OnMousePressed -= ProceedReplace;

        upgradeCardHolder.OnCardSelected -= CardWasSelected;
        upgradeCardHolder.OnCardUnselected -= CardWasUnselected;
        cardPartHolder.OnPartSelected -= PartWasSelected;
        cardPartHolder.OnPartUnselected -= PartWasUnselected;

        upgradeMachineControl.OnReplaceStart -= AttachSelectedCardsToMachine;
        upgradeMachineControl.OnReplaceCardPrinted -= AttachResultCardToMachine;
    }



    private void Init()
    {
        // TODO
        numPartsIfPerfect = 1;
        progressionState = upgradeSceneSetupInfo.CurrentNodeProgressionState;
        lastBattleWasDefendedPerfectly = upgradeSceneSetupInfo.LastBattleWasDefendedPerfectly;
        NodeEnums.HealthState currentNodeHealthState = upgradeSceneSetupInfo.CurrentNodeHealthState;

        if (currentNodeHealthState == NodeEnums.HealthState.GREATLY_DAMAGED)
        {
            numCards = 2;
            numParts = 2;
        }
        else if (currentNodeHealthState == NodeEnums.HealthState.SLIGHTLY_DAMAGED)
        {
            numCards = 3;
            numParts = 2;
        }
        else if (currentNodeHealthState == NodeEnums.HealthState.UNDAMAGED)
        {
            numCards = 3;
            numParts = 3;

            if (lastBattleWasDefendedPerfectly)
            {
                // level up card to max
            }
        }

        if (!partsCreatedByTutorial)
        {
            if (partType == PartType.ATTACK) InitAttacksRandom();
            else if (partType == PartType.BODY) InitBodiesRandom();
            else if (partType == PartType.BASE) InitBasesRandom();
        }
        else
        {
            if (partType == PartType.ATTACK) InitAttacksTutorial();
            else if (partType == PartType.BODY) InitBodiesTutorial();
            else if (partType == PartType.BASE) InitBasesTutorial();
        }


        List<BuildingCard> randomCards = new List<BuildingCard>(GetRandomDeckCards());
        for (int i = 0; i < deckCards.Count; ++i)
        {
            if (!randomCards.Contains(deckCards[i]))
            {
                deckCards[i].DisableMouseInteraction();
            }            
        }

        upgradeCardHolder.Init(randomCards.ToArray());
    }


    public void AwakeSetupTutorialAttacks(TurretPartAttack[] turretPartAttacks)
    {
        partsCreatedByTutorial = true;
        tutorialTurretPartAttacks = turretPartAttacks;
    }
    private void InitAttacksTutorial()
    {
        numParts = tutorialTurretPartAttacks.Length;
        InitAttacks(tutorialTurretPartAttacks);
    }
    private void InitAttacksRandom()
    {
        InitAttacks(partsLibrary.GetRandomTurretPartAttacks(numParts, numPartsIfPerfect, lastBattleWasDefendedPerfectly, progressionState));
    }
    private void InitAttacks(TurretPartAttack[] attacks)
    {
        CardPartAttack[] parts = new CardPartAttack[numParts];
        for (int i = 0; i < numParts; ++i)
        {
            parts[i] = Instantiate(cardPartAttackPrefab, cardPartHolder.cardsHolderTransform).GetComponent<CardPartAttack>();
            parts[i].turretPartAttack = attacks[i];
        }
        cardPartHolder.Init(parts);

        PrintConsoleLine(TextTypes.INSTRUCTION, "Replace a turret's PROJECTILE with a new one", true, 2f);
    }


    public void AwakeSetupTutorialBodies(TurretPartBody[] turretPartBodies)
    {
        partsCreatedByTutorial = true;
        tutorialTurretPartBodies = turretPartBodies;
    }
    private void InitBodiesTutorial()
    {
        numParts = tutorialTurretPartBodies.Length;
        InitBodies(tutorialTurretPartBodies);
    }
    private void InitBodiesRandom()
    {
        InitBodies(partsLibrary.GetRandomTurretPartBodies(numParts, numPartsIfPerfect, lastBattleWasDefendedPerfectly, progressionState));
    }
    private void InitBodies(TurretPartBody[] bodies)
    {
        CardPartBody[] parts = new CardPartBody[numParts];
        for (int i = 0; i < numParts; ++i)
        {
            parts[i] = Instantiate(cardPartBodyPrefab, cardPartHolder.cardsHolderTransform).GetComponent<CardPartBody>();
            parts[i].turretPartBody = bodies[i];
        }
        cardPartHolder.Init(parts);
        PrintConsoleLine(TextTypes.INSTRUCTION, "Replace a turret's BODY with a new one", true, 2f);
    }

    public void AwakeSetupTutorialBases(PartsLibrary.BaseAndPassive[] basesAndPassives)
    {
        partsCreatedByTutorial = true;
        tutorialTurretPartBases = basesAndPassives;
    }
    private void InitBasesTutorial()
    {
        numParts = tutorialTurretPartBases.Length;
        InitBases(tutorialTurretPartBases);
    }
    private void InitBasesRandom()
    {
        InitBases(partsLibrary.GetRandomTurretPartBaseAndPassive(numParts, numPartsIfPerfect, lastBattleWasDefendedPerfectly, progressionState));
    }
    private void InitBases(PartsLibrary.BaseAndPassive[] basesAndPassives)
    {
        CardPartBase[] parts = new CardPartBase[numParts];
        for (int i = 0; i < numParts; ++i)
        {
            parts[i] = Instantiate(cardPartBasePrefab, cardPartHolder.cardsHolderTransform).GetComponent<CardPartBase>();
            parts[i].turretPartBase = basesAndPassives[i].turretPartBase;
            parts[i].turretPassiveBase = basesAndPassives[i].turretPassiveBase;
        }
        cardPartHolder.Init(parts);

        PrintConsoleLine(TextTypes.INSTRUCTION, "Replace a turret's BASE with a new one", true, 2f);
    }



    private BuildingCard[] GetRandomDeckCards()
    {
        // Separate MAXed cards from NON-MAXed cards
        List<BuildingCard> maxLevelCards = new List<BuildingCard>();
        List<BuildingCard> notMaxLevelCards = new List<BuildingCard>();
        
        for (int cardI = 0; cardI < deckCards.Count; ++cardI)
        {
            if (deckCards[cardI].cardBuildingType == BuildingCard.CardBuildingType.TURRET)
            {
                if (deckCards[cardI].GetCardLevel() < 3)
                {
                    notMaxLevelCards.Add(deckCards[cardI]);
                }
                else
                {
                    maxLevelCards.Add(deckCards[cardI]);
                }
            }            
        }

        BuildingCard[] chosenCards = new BuildingCard[numCards];
        int chosenCardI = 0;

        int numMaxedCardsToAdd = numCards - notMaxLevelCards.Count;
        numMaxedCardsToAdd = numMaxedCardsToAdd < 0 ? 0 : numMaxedCardsToAdd;

        // If not enough NON-MAXed cards, add MAXed cards
        if (numMaxedCardsToAdd > 0)
        {            
            HashSet<int> randomMaxedCardsIndices = new HashSet<int>();

            while (randomMaxedCardsIndices.Count < numMaxedCardsToAdd)
            {
                int randomIndex = Random.Range(0, maxLevelCards.Count);
                randomMaxedCardsIndices.Add(randomIndex);
            }
            foreach (int index in randomMaxedCardsIndices)
            {
                chosenCards[chosenCardI] = maxLevelCards[index]; 
                ++chosenCardI;
            }
        }

        // Add NON-MAXed cards
        int numRemainingCards = numCards - numMaxedCardsToAdd;
        HashSet<int> randomNotMaxedCardsIndices = new HashSet<int>();
        while (randomNotMaxedCardsIndices.Count < numRemainingCards)
        {
            int randomIndex = Random.Range(0, notMaxLevelCards.Count);
            randomNotMaxedCardsIndices.Add(randomIndex);
        }
        foreach (int index in randomNotMaxedCardsIndices)
        {
            chosenCards[chosenCardI] = notMaxLevelCards[index];
            ++chosenCardI;
        }

        // Set parent
        for (int cardI = 0; cardI < numCards; ++cardI)
        {
            chosenCards[cardI].transform.SetParent(upgradeCardHolder.CardsHolder, false);
        }

        return chosenCards.ToArray();
    }


    public void ProceedReplace()
    {
        if (replacementDone) return;

        if (CanConfirm())
        {
            replacementDone = true;

            //buttonAnimator.SetTrigger("Pressed");
            //buttonMaterial.SetFloat("_IsAlwaysOn", 1f);
            //StartCoroutine(ReplecementAnimation());

            upgradeMachineControl.Replace();

            // Audio
            GameAudioManager.GetInstance().PlayUpgradeButtonPressed();
        }
        else
        {
            // Audio
            GameAudioManager.GetInstance().PlayUpgradeButtonCantBePressed();
        }
    }


    private bool CanConfirm()
    {
        return cardIsReady && partIsReady;
    }

    
    private void ReplacePartInCard(TurretBuildingCard selectedCard)
    {
        selectedCard.IncrementCardLevel(1);

        switch (partType)
        {
            case PartType.ATTACK:
                {
                    selectedCard.SetNewPartAttack(cardPartHolder.selectedCardPart.gameObject.GetComponent<CardPartAttack>().turretPartAttack);
                }
                break;
            case PartType.BODY:
                {
                    selectedCard.SetNewPartBody(cardPartHolder.selectedCardPart.gameObject.GetComponent<CardPartBody>().turretPartBody);
                }
                break;
            case PartType.BASE:
                {
                    selectedCard.SetNewPartBase(cardPartHolder.selectedCardPart.gameObject.GetComponent<CardPartBase>().turretPartBase,
                                                cardPartHolder.selectedCardPart.gameObject.GetComponent<CardPartBase>().turretPassiveBase);
                }
                break;
            default: 
                break;
        }
    }


    
    private IEnumerator ReplecementAnimation()
    {
        upgradeCardHolder.StopInteractions();
        cardPartHolder.StopInteractions();

        float partMoveDuration = 0.2f;
        float flipDuration = 0.4f;
        float moveDuration = 0.5f;

        Transform upgradedCardTransform = upgradeCardHolder.selectedCard.CardTransform;
        Vector3 upgradedCardStartPos = upgradedCardTransform.position;
        Transform partTransform = cardPartHolder.selectedCardPart.CardTransform;


        yield return new WaitForSeconds(0.1f);

        // Audio
        GameAudioManager.GetInstance().PlayCardPartSwap();
        yield return new WaitForSeconds(0.2f);

        // Move part towards card
        partTransform.DOMove(upgradedCardStartPos, partMoveDuration);
        yield return new WaitForSeconds(partMoveDuration);
        partTransform.DOMove(partTransform.position + Vector3.up * -0.2f, 0.2f);

        // Rise up card
        upgradedCardTransform.DOMove(upgradedCardStartPos + (upgradedCardTransform.forward * -0.5f), moveDuration);
        yield return new WaitForSeconds(moveDuration);

        // Flip down
        upgradedCardTransform.DOLocalRotate(Vector3.up * 180f, flipDuration);
        yield return new WaitForSeconds(flipDuration + 0.1f);


        // EXECUTE REPLACEMENT
        TurretBuildingCard selectedCard = upgradeCardHolder.selectedCard as TurretBuildingCard;
        ReplacePartInCard(selectedCard);
        InvokeReplacementStart();

        bool replacedWithSamePart = selectedCard.ReplacedWithSamePart;
        if (replacedWithSamePart) selectedCard.SubtractPlayCost(playCostSubtractAmountSamePart);
        selectedCard.PlayLevelUpAnimation();


        // Flip up
        upgradedCardTransform.DOLocalRotate(Vector3.up * 360f, flipDuration);
        yield return new WaitForSeconds(flipDuration);

        // Rise down card
        upgradedCardTransform.DOMove(upgradedCardStartPos, moveDuration);
        yield return new WaitForSeconds(moveDuration);

        // Hide parts
        cardPartHolder.Hide(0.5f, 0.2f);
        yield return new WaitForSeconds(1f);

        if (replacedWithSamePart) yield return new WaitForSeconds(1.5f);


        yield return new WaitUntil(() => canFinalRetrieveCard);

        // Enable FINAL retreieve card
        //upgradeCardHolder.EnableFinalRetrieve(0.2f, 0.5f, 0.2f);
        upgradeCardHolder.StartFinalRetrieve(0.2f, 0.5f, 0.2f);

        consoleDialog.Clear();
        PrintConsoleLine(TextTypes.SYSTEM, "Replacement done successfully", false, 0f);


        upgradeCardHolder.OnFinalRetrieve += InvokeReplacementDone;
    }
    


    private void CardWasSelected()
    {
        cardIsReady = true;
        upgradeMachineControl.SelectLeftCard();

        if (partIsReady)
        {
            SetButtonReady();            
            UpdatePreviewCard_CardAndCardPart(previewTurretCard, upgradeCardHolder.selectedCard as TurretBuildingCard, CardPartHolder.selectedCardPart);
        }
        else
        {
            UpdatePreviewCard_MissingParts(previewTurretCard, false, true);
        }
    }
    private void CardWasUnselected()
    {
        cardIsReady = false;
        upgradeMachineControl.RetrieveLeftCard();

        if (partIsReady)
        {
            SetButtonNotReady();
            UpdatePreviewCard_MissingParts(previewTurretCard, true, false);
        }
        else
        {
            UpdatePreviewCard_MissingParts(previewTurretCard, true, true);
        }
    }

    private void PartWasSelected()
    {
        partIsReady = true;
        upgradeMachineControl.SelectRightCard();

        if (cardIsReady)
        {
            SetButtonReady();
            UpdatePreviewCard_CardAndCardPart(previewTurretCard, upgradeCardHolder.selectedCard as TurretBuildingCard, CardPartHolder.selectedCardPart);
        }
        else
        {
            UpdatePreviewCard_MissingParts(previewTurretCard, true, false);
        }
    }
    private void PartWasUnselected()
    {
        partIsReady = false;
        upgradeMachineControl.RetrieveRightCard();

        if (cardIsReady)
        {
            SetButtonNotReady();
            UpdatePreviewCard_MissingParts(previewTurretCard, false, true);
        }
        else
        {
            UpdatePreviewCard_MissingParts(previewTurretCard, true, true);
        }
    }



    private void SetButtonReady()
    {
        upgradeMachineControl.ActivateButton();
    }

    private void SetButtonNotReady()
    {
        upgradeMachineControl.DeactivateButton();
    }

    private void InvokeReplacementDone()
    {
        upgradeCardHolder.OnFinalRetrieve -= InvokeReplacementDone;

        mapSceneNotifier.InvokeOnSceneFinished();
        consoleDialog.Clear();

        if (OnReplacementDone != null) OnReplacementDone(); // Subscribe to this event to load NEXT SCENE
    }

    private void InvokeReplacementStart()
    {
        if (OnReplacementStart != null) OnReplacementStart();

        SetButtonNotReady();
    }

    public bool GetPartIsReady()
    {
        return partIsReady;
    }

    public bool GetCardIsReady()
    {
        return cardIsReady;
    }

    public bool GetReplacementDone()
    {
        return replacementDone;
    }


    private void PlayCardAndCardPartsAppearAnimation()
    {
        StartCoroutine(upgradeCardHolder.AppearAnimation(1f));
        StartCoroutine(cardPartHolder.AppearAnimation(2.5f));
    }

    public void PauseCardsAppearAnimation()
    {
        upgradeCardHolder.appearAnimationCanStartMoving = false;
    }
    public void ResumeCardsAppearAnimation()
    {
        upgradeCardHolder.appearAnimationCanStartMoving = true;
    }

    public void PauseCardPartsAppearAnimation()
    {
        cardPartHolder.appearAnimationCanStartMoving = false;
    }
    public void ResumeCardPartsAppearAnimation()
    {
        cardPartHolder.appearAnimationCanStartMoving = true;
    }




    void PrintConsoleLine(TextTypes type, string text, bool clearBeforeWritting, float delay)
    {
        StartCoroutine(DoPrintConsoleLine(type, text, clearBeforeWritting, delay));
    }

    private IEnumerator DoPrintConsoleLine(TextTypes type, string text, bool clearBeforeWritting, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (!isTutorial)
        {

            if (clearBeforeWritting)
                consoleDialog.Clear();

            textLine.textType = type;
            textLine.text = text;
            consoleDialog.PrintLine(textLine);            
        }
    }


    void PrintConsoleLine(TextTypes type, string text)
    {
        textLine.textType = type;
        textLine.text = text;
        consoleDialog.PrintLine(textLine);
    }


    private void AttachSelectedCardsToMachine()
    {
        upgradeCardHolder.selectedCard.RootCardTransform.SetParent(cardPlacerToParent);
        cardPartHolder.selectedCardPart.RootCardTransform.SetParent(cardPartPlacerToParent);

        Sequence selectedCardSequence = DOTween.Sequence();
        selectedCardSequence.AppendInterval(0.3f);
        float endY = upgradeCardHolder.selectedCard.RootCardTransform.position.y - 2.0f;
        selectedCardSequence.Append(upgradeCardHolder.selectedCard.RootCardTransform.DOMoveY(endY, 0.3f));
    }

    private void AttachResultCardToMachine()
    {
        upgradeCardHolder.selectedCard.RootCardTransform.SetParent(resultCardPlacerToParent);
        upgradeCardHolder.selectedCard.RootCardTransform.localPosition = new Vector3(-7f, 1, 10.78f);
        //upgradeCardHolder.selectedCard.RootCardTransform.position = resultCardPlacerToParent.position;


        TurretBuildingCard selectedCard = upgradeCardHolder.selectedCard as TurretBuildingCard;
        ReplacePartInCard(selectedCard);

        bool replacedWithSamePart = selectedCard.ReplacedWithSamePart;
        if (replacedWithSamePart) selectedCard.SubtractPlayCost(playCostSubtractAmountSamePart);
        selectedCard.PlayLevelUpAnimation();



        Sequence selectedCardSequence = DOTween.Sequence();
        selectedCardSequence.AppendInterval(1.7f);

        selectedCardSequence.AppendCallback(() => cardPartHolder.Hide(0.5f, 0.2f));
        selectedCardSequence.AppendInterval(1f);
        if (replacedWithSamePart) selectedCardSequence.AppendInterval(1.5f);

        selectedCardSequence.AppendCallback(() => upgradeCardHolder.StartFinalRetrieve(0.2f, 0.5f, 0.2f));
        selectedCardSequence.AppendCallback(() => FinishResultCard());

    }

    private void FinishResultCard()
    {
        consoleDialog.Clear();
        PrintConsoleLine(TextTypes.SYSTEM, "Replacement done successfully", false, 0f);

        upgradeCardHolder.OnFinalRetrieve += InvokeReplacementDone;
    }




    private void UpdatePreviewCard_CardAndCardPart(TurretBuildingCard previewCard, TurretBuildingCard selectedCard, CardPart selectedCardPart)
    {
        previewCard.RootCardTransform.gameObject.SetActive(true);

        TurretPartAttack turretPartAttack = null;
        TurretPartBody turretPartBody = null;
        TurretPartBase turretPartBase = null;
        TurretPassiveBase turretPassiveBase = null;

        if (partType == PartType.ATTACK)
        {
            turretPartAttack = selectedCardPart.gameObject.GetComponent<CardPartAttack>().turretPartAttack;
        }
        else if (partType == PartType.BODY)
        {
            turretPartBody = selectedCardPart.gameObject.GetComponent<CardPartBody>().turretPartBody;
        }
        else if (partType == PartType.BASE)
        {
            turretPartBase = selectedCardPart.gameObject.GetComponent<CardPartBase>().turretPartBase;
            turretPassiveBase = selectedCardPart.gameObject.GetComponent<CardPartBase>().turretPassiveBase;
        }

        previewCard.PreviewChangeVisuals(turretPartAttack, turretPartBody, turretPartBase, turretPassiveBase, selectedCard, 
            partType, playCostSubtractAmountSamePart);
    }

    private void UpdatePreviewCard_MissingParts(TurretBuildingCard previewCard, bool cardIsMissing, bool cardPartIsMissing)
    {
        previewCard.RootCardTransform.gameObject.SetActive(false);

        if (cardIsMissing)
        {
            // DO WHATEVER
        }
        if (cardPartIsMissing)
        {
            // DO WHATEVER
        }
    }


}
