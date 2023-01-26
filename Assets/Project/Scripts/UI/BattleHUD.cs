using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Rendering.LookDev;
using System.Collections.Generic;

public class BattleHUD : MonoBehaviour
{
    private readonly float showDuration = 0.2f;
    private readonly float hideDuration = 0.2f;

    [Header("SPEED UP")]
    [Header("Speed Up Button")]
    [SerializeField] private GameObject speedUpButtonHolder;

    [Header("DECK")]
    [Header("Dependenices")]
    [SerializeField] private DeckBuildingCards deckBuildingCards;
    [SerializeField] private CurrencyCounter currencyCounter;
    [SerializeField] private CardDrawer cardDrawer;

    [Header("Draw Costs")]
    [SerializeField] private int drawCost;
    [SerializeField] private int costIncrement;

    [Header("Deck UI")]
    [SerializeField] private RectTransform deckUI;
    private float hiddenDeckUIy;
    private float shownDeckUIy;

    [Header("Deck text")]
    [SerializeField] private RectTransform deckText;
    [SerializeField] private Vector3 hiddenTextSize = Vector3.one * 1f;
    [SerializeField] private Vector3 shownTextSize = Vector3.one * 1.5f;

    [Header("Card Icons")]
    [SerializeField] private RectTransform cardIconsHolder;
    private float hiddenCardIconsHolderY;
    private float shownCardIconsHolderY;
    [SerializeField] private Image referenceCardIconImage;
    [SerializeField] private Sprite spriteHasCard;
    [SerializeField] private Sprite spriteNoCard;
    private List<Image> cardIcons;
    private int lastSpriteHasCardIndex;

    [Header("Draw Button Holder")]
    [SerializeField] private RectTransform drawButtonHolder;
    
    [Header("Draw Button Image")]
    [SerializeField] private Image drawButtonImageImage;
    private RectTransform drawButtonImage;

    [Header("Draw Button Cost Text")]
    [SerializeField] private TextMeshProUGUI costTextText;
    private RectTransform costText;
    [SerializeField] private Image costTextImage;
    private float hiddenDrawButtonX;
    private float shownDrawButtonX;

    [HideInInspector] public bool drewCardViaHUD = false; // scuffed flag boolean used to hide UI when last card is drawn not manually

    private bool canClickDrawButton = false;
    private bool isHoveringDrawButton = false;

    [Header("Draw card animations")]
    [SerializeField] private Color canDrawCardColor = Color.cyan;
    [SerializeField] private Color canNotDrawCardColor = Color.red;
    [SerializeField, Min(0.1f)] private float blinkDuration = 0.2f;

    private bool isHiding = false;
    private bool isShowing = false;

    private void OnEnable()
    {
        CardDrawer.OnStartSetupBattleCanvases += EnableUI;
    }
    private void OnDisable()
    {
        CardDrawer.OnStartSetupBattleCanvases -= EnableUI;
    }

    private void Awake()
    {
        hiddenDeckUIy = deckUI.localPosition.y;

        hiddenCardIconsHolderY = cardIconsHolder.localPosition.y;
        shownCardIconsHolderY = hiddenCardIconsHolderY + 30f;

        hiddenDrawButtonX = drawButtonHolder.localPosition.x + 200f;
        shownDrawButtonX = hiddenDrawButtonX - 200f;
        drawButtonHolder.localPosition = new Vector3(hiddenDrawButtonX, drawButtonHolder.localPosition.y, drawButtonHolder.localPosition.z);

        drawButtonImage = drawButtonImageImage.rectTransform;

        costText = costTextText.rectTransform;
        costText.gameObject.SetActive(false);

        UpdateDrawCostText(drawCost);

        DisableUI();
    }

    private void Update()
    {
        if (canClickDrawButton)
        {
            OnDrawButtonEnabled();
        }
    }

    private void EnableUI()
    {
        speedUpButtonHolder.SetActive(true);
        deckUI.gameObject.SetActive(true);
    }

    private void DisableUI()
    {
        speedUpButtonHolder.SetActive(false);
        deckUI.gameObject.SetActive(false);
    }

    public void ShowDeckUI() // called on hover enter
    {
        if (isHiding) return;

        isShowing = true;

        GameAudioManager.GetInstance().PlayCardHovered();

        shownDeckUIy = ComputeShownDeckUIy();

        deckUI.DOLocalMoveY(shownDeckUIy, showDuration);
        deckText.DOScale(shownTextSize, showDuration);


        cardIconsHolder.DOLocalMoveY(shownCardIconsHolderY, showDuration)
            .OnComplete(() => {
                if (deckBuildingCards.HasCardsLeft())
                {
                    drawButtonHolder.DOLocalMoveX(shownDrawButtonX, hideDuration)
                        .OnComplete(() => { EnableClickDrawButton(); isShowing = false; });
                }

            });
    }

    public void HideDeckUI() // called on hover exit
    {
        if (isShowing) return;

        isHiding = true;

        GameAudioManager.GetInstance().PlayCardHoverExit();

        deckUI.DOLocalMoveY(hiddenDeckUIy, hideDuration);
        deckText.DOScale(hiddenTextSize, hideDuration);
        cardIconsHolder.DOLocalMoveY(hiddenCardIconsHolderY, hideDuration);

        drawButtonHolder.DOLocalMoveX(hiddenDrawButtonX, hideDuration)
            .OnComplete(() => isHiding = false);
        
        OnDrawButtonStopHover();

        DisableClickDrawButton();
    }


    public void OnDrawButtonClicked() // called on click
    {
        if (!canClickDrawButton)
        {
            return;
        }

        if (!IsMouseHoveringDrawButton())
        {
            return;
        }

        CheckDrawCard();  
    }


    private bool IsMouseHoveringDrawButton()
    {
        Rect drawButtonRect = drawButtonHolder.rect;
        drawButtonRect.x += drawButtonHolder.position.x;
        drawButtonRect.y += drawButtonHolder.position.y;

        return drawButtonRect.Contains(Input.mousePosition);
    }

    private void EnableClickDrawButton()
    {
        canClickDrawButton = true;
        isHoveringDrawButton = false;
    }

    private void DisableClickDrawButton()
    {
        canClickDrawButton = false;
        isHoveringDrawButton = false; // just in case
    }


    private void OnDrawButtonEnabled()
    {
        if (IsHoverEnterDrawButton())
        {
            isHoveringDrawButton = true;
            OnDrawButtonStartHover();
        }
        else if (IsHoverExitDrawButton())
        {
            isHoveringDrawButton = false;
            OnDrawButtonStopHover();
        }
    }

    private bool IsHoverEnterDrawButton()
    {
        return !isHoveringDrawButton && IsMouseHoveringDrawButton();
    }
    private bool IsHoverExitDrawButton()
    { 
        return isHoveringDrawButton && !IsMouseHoveringDrawButton();
    }

    private void OnDrawButtonStartHover()
    {
        drawButtonImage.DORotate(Vector3.forward * 15, 0.2f);
        ShowCostText();

        GameAudioManager.GetInstance().PlayCardHovered();
    }
    private void OnDrawButtonStopHover()
    {
        drawButtonImage.DORotate(Vector3.forward * 0, 0.2f);
        HideCostText();

        GameAudioManager.GetInstance().PlayCardHoverExit();
    }

    private void ShowCostText()
    {
        costText.gameObject.SetActive(true);
        costText.DOPunchPosition(Vector3.up*10f, 0.6f, 8);
    }
    private void HideCostText()
    {
        costTextText.DOComplete(false);
        costText.DOComplete(false);

        costText.gameObject.SetActive(false);
    }


    private void CheckDrawCard()
    {
        canClickDrawButton = false;

        if (currencyCounter.HasEnoughCurrency(drawCost))
        {
            DoCanDrawCard();
        }
        else
        {
            DoCanNotDrawCard();
        }

    }

    private void DoCanDrawCard()
    {
        drewCardViaHUD = true;

        currencyCounter.SubtractCurrency(drawCost);
        cardDrawer.TryDrawCardAndUpdateHand();
        drawCost += costIncrement;

        GameAudioManager.GetInstance().PlayCurrencySpent();

        drawButtonImageImage.DOBlendableColor(canDrawCardColor, blinkDuration)
        .OnComplete(() => drawButtonImageImage.DOBlendableColor(Color.white, blinkDuration)
            .OnComplete(() => drawButtonImage.DOBlendableMoveBy(Vector3.zero, 0.2f) // do nothing, wait extra time before hiding
                .OnComplete(() => { HideDeckUI(); UpdateDrawCostText(drawCost); drewCardViaHUD = false; } )));

        costText.DOPunchPosition(Vector3.up * 20f, blinkDuration * 2f);
        costTextText.DOBlendableColor(canDrawCardColor, blinkDuration)
            .OnComplete(() => costTextText.DOBlendableColor(Color.white, blinkDuration));
        costTextImage.DOBlendableColor(canDrawCardColor, blinkDuration)
            .OnComplete(() => costTextImage.DOBlendableColor(Color.white, blinkDuration));
    }

    private void DoCanNotDrawCard()
    {
        GameAudioManager.GetInstance().PlayCardHoverExit();        

        drawButtonImageImage.DOBlendableColor(canNotDrawCardColor, blinkDuration)
            .OnComplete(() => drawButtonImageImage.DOBlendableColor(Color.white, blinkDuration)
                .OnComplete(() => drawButtonImage.DOBlendableMoveBy(Vector3.zero, 0.5f) // do nothing, wait extra time before reenabling click
                    .OnComplete(() => canClickDrawButton = true)));

        costText.DOPunchPosition(Vector3.left * 20f, blinkDuration * 2f);
        costTextText.DOBlendableColor(canNotDrawCardColor, blinkDuration)
            .OnComplete(() => costTextText.DOBlendableColor(Color.white, blinkDuration));
        costTextImage.DOBlendableColor(canNotDrawCardColor, blinkDuration)
            .OnComplete(() => costTextImage.DOBlendableColor(Color.white, blinkDuration));

    }


    private void UpdateDrawCostText(int costAmount)
    {
        costTextText.text = "-" + costAmount.ToString();
    }



    public void InitCardIcons(int amount)
    {
        cardIcons = new List<Image>(amount);
        for (int i = 0; i< amount; ++i)
        {
            AddNewCardIcon();
        }

        lastSpriteHasCardIndex = amount - 1;
    }
    public void AddNewCardIcon()
    {
        Image newCardIcon = Instantiate(referenceCardIconImage);
        newCardIcon.gameObject.SetActive(true);

        cardIcons.Add(newCardIcon);
        newCardIcon.rectTransform.SetParent(cardIconsHolder);
        newCardIcon.sprite = spriteHasCard;
    }

    public void SubtractHasCardIcon()
    {
        if (lastSpriteHasCardIndex < 0) return;

        cardIcons[lastSpriteHasCardIndex].sprite = spriteNoCard;
        --lastSpriteHasCardIndex;
    }
    public void AddHasCardIcon()
    {
        ++lastSpriteHasCardIndex;

        if (lastSpriteHasCardIndex >= cardIcons.Count)
        {
            AddNewCardIcon();
        }

        cardIcons[lastSpriteHasCardIndex].sprite = spriteHasCard;
    }

    private float ComputeShownDeckUIy()
    {
        return hiddenDeckUIy + (50f * ((cardIcons.Count / 5) +1));
    }
}
