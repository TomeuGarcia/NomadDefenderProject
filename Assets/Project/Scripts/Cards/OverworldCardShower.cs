using System.Collections;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class OverworldCardShower : MonoBehaviour
{

    [SerializeField] private DeckData deckData;
    [SerializeField] private DeckCreator deckCreator;
    [SerializeField] private GameObject followCamera;
    [SerializeField] private GameObject showDeckButton;
    [SerializeField] private GameObject backToMapButton;
    private List<BuildingCard> cards;
    private Vector3 prevCameraPos;
    private Quaternion prevCameraRot;
    private bool showingDeck;
    Vector3 lastHoveredCardPos = Vector3.zero;
    BuildingCard currentSelectedCard = null;
    BuildingCard cardBeingDeselected = null;
    private Coroutine currentCoroutine;

    Dictionary<BuildingCard, Vector3> positions;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }
    private void Init()
    {
        showDeckButton.SetActive(true);
        backToMapButton.SetActive(false);
        showingDeck = false;
        cards = new List<BuildingCard>(deckData.GetCards());
        foreach (BuildingCard itCard in cards)
        {
            itCard.OnCardUnhovered += SetStandardCard;
            itCard.OnCardHovered += SetHoveredCard;
            itCard.OnCardSelected += SelectCard;
            Quaternion rotation = transform.rotation;
            itCard.RootCardTransform.rotation = Quaternion.Euler(90, 0, 0);
            itCard.InitPositions(Vector3.zero + Vector3.up * 3.5f, Vector3.zero);
            itCard.RootCardTransform.SetParent(transform);

        }
    }
    public void ResetAll ()
    {
        //DestroyAllCards();
        ResetDeckData();
        Init();
    }
    private void OnDisable()
    {

        foreach (BuildingCard itCard in cards)
        {
            itCard.OnCardHovered -= SetHoveredCard;
            itCard.OnCardUnhovered -= SetStandardCard;
            itCard.OnCardSelected -= SelectCard;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (!showingDeck)
                OnShowDeck();
            else
                OnBackToMap();
        }
        if (Input.GetMouseButtonDown(2))
        {
            DeselectCard();
        }

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
        showingDeck = true;
        prevCameraPos = followCamera.transform.position;
        prevCameraRot = followCamera.transform.rotation;
        //followCamera.transform.position = transform.position + Vector3.up * 5;
        //followCamera.transform.rotation = Quaternion.Euler(90, 0, 0);
        followCamera.SetActive(false);
        transform.GetChild(0).gameObject.SetActive(true);

        showDeckButton.SetActive(false);
        backToMapButton.SetActive(true);
        StartCoroutine(setCardsInPlace());

    }

    public void OnBackToMap()
    {
        showingDeck = false;
        //followCamera.transform.position = prevCameraPos;
        //followCamera.transform.rotation = prevCameraRot;

        followCamera.SetActive(true);
        transform.GetChild(0).gameObject.SetActive(false);
         backToMapButton.SetActive(false);
        showDeckButton.SetActive(true);
    }



    void SetHoveredCard(BuildingCard buildingCard)
    {
        if (currentSelectedCard == buildingCard || cardBeingDeselected == buildingCard)
            return;

        lastHoveredCardPos = buildingCard.transform.position;

        GameAudioManager.GetInstance().PlayCardHovered();
        buildingCard.HoveredState();
    }
    void SelectCard(BuildingCard buildingCard)
    {
        if (currentSelectedCard == buildingCard || cardBeingDeselected == buildingCard)
            return;

        //buildingCard.StandardState();
        buildingCard.OnCardInfoSelected += ShowCardInfo;
        DeselectCard();
        currentSelectedCard = buildingCard;
        buildingCard.RootCardTransform.DOMove(Vector3.zero - Vector3.up * -2.5f, 0.35f);
    }


    void SetStandardCard(BuildingCard buildingCard)
    {

        if (currentSelectedCard == buildingCard && buildingCard.isShowingInfo)
        {
            HideCardInfo(buildingCard);
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



        if (currentSelectedCard.isShowingInfo)
            HideCardInfo(currentSelectedCard);

        cardBeingDeselected = currentSelectedCard;
        currentSelectedCard.RootCardTransform.DOMove(positions[currentSelectedCard], 0.35f);
        currentSelectedCard.StandardState();
        currentSelectedCard.OnCardInfoSelected -= ShowCardInfo;
        currentSelectedCard = null;
        cardBeingDeselected = null;

    }

  
    void ShowCardInfo(BuildingCard buildingCard)
    {
        buildingCard.ShowInfo();
        buildingCard.OnCardInfoSelected -= ShowCardInfo;
        buildingCard.OnCardInfoSelected += HideCardInfo;
    }
    void HideCardInfo(BuildingCard buildingCard)
    {
        buildingCard.HideInfo();
        buildingCard.OnCardInfoSelected += ShowCardInfo;
        buildingCard.OnCardInfoSelected -= HideCardInfo;
    }

    IEnumerator setCardsInPlace()
    {
        positions = new Dictionary<BuildingCard, Vector3>();
        float numCards = cards.Count;
        float yOffset = 3.7f;
        currentSelectedCard = null;
        foreach (BuildingCard itCard in cards)
        {
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
