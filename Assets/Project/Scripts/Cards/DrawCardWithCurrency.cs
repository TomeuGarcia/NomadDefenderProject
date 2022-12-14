using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class DrawCardWithCurrency : MonoBehaviour
{

    [Header("UI")] 
    [SerializeField] private GameObject DrawUI;
    [SerializeField] private TextMeshPro tittleText;
    [SerializeField] private TextMeshPro costText;
    [SerializeField] private SpriteRenderer currencySprite;

    [SerializeField] private Color defualtTextColor;
    [SerializeField] private Color cantDrawColor;

    [Header("Draw Costs")]
    [SerializeField] private int drawCost;
    [SerializeField] private int costIncrement;

    [Header("Dependenices")]
    [SerializeField] private DeckBuildingCards deckBuildingCards;
    [SerializeField] private CurrencyCounter currencyCounter;
    [SerializeField] private CardDrawer cardDrawer;

    

    private int cardsDrawnWithCurrency = 0;
    private Vector3 defaultCardPosition;





    private void OnMouseEnter()
    {
        if (!deckBuildingCards.HasCardsLeft()) return;

        DrawUI.SetActive(true);
        TiltCard();
        //Tilt first FakeCard
    }

    private void OnMouseExit()
    {
        if (!deckBuildingCards.HasCardsLeft()) return;

        DrawUI.SetActive(false);
        UntiltCard();
        //Untilt first FakeCard
    }

    private void OnMouseDown()
    {

        if (!deckBuildingCards.HasCardsLeft()) return;
        else if(!currencyCounter.HasEnoughCurrency(drawCost))
        {
            //TODO Play Sound
            CantAffordDraw();
            UntiltCard();
            return;
        }
        //Draw Card
        //Add Random Card to Hand
        //Substract Player's Currency 
        currencyCounter.SubtractCurrency(drawCost);
        cardDrawer.TryDrawCard();
        cardsDrawnWithCurrency++;
        drawCost += costIncrement;
        costText.text = drawCost.ToString();
        UntiltCard();
    }


    // Start is called before the first frame update
    private void Awake()
    {
        DrawUI.SetActive(false);
    }

    IEnumerator Start()
    {
        yield return null;
        costText.text = drawCost.ToString();
        defaultCardPosition = deckBuildingCards.GetTopFakeCard().transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void TiltCard()
    {
        //GameObject topFakeCard 
        //defaultCardPosition.rotation = Quaternion.Lerp()

        //TODO Play Sound
        GameObject topFakeCard = deckBuildingCards.GetTopFakeCard();
        topFakeCard.transform.DOKill();
        Vector3 endPos = new Vector3(-0.2f,0f,-0.07f);
        topFakeCard.transform.DOLocalMove(endPos, 0.1f);
        Vector3 endRotation = new Vector3(0f, -16.15f, 0f);
        topFakeCard.transform.DOLocalRotate(endRotation, 0.1f);

    }

    private void UntiltCard()
    {
        //TODO Play Sound

        GameObject topFakeCard = deckBuildingCards.GetTopFakeCard();
        topFakeCard.transform.DOKill();

        topFakeCard.transform.DOLocalMove(defaultCardPosition, 0.2f);

        topFakeCard.transform.DOLocalRotate(Vector3.zero, 0.2f);
    }

    private void CantAffordDraw()
    {
        //TODO Play Sound

        tittleText.DOKill();
        costText.DOKill();
        currencySprite.DOKill();

        //tittleText.DOPunchPosition(Vector3.zero, 0.5f, 2, 0.25f, false);
        tittleText.color = cantDrawColor;
        tittleText.DOColor(defualtTextColor, 0.5f);

        //costText.DOPunchPosition(Vector3.zero, 0.5f, 2, 0.25f, false);
        costText.color = cantDrawColor;
        costText.DOColor(defualtTextColor, 0.5f);

        /*currencySprite.DOPunch();
        currencySprite.color = cantDrawColor;
        currencySprite.DOColor(defualtTextColor, 0.5f);*/

    }
}
