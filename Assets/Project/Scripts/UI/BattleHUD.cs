using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class BattleHUD : MonoBehaviour
{
    private readonly float showDuration = 0.2f;
    private readonly float hideDuration = 0.2f;

    [Header("CURRENCY")]
    [Header("Currency UI")]
    [SerializeField] private GameObject currencyUI;
    [SerializeField] private CanvasGroup cgCurrencyUI;

    [Header("SPEED UP")]
    [Header("Speed Up Button")]
    [SerializeField] private GameObject speedUpButtonHolder;
    [SerializeField] private CanvasGroup cgSpeedUpUI;

    [Header("DECK")]
    [Header("Dependenices")]
    [SerializeField] private DeckBuildingCards deckBuildingCards;
    [SerializeField] private CurrencyCounter currencyCounter;
    [SerializeField] private CardDrawer cardDrawer;

    [Header("Draw Costs")]
    [SerializeField] private int drawCost;
    [SerializeField] private int costIncrement;

    [Header("Deck UI")]
    [HideInInspector] public bool canInteractWithDeckUI = false;
    [SerializeField] private CanvasGroup cgDeckUI;
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

    [HideInInspector] public bool showSpeedUp;
    [HideInInspector] public bool canShowDrawCardButton;

    private void OnEnable()
    {
        CardDrawer.OnStartSetupBattleCanvases += PlayStartGameAnimation;
    }
    private void OnDisable()
    {
        CardDrawer.OnStartSetupBattleCanvases -= PlayStartGameAnimation;
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

        canInteractWithDeckUI = false;
        HideUI();

        StartCoroutine(CurrencyUIStartGameAnimation(true));

        showSpeedUp = true;
        canShowDrawCardButton = true;
    }

    private void Update()
    {
        if (canClickDrawButton)
        {
            OnDrawButtonEnabled();
        }
    }

    private void ShowUI()
    {
        if(showSpeedUp)
            speedUpButtonHolder.SetActive(true);

        deckUI.gameObject.SetActive(true);
    }

    private void HideUI()
    {
        speedUpButtonHolder.SetActive(false);
        deckUI.gameObject.SetActive(false);
    }

    private void PlayStartGameAnimation()
    {
        ShowUI();
        StartCoroutine(DeckUIStartGameAnimation());   
        if(showSpeedUp)
            StartCoroutine(SpeedUpUIStartGameAnimation());   
    }


    public void DeactivateCurrencyUI()
    {
        currencyUI.SetActive(false);
    }
    public void ActivateCurrencyUI()
    {
        currencyUI.SetActive(true);
        StartCoroutine(CurrencyUIStartGameAnimation(false));
    }

    private IEnumerator CurrencyUIStartGameAnimation(bool startWithDelay)
    {
        // Start wait
        cgCurrencyUI.alpha = 0f;
        if (startWithDelay) yield return new WaitForSeconds(1.0f);

        // Appear
        float t1 = 0.1f;
        cgCurrencyUI.DOFade(1f, t1);
        GameAudioManager.GetInstance().PlayCardInfoShown();
        yield return new WaitForSeconds(t1);

        cgCurrencyUI.DOFade(0f, t1);
        GameAudioManager.GetInstance().PlayCardInfoShown();
        yield return new WaitForSeconds(t1);

        cgCurrencyUI.DOFade(1f, t1);        
    }
    private IEnumerator SpeedUpUIStartGameAnimation()
    {
        // Start wait
        cgSpeedUpUI.alpha = 0f;
        yield return new WaitForSeconds(1.0f);

        // Appear
        cgSpeedUpUI.DOFade(1f, 1f);
    }

    public void DisableSpeedUpUI()
    {
        showSpeedUp = false;
        speedUpButtonHolder.SetActive(false);
    }

    public void EnableSpeedUpUI()
    {
        showSpeedUp = true;
        speedUpButtonHolder.SetActive(true);
    }
    private IEnumerator DeckUIStartGameAnimation()
    {
        canInteractWithDeckUI = false;


        // Start wait
        drawButtonHolder.gameObject.SetActive(false);
        cgDeckUI.alpha = 0f;
        yield return new WaitForSeconds(2.0f);


        // Appear
        cgDeckUI.DOFade(1f, 1.0f);

        foreach (Image icon in cardIcons)
        {
            icon.DOFade(0f, 0.1f); // kinda scuffed but make all icons invisible before showing
        }

        yield return new WaitForSeconds(1.0f);


        // Show up
        ShowDeckUI();
        yield return new WaitForSeconds(0.4f);

        float pitch = 0.9f;
        float pitchIncrement = 0.05f;
        foreach (Image icon in cardIcons)
        {
            icon.DOFade(1f, 0.25f);
            GameAudioManager.GetInstance().PlayCardUIInfoShown(pitch);
            pitch += pitchIncrement;
            yield return new WaitForSeconds(0.25f);
        }
        yield return new WaitForSeconds(0.25f);


        // Hide back
        HideDeckUI();
        yield return new WaitForSeconds(1.0f);


        // Ready
        drawButtonHolder.gameObject.SetActive(true);
        canInteractWithDeckUI = true;
    }


    public void OnHoverEnter() // called by UI
    {
        if (!canInteractWithDeckUI) return;
        ShowDeckUI();
    }
    public void OnHoverExit() // called by UI
    {
        if (!canInteractWithDeckUI) return;
        HideDeckUI();
    }
    public void OnClick() // called by UI
    {
        if (!canInteractWithDeckUI) return;
        CheckDrawButtonClicked();
    }


    public void ShowDeckUI()
    {
        GameAudioManager.GetInstance().PlayCardHovered();

        shownDeckUIy = ComputeShownDeckUIy();

        deckUI.DOLocalMoveY(shownDeckUIy, showDuration);
        deckText.DOScale(shownTextSize, showDuration);


        cardIconsHolder.DOLocalMoveY(shownCardIconsHolderY, showDuration)
            .OnComplete(() => {
                if (deckBuildingCards.HasCardsLeft() && canShowDrawCardButton)
                {
                    drawButtonHolder.DOLocalMoveX(shownDrawButtonX, hideDuration)
                        .OnComplete(() => { EnableClickDrawButton(); });
                }

            });
    }

    public void HideDeckUI() // called on hover exit
    {
        GameAudioManager.GetInstance().PlayCardHoverExit();

        deckUI.DOLocalMoveY(hiddenDeckUIy, hideDuration);
        deckText.DOScale(hiddenTextSize, hideDuration);

        cardIconsHolder.DOComplete(false);
        cardIconsHolder.DOLocalMoveY(hiddenCardIconsHolderY, hideDuration);

        drawButtonHolder.DOComplete(false);
        drawButtonHolder.DOLocalMoveX(hiddenDrawButtonX, hideDuration);
        
        OnDrawButtonStopHover();

        DisableClickDrawButton();
    }


    public void CheckDrawButtonClicked() // called on click
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



    public void InitDeckCardIcons(int amount)
    {
        cardIcons = new List<Image>(amount);
        for (int i = 0; i< amount; ++i)
        {
            AddNewDeckCardIcon();
        }

        lastSpriteHasCardIndex = amount - 1;
    }
    public void AddNewDeckCardIcon()
    {
        Image newCardIcon = Instantiate(referenceCardIconImage);
        newCardIcon.gameObject.SetActive(true);

        cardIcons.Add(newCardIcon);
        newCardIcon.rectTransform.SetParent(cardIconsHolder);
        newCardIcon.sprite = spriteHasCard;
    }

    public void SubtractHasDeckCardIcon()
    {
        if (lastSpriteHasCardIndex < 0) return;

        cardIcons[lastSpriteHasCardIndex].sprite = spriteNoCard;
        --lastSpriteHasCardIndex;
    }
    public void AddHasDeckCardIcon()
    {
        ++lastSpriteHasCardIndex;

        if (lastSpriteHasCardIndex >= cardIcons.Count)
        {
            AddNewDeckCardIcon();
        }

        cardIcons[lastSpriteHasCardIndex].sprite = spriteHasCard;
    }

    private float ComputeShownDeckUIy()
    {
        float yPerLevel = 50f;
        return hiddenDeckUIy + (yPerLevel * (((cardIcons.Count-1) / 5) +1));
    }
}
