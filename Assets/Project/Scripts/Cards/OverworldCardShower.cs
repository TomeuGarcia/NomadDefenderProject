using System.Collections;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverworldCardShower : MonoBehaviour
{
    [Header("DECK DATA")]
    [SerializeField] private DeckData deckData;

    [Header("DEPENDENCIES")]
    [SerializeField] private DeckCreator deckCreator;
    [SerializeField] private Camera followCamera;

    [Header("CAMERA")]
    [SerializeField] private Camera cardShowerCamera;

    [Header("BUTTONS")]
    [SerializeField] private Button showDeckButton;
    [SerializeField] private Button backToMapButton;
    [SerializeField] private CanvasGroup showDeckButtonCG;
    [SerializeField] private bool showButtons = true;

    private List<BuildingCard> cards;
    private Vector3 prevCameraPos;
    private Quaternion prevCameraRot;
    private static bool showingDeck;
    Vector3 lastHoveredCardPos = Vector3.zero;
    BuildingCard currentSelectedCard = null;
    BuildingCard cardBeingDeselected = null;
    private Coroutine currentCoroutine;

    Dictionary<BuildingCard, Vector3> positions;



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
        cards = new List<BuildingCard>(deckData.GetCards());
        foreach (BuildingCard itCard in cards)
        {
            itCard.OnCardUnhovered += SetStandardCard;
            itCard.OnCardHovered += SetHoveredCard;
            //itCard.OnCardSelected += SelectCard;
            Quaternion rotation = transform.rotation;
            itCard.RootCardTransform.rotation = Quaternion.Euler(90, 0, 0);
            itCard.InitPositions(Vector3.up * 3.5f, Vector3.zero, itCard.RootCardTransform.position);
            itCard.RootCardTransform.SetParent(transform);

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
        ResetDeckData();
        Init();
    }


    public void DestroyAllCards()
    {
        for(int i = 0; i < cards.Count; i++)
        {
            Destroy(cards[i].gameObject);
        }
    }

    private void ResetDeckData()
    {
        deckCreator.SpawnCardsAndResetDeckData();
    }
    public void OnShowDeck()
    {
        CardDescriptionDisplayer.GetInstance().SetCamera(cardShowerCamera);

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
        currentCoroutine =  StartCoroutine(setCardsInPlace());

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
        buildingCard.RootCardTransform.DOMove(Vector3.zero - Vector3.up * -2.5f, 0.35f);
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

    IEnumerator setCardsInPlace()
    {
        positions = new Dictionary<BuildingCard, Vector3>();
        float numCards = cards.Count;
        float yOffset = 3.7f;
        currentSelectedCard = null;
        foreach (BuildingCard itCard in cards)
        {
            itCard.RootCardTransform.DOComplete();
            itCard.transform.position = new Vector3(0, 3, 3.5f);
            itCard.StandardState();
        }
        for (int i = 0; i < cards.Count; ++i)
        {
            if (i % 8 == 0)
            {
                yOffset -= 2f;
            }
           

            Vector3 targetPos;
            yield return new WaitForSecondsRealtime(0.1f);
            //TODO: Play Sound

            cards[i].DisableMouseInteraction();
            targetPos = Vector3.zero - Vector3.up * 0.25f;
            targetPos -= transform.right * (1.25f * (-(i % 8 + 1))) - transform.right * -5.5f;
            targetPos += transform.forward * yOffset;

            cards[i].cardLocation = BuildingCard.CardLocation.DECK;

            cards[i].RootCardTransform.DOMove(targetPos, 0.5f);
            positions.Add(cards[i], targetPos);
        }
        yield return new WaitForSecondsRealtime(0.5f);

        foreach (BuildingCard itCard in cards)
            itCard.EnableMouseInteraction();


    }
}
