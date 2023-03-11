using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class CardPartReplaceManager : MonoBehaviour
{
    [Header("UPGRADE SETUP")]
    [SerializeField] private UpgradeSceneSetupInfo upgradeSceneSetupInfo;

    [Header("SCENE MANAGEMENT")]
    [SerializeField] private MapSceneNotifier mapSceneNotifier;

    [Header("DECK DATA")]
    [SerializeField] private DeckData deckData;
    private List<BuildingCard> deckCards;

    [Header("HOLDERS")]
    [SerializeField] private UpgradeCardHolder upgradeCardHolder;
    [SerializeField] private CardPartHolder cardPartHolder;
    public UpgradeCardHolder UpgradeCardHolder => upgradeCardHolder;
    public CardPartHolder CardPartHolder => cardPartHolder;

    private enum PartType { ATTACK, BODY, BASE }
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
    [SerializeField] private GameObject buttonText;
    [SerializeField] private Animator buttonAnimator;
    [SerializeField] private MouseOverNotifier buttonMouseOverNotifier;

    [SerializeField] ConsoleDialogSystem consoleDialog;
    [SerializeField] TextLine textLine;
    [SerializeField] bool isTutorial;

    private bool replacementDone = false;
    public bool ReplacementDone => replacementDone;

    private bool cardIsReady = false;
    private bool partIsReady = false;
    [HideInInspector] public bool canFinalRetrieveCard = true;

    [Header("MATERIALS")]
    [SerializeField] private MeshRenderer buttonMeshRenderer;
    private Material buttonMaterial;


    private int numPartsIfPerfect = 1;
    private bool lastBattleWasDefendedPerfectly = false;
    NodeEnums.ProgressionState progressionState = NodeEnums.ProgressionState.EARLY;


    public delegate void CarPartReplaceManagerAction();
    public static event CarPartReplaceManagerAction OnReplacementStart;
    public static event CarPartReplaceManagerAction OnReplacementDone;


    // TUTORIAL
    private bool partsCreatedByTutorial = false;

    

    private void Start()
    {
        deckCards = deckData.GetCardsReference();

        buttonMaterial = buttonMeshRenderer.material;

        SetButtonNotReady();
        Init();

        PlayCardAndCardPartsAppearAnimation();
    }

    private void OnEnable()
    {
        buttonMouseOverNotifier.OnMousePressed += ProceedReplace;

        upgradeCardHolder.OnCardSelected += CardWasSelected;
        upgradeCardHolder.OnCardUnselected += CardWasUnselected;
        cardPartHolder.OnPartSelected += PartWasSelected;
        cardPartHolder.OnPartUnselected += PartWasUnselected;
    }

    private void OnDisable()
    {
        buttonMouseOverNotifier.OnMousePressed -= ProceedReplace;

        upgradeCardHolder.OnCardSelected -= CardWasSelected;
        upgradeCardHolder.OnCardUnselected -= CardWasUnselected;
        cardPartHolder.OnPartSelected -= PartWasSelected;
        cardPartHolder.OnPartUnselected -= PartWasUnselected;
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
        HashSet<int> randomIndices = new HashSet<int>();
        while (randomIndices.Count < numCards)
        {
            int randomIndex = Random.Range(0, deckCards.Count);
            if (deckCards[randomIndex].cardBuildingType == BuildingCard.CardBuildingType.TURRET)
            {
                randomIndices.Add(randomIndex);
            }
        }

        BuildingCard[] chosenCards = new BuildingCard[numCards];
        int i = 0;
        foreach (int index in randomIndices)
        {
            chosenCards[i] = deckCards[index];
            chosenCards[i].transform.SetParent(upgradeCardHolder.transform, false);
            ++i;
        }

        return chosenCards;
    }


    public void ProceedReplace()
    {
        if (replacementDone) return;

        if (CanConfirm())
        {
            buttonAnimator.SetTrigger("Pressed");
            buttonMaterial.SetFloat("_IsAlwaysOn", 1f);

            replacementDone = true;

            StartCoroutine(ReplecementAnimation());

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
        upgradeCardHolder.StartAnimation();

        if (partIsReady) SetButtonReady();
    }

    private void CardWasUnselected()
    {
        cardIsReady = false;
        upgradeCardHolder.FinishAnimation();

        if (partIsReady) SetButtonNotReady();
    }

    private void PartWasSelected()
    {
        partIsReady = true;
        cardPartHolder.StartAnimation();

        if (cardIsReady) SetButtonReady();
    }

    private void PartWasUnselected()
    {
        partIsReady = false;
        cardPartHolder.FinishAnimation();

        if (cardIsReady) SetButtonNotReady();
    }

    private void SetButtonReady()
    {
        buttonText.SetActive(true);

        buttonMaterial.SetFloat("_IsOn", 1f);
    }

    private void SetButtonNotReady()
    {
        buttonText.SetActive(false);

        buttonMaterial.SetFloat("_IsOn", 0f);
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
        buttonMaterial.SetFloat("_IsAlwaysOn", 0f);
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

}
