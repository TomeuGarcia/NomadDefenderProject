using System.Collections;
using DG.Tweening;
using System.Collections.Generic;
using Project.Scripts.Cards;
using UnityEngine;
using UnityEngine.UI;

public class OverworldCardShower : MonoBehaviour
{
    [Header("DECK IN USE")]
    [SerializeField] private CardDeckInUseData _deckInUse;

    [Header("DEPENDENCIES")]
    [SerializeField] private Camera followCamera;

    [Header("CAMERA")]
    [SerializeField] private Camera cardShowerCamera;

    [Header("BUTTONS")]
    [SerializeField] private Button showDeckButton;
    [SerializeField] private Button backToMapButton;
    [SerializeField] private CanvasGroup showDeckButtonCG;
    [SerializeField] private bool showButtons = true;

    [Header("CARDS")] 
    [SerializeField] private Transform _cardsHolder;
    [SerializeField] private Vector3 _cardsStartPosition = new Vector3(0, 3, 3.5f);
    [SerializeField, Min(1)] private int _placeCards_cardsPerRow = 7;
    [SerializeField, Min(0)] private float _placeCards_cardHeight = 1.5f;
    [SerializeField, Min(0)] private float _placeCards_cardWidth = 1.0f;
    [SerializeField, Min(0)] private float _placeCards_spacingBetweenCards = 0.25f;
    [SerializeField, Min(0)] private float _placeCards_spacingBetweenRows = 0.25f;
    [Header("DEBUG")]
    [SerializeField, Min(1)] private int _debugCardsCount = 9;


    private BuildingCard[] cards;
    private Vector3 prevCameraPos;
    private Quaternion prevCameraRot;
    private static bool showingDeck;
    Vector3 lastHoveredCardPos = Vector3.zero;
    BuildingCard currentSelectedCard = null;
    BuildingCard cardBeingDeselected = null;
    private Coroutine currentCoroutine;

    Dictionary<BuildingCard, Vector3> positions;

    private void Awake()
    {
        ServiceLocator.GetInstance().CameraHelp.SetCardsCamera(cardShowerCamera);
    }

    void Start()
    {
        Init();

        if (showButtons)
        {
            showDeckButton.gameObject.SetActive(true);
            backToMapButton.gameObject.SetActive(false); 
            StartCoroutine(PlayGameStartAnimation());
        }
        else
        {
            showDeckButton.gameObject.SetActive(false);
            backToMapButton.gameObject.SetActive(false);
        }

    }

    private void OnDisable()
    {

        foreach (BuildingCard itCard in cards)
        {
            itCard.OnCardHovered -= SetHoveredCard;
            itCard.OnCardUnhovered -= SetStandardCard;
            //itCard.OnCardSelected -= SelectCard;
        }

        showDeckButtonCG.alpha = 1f;
        showDeckButtonCG.interactable = true;
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.D))
        //{
        //    if (!showingDeck)
        //        OnShowDeck();
        //    else
        //        OnBackToMap();
        //}
        if (currentSelectedCard != null && Input.GetKeyDown(KeyCode.Mouse1))
        {
            DeselectCard();
        }
    }

    private void Init()
    {
        showDeckButton.gameObject.SetActive(true);
        backToMapButton.gameObject.SetActive(false);

        showingDeck = false;

        cards = _deckInUse.SpawnCurrentDeckBuildingCards(_cardsHolder);
        foreach (BuildingCard itCard in cards)
        {
            itCard.OnCardUnhovered += SetStandardCard;
            itCard.OnCardHovered += SetHoveredCard;
            //itCard.OnCardSelected += SelectCard;
            Quaternion rotation = transform.rotation;
            itCard.RootCardTransform.rotation = Quaternion.Euler(90, 0, 0);
            itCard.InitPositions(Vector3.up * 3.5f, Vector3.zero, itCard.RootCardTransform.position);
            itCard.RootCardTransform.SetParent(_cardsHolder);

        }
    }

    private IEnumerator PlayGameStartAnimation()
    {
        showDeckButtonCG.interactable = false;
        showDeckButtonCG.alpha = 0f;

        yield return new WaitForSeconds(3f);

        float t1 = 0.1f;
        showDeckButtonCG.DOFade(1f, t1);
        GameAudioManager.GetInstance().PlayCardInfoShown();
        yield return new WaitForSeconds(t1);

        showDeckButtonCG.DOFade(0f, t1);
        yield return new WaitForSeconds(t1*2);

        showDeckButtonCG.DOFade(1f, t1);
        GameAudioManager.GetInstance().PlayCardInfoShown();
        yield return new WaitForSeconds(t1);

        showDeckButtonCG.interactable = true;
    }

    public void ResetAll ()
    {
        //DestroyAllCards();
        Init();
    }


    public void DestroyAllCards()
    {
        for(int i = 0; i < cards.Length; i++)
        {
            Destroy(cards[i].gameObject);
        }
    }
    
    public void OnShowDeck()
    {
        CardTooltipDisplayManager.GetInstance().SetDisplayCamera(cardShowerCamera);

        showingDeck = true;

        prevCameraPos = followCamera.transform.position;
        prevCameraRot = followCamera.transform.rotation;
        //followCamera.transform.position = transform.position + Vector3.up * 5;
        //followCamera.transform.rotation = Quaternion.Euler(90, 0, 0);
        followCamera.gameObject.SetActive(false);
        transform.GetChild(0).gameObject.SetActive(true);

        showDeckButton.gameObject.SetActive(false);
        backToMapButton.gameObject.SetActive(true);

        if(currentCoroutine!= null) { StopCoroutine(currentCoroutine); }
        currentCoroutine =  StartCoroutine(SetCardsInPlace());

    }

    public void OnBackToMap()
    {
        showingDeck = false;
        //followCamera.transform.position = prevCameraPos;
        //followCamera.transform.rotation = prevCameraRot;
        

        followCamera.gameObject.SetActive(true);        
        transform.GetChild(0).gameObject.SetActive(false);
        backToMapButton.gameObject.SetActive(false);
        showDeckButton.gameObject.SetActive(true);
    }



    void SetHoveredCard(BuildingCard buildingCard)
    {
        if (currentSelectedCard == buildingCard || cardBeingDeselected == buildingCard)
            return;

        lastHoveredCardPos = buildingCard.transform.position;

        GameAudioManager.GetInstance().PlayCardHovered();
        buildingCard.HoveredState(rotate: false);
    }
    void SelectCard(BuildingCard buildingCard)
    {
        if (currentSelectedCard == buildingCard || cardBeingDeselected == buildingCard)
            return;

        //buildingCard.StandardState();

        //buildingCard.OnCardInfoSelected += ShowCardInfo;
        DeselectCard();
        currentSelectedCard = buildingCard;
        buildingCard.RootCardTransform.DOLocalMove(Vector3.zero - Vector3.up * -2.5f, 0.35f);
    }


    void SetStandardCard(BuildingCard buildingCard)
    {

        if (currentSelectedCard == buildingCard && buildingCard.isShowingInfo)
        {
            //HideCardInfo(buildingCard);
            return;
        }
        else if (currentSelectedCard == buildingCard)
        {
            return;
        }

        buildingCard.StandardState();
    }

    void DeselectCard()
    {
        if (currentSelectedCard == null)
            return;



        //if (currentSelectedCard.isShowingInfo)
        //    HideCardInfo(currentSelectedCard);

        cardBeingDeselected = currentSelectedCard;
        currentSelectedCard.RootCardTransform.DOMove(positions[currentSelectedCard], 0.35f);
        currentSelectedCard.StandardState();
        //currentSelectedCard.OnCardInfoSelected -= ShowCardInfo;
        currentSelectedCard = null;
        cardBeingDeselected = null;

    }

  
    //void ShowCardInfo(BuildingCard buildingCard)
    //{
    //    buildingCard.ShowInfo();
    //    buildingCard.OnCardInfoSelected -= ShowCardInfo;
    //    buildingCard.OnCardInfoSelected += HideCardInfo;
    //}
    //void HideCardInfo(BuildingCard buildingCard)
    //{
    //    buildingCard.HideInfo();
    //    buildingCard.OnCardInfoSelected += ShowCardInfo;
    //    buildingCard.OnCardInfoSelected -= HideCardInfo;
    //}

    IEnumerator SetCardsInPlace()
    {
        positions = new Dictionary<BuildingCard, Vector3>();
        currentSelectedCard = null;
        
        Vector3[] cardsEndPositions = ComputeCardsEndPositions(cards.Length); 
        
        for (int i = 0; i < cards.Length; ++i)
        {
            BuildingCard itCard = cards[i];
            itCard.RootCardTransform.DOComplete();
            itCard.transform.localPosition = _cardsStartPosition;
            itCard.StandardState();
            
            itCard.DisableMouseInteraction();
            itCard.ResizeColliderForShowcase();
            
            positions.Add(itCard, cardsEndPositions[i]);
        }


        float cardMoveDelay = 0.1f;
        float cardMoveDuration = 0.5f;
        float cardMoveSoundPitch = 1.2f;
        
        for (int i = 0; i < cards.Length; ++i)
        {
            yield return new WaitForSecondsRealtime(cardMoveDelay);
            //TODO: Play Sound

            GameAudioManager.GetInstance().PlayCardInfoMoveShown(cardMoveSoundPitch);
            
            cards[i].cardLocation = BuildingCard.CardLocation.DECK;

            cards[i].RootCardTransform.DOLocalMove(cardsEndPositions[i], cardMoveDuration).SetEase(Ease.OutQuart);

            cardMoveDelay *= 0.98f;
            cardMoveDuration *= 0.98f;
            cardMoveSoundPitch *= 1.02f;
        }
        yield return new WaitForSecondsRealtime(0.3f);

        foreach (BuildingCard itCard in cards)
        {
            itCard.EnableMouseInteraction();            
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Vector3 cardSize = new Vector3(_placeCards_cardWidth, 0.2f, _placeCards_cardHeight);        
        Vector3[] cardPositions = ComputeCardsEndPositions(_debugCardsCount);

        for (int i = 0; i < cardPositions.Length; ++i)
        {
            Vector3 position = cardPositions[i];

            Gizmos.DrawSphere(position, 0.1f);
            Gizmos.DrawCube(position, cardSize);
        }
    }

    private Vector3[] ComputeCardsEndPositions(int cardsCount)
    {
        return CardArrangingUtilities.GetCenteredCards(_cardsHolder, cardsCount, _placeCards_cardsPerRow,
            _placeCards_spacingBetweenRows, _placeCards_spacingBetweenCards,
            _placeCards_cardWidth, _placeCards_cardHeight);
    }
    
    
}
