using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class DrawCardWithCurrency : MonoBehaviour
{

    [Header("UI")] 
    [SerializeField] private GameObject DrawUI;
    [SerializeField] private TextMeshPro tittleText;
    [SerializeField] private TextMeshPro costText;
    [SerializeField] private Image currencySprite;

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



    private void Awake()
    {
        DrawUI.SetActive(false);
    }

    IEnumerator Start()
    {
        yield return null;
        costText.text = drawCost.ToString();
        defaultCardPosition = deckBuildingCards.GetTopCardTransform().localPosition;
    }

    private void OnMouseEnter()
    {
        if (!deckBuildingCards.HasCardsLeft()) return;

        DrawUI.SetActive(true);
        TiltCard();
        GameAudioManager.GetInstance().PlayCardHovered();
    }

    private void OnMouseExit()
    {
        if (!deckBuildingCards.HasCardsLeft()) return;

        DrawUI.SetActive(false);
        UntiltCard();
        GameAudioManager.GetInstance().PlayCardHoverExit();
    }

    private void OnMouseDown()
    {
        //TryDrawCard();
    }



    public void TryDrawCard()
    {
        if (!deckBuildingCards.HasCardsLeft()) return;
        else if (!currencyCounter.HasEnoughCurrency(drawCost))
        {
            //TODO Play Sound
            CantAffordDraw();
            UntiltCard();
            GameAudioManager.GetInstance().PlayCardHoverExit();
            return;
        }
        //Draw Card
        //Add Random Card to Hand
        //Substract Player's Currency 
        currencyCounter.SubtractCurrency(drawCost);
        cardDrawer.TryDrawCardAndUpdateHand();
        cardsDrawnWithCurrency++;
        drawCost += costIncrement;
        costText.text = drawCost.ToString();
        UntiltCard();
        GameAudioManager.GetInstance().PlayCurrencySpent();
    }




    private void TiltCard()
    {
        //GameObject topFakeCard 
        //defaultCardPosition.rotation = Quaternion.Lerp()

        //TODO Play Sound
        Transform cardTransform = deckBuildingCards.GetTopCardTransform();
        cardTransform.DOKill();
        Vector3 endPos = new Vector3(0.2f, 0.1f, 0.0f);
        cardTransform.DOLocalMove(endPos, 0.1f);
        Vector3 endRotation = new Vector3(0f, 0.0f, -8.0f);
        cardTransform.DOLocalRotate(endRotation, 0.1f);

    }

    private void UntiltCard()
    {
        //TODO Play Sound

        Transform cardTransform = deckBuildingCards.GetTopCardTransform();
        cardTransform.transform.DOKill();

        cardTransform.transform.DOLocalMove(defaultCardPosition, 0.2f);

        cardTransform.transform.DOLocalRotate(Vector3.zero, 0.2f);
    }

    private void CantAffordDraw()
    {
        //TODO Play Sound

        //tittleText.DOKill();
        costText.DOKill();
        currencySprite.DOKill();

        //tittleText.transform.DOPunchPosition(Vector3.forward * 0.1f, 0.25f, 1, 0.25f, false);
        //tittleText.color = cantDrawColor;
        //tittleText.DOColor(defualtTextColor, 0.5f);

        costText.transform.DOPunchPosition(Vector3.forward * 0.11f, 0.25f, 1, 0.25f, false);
        costText.color = cantDrawColor;
        costText.DOColor(defualtTextColor, 0.5f);

        //currencySprite.DOPunch();
        currencySprite.color = cantDrawColor;
        currencySprite.DOColor(defualtTextColor, 0.5f);

    }
}
