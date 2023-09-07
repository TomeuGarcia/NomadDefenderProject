using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SelectableDeck;

public class SelectableDeck : MonoBehaviour
{
    [Header("DECK DATA")]
    [SerializeField] private DeckData deckData;

    [Header("RUN CONTENT")]
    [SerializeField] private RunUpgradesContent runContent;

    [Header("PARAMETERS")]
    [SerializeField] private string firstMaterialLightName;
    [SerializeField] private string secondMaterialLightName;
    [SerializeField] private int materialBorderIndex;

    [Header("REFERENCES")]
    [SerializeField] private Transform cardsHolder;
    [SerializeField] private Collider interactionCollider;
    [SerializeField] private MeshRenderer pulsingMesh;
    [SerializeField] private MeshRenderer screenMesh;
    private Material pulsingMaterial;
    private int isSelectedPropertyId;

    public Color DeckColor {get; private set;}

    [SerializeField] private SpriteRenderer supportSprite;
    [SerializeField] private SpriteRenderer mainProjectileSprite;

    private BuildingCard[] cards;
    private DeckSelector deckSelector;

    private bool isSelected;


    private readonly Vector3 faceUpRotationOffset = Vector3.right * 90.0f;

    public RunUpgradesContent RunContent => runContent;
    public DeckData DeckData => deckData;
    public Transform CardsHolder => cardsHolder;
    public Vector3 Position => cardsHolder.position;

    public bool FinishedArranging { get; private set; }


    [System.Serializable]
    public class RunUpgradesContent
    {
        [SerializeField] public CardsLibraryContent cardsContent;
        [SerializeField] public AttackPartsLibraryContent attacksContent;
        [SerializeField] public BodyPartsLibraryContent bodiesContent;
        [SerializeField] public BasePartsLibraryContent basesContent;
    }

    [System.Serializable]
    public class ArrangeCardsData
    {
        [SerializeField] private Vector3 displacement;
        [SerializeField] private Vector3 maxRotationAngle;

        public Vector3 DisplacementBetweenCards => displacement;
        public Vector3 RandomRotationAngle => new Vector3(Random.Range(-maxRotationAngle.x, maxRotationAngle.x), 
                                                          Random.Range(-maxRotationAngle.y, maxRotationAngle.y), 
                                                          Random.Range(-maxRotationAngle.z, maxRotationAngle.z));
    }

    private void OnValidate()
    {
        SetDeckSprites();
    }

    private void Awake()
    {
        SetDeckSprites();

        pulsingMaterial = pulsingMesh.material;

        isSelectedPropertyId = Shader.PropertyToID("_IsSelected");
    }


    private void OnMouseDown()
    {
        deckSelector.OnDeckSelected(this);
    }

    private void OnMouseEnter()
    {
        if (isSelected) return;
        SetHovered();
    }
    private void OnMouseExit()
    {
        if (isSelected) return;
        SetNotHovered();
    }

    private void SetDeckSprites()
    {
        if (deckData == null || supportSprite == null || mainProjectileSprite == null) return;

        TurretPartBase supportBasePart = deckData.starterSupportCardsComponents[deckData.starterSupportCardsComponents.Count - 1].turretPartBase;
        supportSprite.sprite = supportBasePart.abilitySprite;
        supportSprite.color = supportBasePart.spriteColor;
        DeckColor = supportBasePart.spriteColor;

        TurretPartAttack mainTurretAttackPart = deckData.starterTurretCardsComponents[deckData.starterTurretCardsComponents.Count - 1].turretPartAttack;
        mainProjectileSprite.sprite = mainTurretAttackPart.abilitySprite;
        mainProjectileSprite.color = mainTurretAttackPart.materialColor;
    }


    public void InitReferences(DeckSelector deckSelector)
    {
        this.deckSelector = deckSelector;
    }

    public void InitSpawnCards(DeckCreator deckCreator)
    {
        deckCreator.SpawnCardsAndResetDeckData(deckData, out cards);
    }

    public void InitArrangeCards(ArrangeCardsData arrangeCardsData)
    {
        for (int i = 0; i < cards.Length; ++i)
        {
            BuildingCard card = cards[i];

            card.DisableMouseInteraction();

            card.RootCardTransform.SetParent(cardsHolder.transform);

            card.RootCardTransform.localRotation = Quaternion.Euler(arrangeCardsData.RandomRotationAngle + Vector3.right * 90f);
            card.RootCardTransform.localPosition = i * arrangeCardsData.DisplacementBetweenCards;
        }
    }


    public void SetEnabledInteraction(bool isEnabled)
    {
        interactionCollider.enabled = isEnabled;
    }



    public IEnumerator ArrangeCardsFromFirst(float motionDuration, float delayBetweenCards, ArrangeCardsData arrangeCardsData, Transform newParent)
    {
        FinishedArranging = false;
        for (int i = 0; i < cards.Length; ++i)
        {
            ArrangeCard(cards[i], i, motionDuration, arrangeCardsData, newParent);

            yield return new WaitForSeconds(delayBetweenCards);
        }
        FinishedArranging = true;
    }
    
    public IEnumerator ArrangeCardsFromLast(float motionDuration, float delayBetweenCards, ArrangeCardsData arrangeCardsData, Transform newParent)
    {
        FinishedArranging = false;
        for (int i = cards.Length - 1; i >= 0; --i)
        {           
            ArrangeCard(cards[i], cards.Length-1 - i, motionDuration, arrangeCardsData, newParent);
            GameAudioManager.GetInstance().PlayCardInfoMoveShown();

            yield return new WaitForSeconds(delayBetweenCards);
        }
        FinishedArranging = true;
    }

    private void ArrangeCard(BuildingCard card, int i, float motionDuration, ArrangeCardsData arrangeCardsData, Transform newParent)
    {
        card.RootCardTransform.SetParent(newParent);

        card.RootCardTransform.DOLocalRotateQuaternion(Quaternion.Euler(arrangeCardsData.RandomRotationAngle + faceUpRotationOffset), motionDuration).SetEase(Ease.OutCubic);
        card.RootCardTransform.DOLocalMove(i * arrangeCardsData.DisplacementBetweenCards, motionDuration).SetEase(Ease.OutCubic);
    }

    public void EnableCardsMouseInteraction()
    {
        foreach (BuildingCard card in cards)
        {
            card.ReenableMouseInteraction();
            card.canDisplayInfoIfNotInteractable = true;
        }
    }

    public void DisableCardsMouseInteraction()
    {
        foreach (BuildingCard card in cards)
        {
            card.DisableMouseInteraction();
            card.canDisplayInfoIfNotInteractable = false;
        }
    }

    public void SetSelected()
    {
        isSelected = true;
        pulsingMaterial.SetFloat(isSelectedPropertyId, 1.0f);

        ChangeBorderLight(firstMaterialLightName, 0.0f, 1.0f, 0.8f);
    }
    public void SetNotSelected()
    {
        isSelected = false;
        pulsingMaterial.SetFloat(isSelectedPropertyId, 0.0f);

        ChangeBorderLight(firstMaterialLightName, 1.0f, 0.0f, 0.2f);
        ChangeBorderLight(secondMaterialLightName, 1.0f, 0.0f, 0.2f);
    }


    private void ChangeBorderLight(string reference, float start, float goal, float duration)
    {
        DOTween.To(
            () => start,
            (x) => { start = x;
                screenMesh.materials[materialBorderIndex].SetFloat(reference, start);
            },
            goal,
            duration
        );
    }

    private void SetHovered()
    {
        pulsingMaterial.SetFloat(isSelectedPropertyId, 1.0f);

        GameAudioManager.GetInstance().PlayCardInfoMoveShown();
        ChangeBorderLight(secondMaterialLightName, 0.0f, 1.0f, 0.2f);
    }
    private void SetNotHovered()
    {
        pulsingMaterial.SetFloat(isSelectedPropertyId, 0.0f);

        GameAudioManager.GetInstance().PlayCardInfoMoveHidden();
        ChangeBorderLight(secondMaterialLightName,  1.0f,0.0f, 0.2f);
    }

}
