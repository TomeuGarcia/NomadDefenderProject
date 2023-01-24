using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    private readonly float showDuration = 0.2f;
    private readonly float hideDuration = 0.2f;

    [Header("DrawCardWithCurrency")]
    [SerializeField] private DrawCardWithCurrency drawCardWithCurrency;

    [Header("UI elements")]
    [SerializeField] private RectTransform deckUI;
    private float hiddenDeckUIy;
    private float shownDeckUIy;

    [SerializeField] private RectTransform deckText;
    [SerializeField] private Vector3 hiddenTextSize = Vector3.one * 1f;
    [SerializeField] private Vector3 shownTextSize = Vector3.one * 1.5f;

    [SerializeField] private RectTransform cardIconsHolder;
    private float hiddenCardIconsHolderY;
    private float shownCardIconsHolderY;

    [SerializeField] private RectTransform drawCardButton;
    private float hiddenDrawCardButtonX;
    private float shownDrawCardButtonX;

    private bool canClickDrawButton;




    private void Awake()
    {
        hiddenDeckUIy = deckUI.localPosition.y;
        shownDeckUIy = hiddenDeckUIy + 100f;

        hiddenCardIconsHolderY = cardIconsHolder.localPosition.y;
        shownCardIconsHolderY = hiddenCardIconsHolderY + 30f;

        hiddenDrawCardButtonX = drawCardButton.localPosition.x + 200f;
        shownDrawCardButtonX = hiddenDrawCardButtonX - 200f;
        drawCardButton.localPosition = new Vector3(hiddenDrawCardButtonX, drawCardButton.localPosition.y, drawCardButton.localPosition.z);
    }


    public void ShowDeckUI()
    {
        deckUI.DOLocalMoveY(shownDeckUIy, showDuration);
        deckText.DOScale(shownTextSize, showDuration);
        cardIconsHolder.DOLocalMoveY(shownCardIconsHolderY, showDuration)
            .OnComplete( () => { drawCardButton.DOLocalMoveX(shownDrawCardButtonX, hideDuration); canClickDrawButton = true; } );     
    }

    public void HideDeckUI()
    {
        deckUI.DOLocalMoveY(hiddenDeckUIy, hideDuration);
        deckText.DOScale(hiddenTextSize, hideDuration);
        cardIconsHolder.DOLocalMoveY(hiddenCardIconsHolderY, hideDuration);

        drawCardButton.DOLocalMoveX(hiddenDrawCardButtonX, hideDuration);

        canClickDrawButton = false;
    }


    public void DrawCard()
    {
        if (!canClickDrawButton) return;

        Rect drawButtonRect = drawCardButton.rect;
        drawButtonRect.x += drawCardButton.position.x;
        drawButtonRect.y += drawCardButton.position.y;

        if (!drawButtonRect.Contains(Input.mousePosition)) return;


        drawCardWithCurrency.TryDrawCard();
    }

}
