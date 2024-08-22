using System;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SelectableDeck : MonoBehaviour
{
    [Header("CARDS DECK")]
    [SerializeField] private CardDeckAsset _deck;

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

    [SerializeField] private SpriteRenderer supportSprite;
    [SerializeField] private SpriteRenderer mainProjectileSprite;

    [Header("UNLOCK")]
    [SerializeField] private GameObject _unlockedCardsHolder;
    [SerializeField] private GameObject _lock;    


    public Color DeckColor {get; private set;}

    private BuildingCard[] _cards;
    private DeckSelector deckSelector;

    private bool isSelected;
    private bool _isUnlocked;


    private readonly Vector3 faceUpRotationOffset = Vector3.right * 90.0f;

    public RunUpgradesContent RunContent => runContent;
    public CardDeckAsset Deck => _deck;
    public Transform CardsHolder => cardsHolder;
    public Vector3 Position => cardsHolder.position;

    public bool FinishedArranging { get; private set; }


    [System.Serializable]
    public class RunUpgradesContent
    {
        [SerializeField] public CardsLibraryContent cardsContent;
        [SerializeField] public AttackPartsLibraryContent attacksContent;
        [SerializeField] public BodyPartsLibraryContent bodiesContent;
        [SerializeField] public PassivesLibraryContent basesContent;
        [SerializeField] public BonusStatsPartsLibraryContent bonusStatsContent;

    }

    [System.Serializable]
    public class ArrangeCardsData
    {
        [SerializeField] private Vector3 displacement;
        [SerializeField] private Vector3 maxRotationAngle;
        [SerializeField] private AnimationCurve _rotationEase;
        [SerializeField] private AnimationCurve _movementEase;
        
        public AnimationCurve RotationEase => _rotationEase;
        public AnimationCurve MovementEase => _movementEase;

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
        if (_deck == null || supportSprite == null || mainProjectileSprite == null) return;

        SupportPartBase supportBasePart = _deck.MainSupportCardDataModel().SharedPartsGroup.Base;
        supportSprite.sprite = supportBasePart.abilitySprite;
        supportSprite.color = supportBasePart.spriteColor;
        DeckColor = supportBasePart.spriteColor;

        TurretPartProjectileDataModel mainTurretAttackPart = _deck.MainTurretCardDataModel().SharedPartsGroup.Projectile;
        mainProjectileSprite.sprite = mainTurretAttackPart.abilitySprite;
        mainProjectileSprite.color = mainTurretAttackPart.materialColor;
    }


    public void InitReferences(DeckSelector deckSelector)
    {
        this.deckSelector = deckSelector;
    }

    public void InitSpawnCards(ICardSpawnService cardSpawnService)
    {
        _cards = cardSpawnService.MakeAllCardsFromDeck(_deck.MakeDeckContent(), cardsHolder);
    }

    public void InitArrangeCards(ArrangeCardsData arrangeCardsData)
    {
        for (int i = 0; i < _cards.Length; ++i)
        {
            BuildingCard card = _cards[i];

            card.DisableMouseInteraction();
            card.hideInfoWhenSelected = false;

            card.RootCardTransform.SetParent(cardsHolder.transform);

            card.RootCardTransform.localRotation = Quaternion.Euler(arrangeCardsData.RandomRotationAngle + Vector3.right * 90f);
            card.RootCardTransform.localPosition = i * arrangeCardsData.DisplacementBetweenCards;

            card.MotionEffectsController.DisableMotion();
        }
    }


    public void SetEnabledInteraction(bool isEnabled)
    {
        interactionCollider.enabled = isEnabled && _isUnlocked;
    }

    public void DisableShowInfo()
    {
        foreach (BuildingCard card in _cards)
        {
            card.canDisplayInfoIfNotInteractable = false;
            if (card.isShowingInfo)
            {
                card.HideInfo();
            }
        }
    }


    public IEnumerator ArrangeCardsFromFirst(float motionDuration, float delayBetweenCards, ArrangeCardsData arrangeCardsData, Transform newParent,
        bool isFloating)
    {
        FinishedArranging = false;
        for (int i = 0; i < _cards.Length; ++i)
        {
            ArrangeCard(_cards[i], i, motionDuration, arrangeCardsData, newParent, isFloating);

            yield return new WaitForSeconds(delayBetweenCards);
        }
        FinishedArranging = true;
    }
    
    public IEnumerator ArrangeCardsFromLast(float motionDuration, float delayBetweenCards, ArrangeCardsData arrangeCardsData, Transform newParent, 
        bool isFloating)
    {
        FinishedArranging = false;
        for (int i = _cards.Length - 1; i >= 0; --i)
        {           
            ArrangeCard(_cards[i], _cards.Length-1 - i, motionDuration, arrangeCardsData, newParent, isFloating);
            GameAudioManager.GetInstance().PlayCardInfoMoveShown();

            yield return new WaitForSeconds(delayBetweenCards);
        }
        FinishedArranging = true;
    }

    private void ArrangeCard(BuildingCard card, int i, float motionDuration, ArrangeCardsData arrangeCardsData, Transform newParent, 
        bool isFloating)
    {
        card.RootCardTransform.SetParent(newParent);

        card.RootCardTransform.DOLocalRotateQuaternion(Quaternion.Euler(arrangeCardsData.RandomRotationAngle + faceUpRotationOffset), motionDuration)
            .SetEase(arrangeCardsData.RotationEase);
        card.RootCardTransform.DOLocalMove(i * arrangeCardsData.DisplacementBetweenCards, motionDuration)
            .SetEase(arrangeCardsData.MovementEase);

        if (isFloating)
        {
            card.MotionEffectsController.EnableMotion();
        }
        else
        {
            card.MotionEffectsController.DisableMotion();
        }
    }

    public void EnableCardsMouseInteraction()
    {
        foreach (BuildingCard card in _cards)
        {
            card.ReenableMouseInteraction();
            card.canDisplayInfoIfNotInteractable = true;
        }
    }

    public void DisableCardsMouseInteraction()
    {
        foreach (BuildingCard card in _cards)
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


    public void InitState(bool isUnlocked)
    {
        _isUnlocked = isUnlocked;
        _unlockedCardsHolder.SetActive(isUnlocked);
        _lock.SetActive(!isUnlocked);

        SetEnabledInteraction(isUnlocked);
    }

}
