using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardPartReplaceManager : MonoBehaviour
{
    [Header("DECK DATA")]
    [SerializeField] private DeckData deckData;
    private List<BuildingCard> deckCards;

    [Header("HOLDERS")]
    [SerializeField] private UpgradeCardHolder upgradeCardHolder;
    [SerializeField] private CardPartHolder cardPartHolder;

    private enum PartType { ATTACK, BODY, BASE }
    [Header("TYPE")]
    [SerializeField] private int numCards = 3;
    [SerializeField] private int numParts = 3;
    [SerializeField] private PartType partType;

    [Header("ATTACK")]
    [SerializeField] private GameObject cardPartAttackPrefab;
    [SerializeField] private TurretPartAttack[] attacks;
    [Header("BODY")]
    [SerializeField] private GameObject cardPartBodyPrefab;
    [SerializeField] private TurretPartBody[] bodies;
    [Header("BASE")]
    [SerializeField] private GameObject cardPartBasePrefab;
    [SerializeField] private TurretPartBase[] bases;

    [Header("COMPONENTS")]
    [SerializeField] private TextMeshProUGUI uiDescriptionText;
    [SerializeField] private GameObject buttonText;
    [SerializeField] private Animator buttonAnimator;
    [SerializeField] private MouseOverNotifier buttonMouseOverNotifier;

    private bool replacementDone = false;

    private bool cardIsReady = false;
    private bool partIsReady = false;

    [Header("MATERIALS")]
    [SerializeField] private MeshRenderer buttonMeshRenderer;
    private Material buttonMaterial;

    


    public delegate void CarPartReplaceManagerAction();
    public static event CarPartReplaceManagerAction OnReplacementStart;
    public static event CarPartReplaceManagerAction OnReplacementDone;

    

    private void Start()
    {
        deckCards = deckData.GetCardsReference();

        buttonMaterial = buttonMeshRenderer.material;

        SetButtonNotReady();
        Init();
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
        if (partType == PartType.ATTACK) InitAttacks();
        else if (partType == PartType.BODY) InitBodies();
        else if (partType == PartType.BASE) InitBases();

        upgradeCardHolder.Init(GetRandomDeckCards());
    }

    private void InitAttacks()
    {
        CardPartAttack[] parts = new CardPartAttack[numParts];
        for (int i = 0; i < numParts; ++i)
        {
            parts[i] = Instantiate(cardPartAttackPrefab, cardPartHolder.cardsHolderTransform).GetComponent<CardPartAttack>();
            parts[i].turretPartAttack = GetRandomTurretPartAttack();
        }
        cardPartHolder.Init(parts);

        uiDescriptionText.text = "Replace a turret's ATTACK with a new one";
    }

    private void InitBodies()
    {
        CardPartBody[] parts = new CardPartBody[numParts];
        for (int i = 0; i < numParts; ++i)
        {
            parts[i] = Instantiate(cardPartBodyPrefab, cardPartHolder.cardsHolderTransform).GetComponent<CardPartBody>();
            parts[i].turretPartBody = GetRandomTurretPartBody();
        }
        cardPartHolder.Init(parts);

        uiDescriptionText.text = "Replace a turret's BODY with a new one";
    }

    private void InitBases()
    {
        CardPartBase[] parts = new CardPartBase[numParts];
        for (int i = 0; i < numParts; ++i)
        {
            parts[i] = Instantiate(cardPartBasePrefab, cardPartHolder.cardsHolderTransform).GetComponent<CardPartBase>();
            parts[i].turretPartBase = GetRandomTurretPartBase();
        }
        cardPartHolder.Init(parts);

        uiDescriptionText.text = "Replace a turret's BASE with a new one";
    }

    // TODO make this give sets so no parts are repeated!!!!!!!!!
    private TurretPartAttack GetRandomTurretPartAttack()
    {
        return attacks[Random.Range(0, attacks.Length)];
    }
    private TurretPartBody GetRandomTurretPartBody()
    {
        return bodies[Random.Range(0, bodies.Length)];
    }
    private TurretPartBase GetRandomTurretPartBase()
    {
        return bases[Random.Range(0, bases.Length)];
    }

    private BuildingCard[] GetRandomDeckCards()
    {
        HashSet<int> randomIndices = new HashSet<int>();
        while (randomIndices.Count < numCards)
        {
            randomIndices.Add(Random.Range(0, deckCards.Count));
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
        }
    }


    private bool CanConfirm()
    {
        return cardIsReady && partIsReady;
    }

    
    private void ReplacePartInCard()
    {
        switch (partType)
        {
            case PartType.ATTACK:
                {
                    upgradeCardHolder.selectedCard.SetNewPartAttack(cardPartHolder.selectedCardPart.gameObject.GetComponent<CardPartAttack>().turretPartAttack);
                }
                break;
            case PartType.BODY:
                {
                    upgradeCardHolder.selectedCard.SetNewPartBody(cardPartHolder.selectedCardPart.gameObject.GetComponent<CardPartBody>().turretPartBody);
                }
                break;
            case PartType.BASE:
                {
                    upgradeCardHolder.selectedCard.SetNewPartBase(cardPartHolder.selectedCardPart.gameObject.GetComponent<CardPartBase>().turretPartBase);
                }
                break;
            default: break;
        }
    }


    private IEnumerator ReplecementAnimation()
    {
        upgradeCardHolder.StopInteractions();
        cardPartHolder.StopInteractions();

        float partMoveDuration = 0.2f;
        float flipDuration = 0.4f;
        float moveDuration = 0.5f;

        Transform upgradedCardTransform = upgradeCardHolder.selectedCard.transform;
        Vector3 upgradedCardStartPos = upgradedCardTransform.position;
        Transform partTransform = cardPartHolder.selectedCardPart.transform;


        yield return new WaitForSeconds(0.3f);

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
        ReplacePartInCard();
        InvokeReplacementStart();

        // Flip up
        upgradedCardTransform.DOLocalRotate(Vector3.up * 360f, flipDuration);
        yield return new WaitForSeconds(flipDuration);

        // Rise down card
        upgradedCardTransform.DOMove(upgradedCardStartPos, moveDuration);
        yield return new WaitForSeconds(moveDuration);

        // Hide parts
        cardPartHolder.Hide(0.5f, 0.2f);
        yield return new WaitForSeconds(1f);

        // Enable FINAL retreieve card
        upgradeCardHolder.EnableFinalRetrieve(0.2f, 0.5f, 0.2f);

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

        if (OnReplacementDone != null) OnReplacementDone(); // Subscribe to this event to load NEXT SCENE
    }

    private void InvokeReplacementStart()
    {
        if (OnReplacementStart != null) OnReplacementStart();

        SetButtonNotReady();
        buttonMaterial.SetFloat("_IsAlwaysOn", 0f);
    }


}
