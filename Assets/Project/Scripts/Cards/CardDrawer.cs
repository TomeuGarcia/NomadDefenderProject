using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using TMPro;
using DG.Tweening;

public class CardDrawer : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private HandBuildingCards hand;
    [SerializeField] protected DeckBuildingCards deck;
    [SerializeField] protected BattleHUD battleHUD;
    [SerializeField] private DeckCreator deckCreator;
    [SerializeField] private TurretCardParts testTurretCardParts;
    [SerializeField] private SpriteRenderer warningBackground;

    private Material warningBackgroundMat;

    [Header("REDRAWS UI")]
    [SerializeField] protected GameObject redrawCanvasGameObject; // works for 1 waveSpawner
    [SerializeField] private TextMeshProUGUI redrawsLeftCounterText;
    [SerializeField] private CanvasGroup cgRedrawsLeftText;
    [SerializeField] private CanvasGroup cgFinishRedrawsButton;
    [SerializeField] private Button finishRedrawsButton;
    [SerializeField] private TextMeshProUGUI finishRedrawsButtonText;
    [SerializeField] private Image[] redrawsLeftWireImages;
    [SerializeField] private Image leftWireImage;
    [SerializeField] private Image rightWireImage;
    [SerializeField] private TextDecoder hintDecoder;
    [SerializeField] private float warningAppearTime;
    [SerializeField] private Ease warningAppearEase;
    [SerializeField] private float warningDisappearTime;
    [SerializeField] private Ease warningDisappearEase;
    [SerializeField, TextArea] private string hintText;
    private static Color fadedInColor = Color.cyan;
    private static Color fadedOutColor = new Color(0.6f, 0.6f, 0.6f);
    private static Color disabledColor = new Color(0.15f, 0.15f, 0.15f);

    [Header("REDRAWS STATS")]
    [SerializeField, Min(1)] private int numCardsHandStart = 2;    
    [SerializeField] private int cardsToDrawPerWave;

    public delegate void CardDrawerAction();
    public static event CardDrawerAction OnStartSetupBattleCanvases;

    [HideInInspector] public bool cheatDrawCardActivated = true;
    [HideInInspector] public bool canRedraw = true;
    [Header("FLAGS")]
    [SerializeField]  public bool canDisplaySpeedUp = true;
    [SerializeField] public bool displayRedrawsOnGameStart = true;
    [SerializeField] public bool finishRedrawSetup = true;


    private void OnEnable()
    {
        HandBuildingCards.OnQueryDrawCard += TryDrawCardAndUpdateHand;

        HandBuildingCards.OnQueryRedrawCard += TryRedrawCard;
        HandBuildingCards.OnFinishRedrawing += FinishRedrawSetupUI;
        HandBuildingCards.ReturnCardToDeck += ReturnCardToDeck;

        EnemyWaveManager.OnStartNewWaves += DrawCardAfterWave;
    }

    private void OnDisable()
    {
        HandBuildingCards.OnQueryDrawCard -= TryDrawCardAndUpdateHand;

        HandBuildingCards.OnQueryRedrawCard -= TryRedrawCard;
        HandBuildingCards.OnFinishRedrawing -= FinishRedrawSetupUI;
        HandBuildingCards.ReturnCardToDeck -= ReturnCardToDeck;

        EnemyWaveManager.OnStartNewWaves -= DrawCardAfterWave;
    }

    private void Start()
    {
        GameStartSetup(2f, displayRedrawsOnGameStart, finishRedrawSetup);

        warningBackgroundMat = warningBackground.material;
    }


    protected void GameStartSetup(float startDelay, bool displayRedrawsOnEnd, bool finishRedrawSetup)
    {
        ServiceLocator.GetInstance().CardDrawer = this;

        SetupRedraws();
        SetupDeck();        

        DrawStartHand(startDelay, displayRedrawsOnEnd, finishRedrawSetup);

        hand.Init();

        deck.GetDeckData().SetStarterCardComponentsAsSaved();

        //HandBuildingCards.OnCardPlayed += StartDrawOverTime;
    }

    private void SetupRedraws()
    {
        UpdateRedrawsLeftText();
        
        cgFinishRedrawsButton.alpha = 0f;
        cgFinishRedrawsButton.blocksRaycasts = false;
        redrawCanvasGameObject.SetActive(false);
    }
    protected virtual void SetupDeck()
    {
        deck.Init();
        battleHUD.InitDeckCardIcons(deck.NumCards);
    }



    public BuildingCard UtilityTryDrawAnyRandomCard(float handShownDuration)
    {
        BuildingCard card = null;
        if (deck.HasCardsLeft())
        {
            card = deck.GetRandomCard();
            AddCardToHand(card, handShownDuration);

            hand.InitCardsInHand();
        }

        return card;
    }
    public BuildingCard UtilityTryDrawRandomCardOfType(BuildingCard.CardBuildingType cardBuildingType, float handShownDuration)
    {
        BuildingCard card = deck.GetRandomCardOfType(cardBuildingType);

        if (card != null)
        {
            AddCardToHand(card, handShownDuration);
            hand.InitCardsInHand();
        }

        return card;
    }
    
    
    
    private void TryDrawCard()
    {
        if (deck.HasCardsLeft())
            DrawRandomCard();
    }
    public void TryRedrawCard()
    {
        if (deck.HasCardsLeft() && canRedraw) 
        {
            DrawTopCard();

            UpdateRedrawsLeftText();
            
            if (hand.HasRedrawsLeft())
            {
                hand.InitCardsInHandForRedraw();
            }
            else
            {
                hand.InitCardsInHand();
            }
        }
    }
    public void OnFinishRedrawsButtonPressed()
    {
        FinishRedraws();
        GameAudioManager.GetInstance().PlayCardInfoMoveShown();
        finishRedrawsButton.transform.DOPunchScale(Vector3.one * -0.2f, 0.4f, 8);
    }
    public void FinishRedraws()
    {
        hand.FinishedRedrawing();
        cgFinishRedrawsButton.interactable = false;
    }
    public void FinishRedrawSetupUI()
    {
        PlayEndRedrawHUDAnimation();

        battleHUD.PlayDeckUIHideStartGameAnimation(0.5f); // DECK UI ANIM - PART 2        
        if (canDisplaySpeedUp) battleHUD.PlaySpeedUpUIStartGameAnimation();

        if (OnStartSetupBattleCanvases != null) OnStartSetupBattleCanvases();
    }
    public void TryDrawCardAndUpdateHand()
    {
        if (deck.HasCardsLeft()) 
        {
            DrawRandomCard();
            hand.InitCardsInHand();
        }
    }

    private void DrawTopCard()
    {
        AddCardToHand(deck.GetTopCard());
        //TryHideDeckHUD();        
    }
    private void DrawRandomCard()
    {
        BuildingCard card = deck.GetRandomCard();
        AddCardToHand(card);
        //TryHideDeckHUD();
    }

    private void AddCardToHand(BuildingCard card, float handShownDuration = 0.0f)
    {
        hand.CorrectCardsBeforeAddingCard();
        hand.HintedCardWillBeAdded();
        hand.AddCard(card, handShownDuration);

        card.StartDisableInfoDisplayForDuration(1.5f);

        battleHUD.SubtractHasDeckCardIcon();
    }

    private void TryHideDeckHUD()
    {
        if (!deck.HasCardsLeft())
        {
            if (!battleHUD.drewCardViaHUD)
            {
                battleHUD.HideDeckUI();
            }
        }
    }



    private void DrawCardAfterWave()
    {
        StartCoroutine(DoDrawCardAfterWave());   
    }

    private IEnumerator DoDrawCardAfterWave()
    {        
        if (deck.HasCardsLeft())
        {
            battleHUD.canInteractWithDeckUI = false;
            battleHUD.canShowDrawCardButton = false;
            battleHUD.ShowDeckUI();
            yield return new WaitForSeconds(1.0f);

            int numCardsDrawn = 0;
            while (numCardsDrawn < cardsToDrawPerWave && deck.HasCardsLeft())
            {
                yield return new WaitForSeconds(0.5f);
                DrawRandomCard();
                hand.InitCardsInHand();
                ++numCardsDrawn;
            }

            if (numCardsDrawn < cardsToDrawPerWave)
            {
                battleHUD.PlayDeckNoCardsLeftAnimation(2);
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
                battleHUD.HideDeckUI();
                yield return new WaitForSeconds(0.5f);
                battleHUD.canInteractWithDeckUI = true;
                battleHUD.canShowDrawCardButton = true;
            }
        }
        else
        {
            battleHUD.PlayDeckNoCardsLeftAnimation(4);
        }
    }

    public void ReturnCardToDeck(BuildingCard card)
    {
        card.SetCannotBePlayedAnimation(true);

        hand.RemoveCard(card);
        //hand.InitCardsInHand();
        deck.AddCardToDeckBottom(card);

        battleHUD.AddHasDeckCardIcon();
    }


    private void DrawStartHand(float startDelay, bool displayRedrawsOnEnd, bool finishRedrawSetup)
    {
        StartCoroutine(DoDrawStartHand(startDelay, displayRedrawsOnEnd, finishRedrawSetup));
    }

    private IEnumerator DoDrawStartHand(float startDelay, bool displayRedrawsOnEnd, bool finishRedrawSetup)
    {
        yield return new WaitForSeconds(startDelay);

        battleHUD.PlayDeckUIShowStartGameAnimation(); // DECK UI ANIM - PART 1  
        yield return new WaitForSeconds(1f);

        DrawTopCard();

        hand.InitCardsInHandForRedraw();


        if (deck.HasCardsLeft())
        {
            for (int i = 1; i < numCardsHandStart; i++)
            {
                yield return new WaitForSeconds(0.5f);
                TryDrawCard();
                hand.InitCardsInHandForRedraw();
            }
        }

        if (displayRedrawsOnEnd)
        {
            StartRedrawButtonAnimation();
        }
        else
        {
            if (finishRedrawSetup)
            {
                Debug.Log("finish redraw xd");
                FinishRedrawSetupUI();
            }            
        }
    }

    public void StartRedrawButtonAnimation()
    {
        PlayStartRedrawHUDAnimation();
    }

    private void PlayStartRedrawHUDAnimation()
    {
        // Setup
        redrawCanvasGameObject.SetActive(true);

        cgFinishRedrawsButton.alpha = 0f;
        cgFinishRedrawsButton.blocksRaycasts = false;
        cgFinishRedrawsButton.gameObject.SetActive(false);

        cgRedrawsLeftText.alpha = 0f;
        leftWireImage.fillAmount = 0f;
        rightWireImage.fillAmount = 0f;
        for (int i = 0; i < redrawsLeftWireImages.Length; ++i)
        {
            redrawsLeftWireImages[i].fillAmount = 0f;
        }

        float t3 = 0.3f;
        float t2 = 0.2f;
        float t1 = 0.1f;

        Sequence appearSequence = DOTween.Sequence();
        appearSequence.Append(leftWireImage.DOFillAmount(1.0f, t3));
        appearSequence.Join(cgRedrawsLeftText.DOFade(1f, t1));
        appearSequence.Join(cgFinishRedrawsButton.DOFade(1.0f, t2));
        appearSequence.Join(rightWireImage.DOFillAmount(1.0f, t3));
        
        for (int i = 0; i < redrawsLeftWireImages.Length; ++i)
        {
            appearSequence.Append(redrawsLeftWireImages[i].DOFillAmount(1.0f, t1));
        }



                
        appearSequence.AppendCallback(() => cgFinishRedrawsButton.gameObject.SetActive(true) );
        appearSequence.AppendInterval(t1);
        appearSequence.AppendCallback(() => cgFinishRedrawsButton.gameObject.SetActive(false));
        appearSequence.AppendInterval(t1);
        appearSequence.AppendCallback(() => cgFinishRedrawsButton.gameObject.SetActive(true));
        appearSequence.AppendInterval(t1);
        appearSequence.AppendCallback(() => cgFinishRedrawsButton.gameObject.SetActive(false));
        appearSequence.AppendInterval(t1);
        appearSequence.AppendCallback(() => cgFinishRedrawsButton.gameObject.SetActive(true));
        appearSequence.AppendInterval(t1);

        appearSequence.AppendCallback(() => cgFinishRedrawsButton.blocksRaycasts = true);
        appearSequence.AppendCallback(() => ButtonFadeIn(finishRedrawsButton, finishRedrawsButtonText, true) );

        warningBackground.gameObject.SetActive(true);
        warningBackgroundMat.DOFloat(1.0f, MaterialProperties.Activate, warningAppearTime).SetEase(warningAppearEase);

        hintDecoder.SetTextStrings(hintText);
        hintDecoder.Activate();


    }

    private void PlayEndRedrawHUDAnimation()
    {
        float t1 = 0.1f;
        float t3 = 0.3f;

        Sequence disappearSequence = DOTween.Sequence();
        disappearSequence.AppendInterval(0.3f);


        int redrawsLeft = hand.GetRedrawsLeft();
        for (int i = redrawsLeftWireImages.Length-1; i >= 0; --i)
        {
            if (redrawsLeftWireImages[i].fillAmount > 0.5f)
            {
                disappearSequence.Append(redrawsLeftWireImages[i].DOFillAmount(0f, t1));                
            }            
        }

        disappearSequence.Append(cgFinishRedrawsButton.DOFade(0f, t1));

        disappearSequence.Append(leftWireImage.DOFillAmount(0f, t3));
        disappearSequence.Join(cgRedrawsLeftText.DOFade(0f, t1));
        disappearSequence.Join(rightWireImage.DOFillAmount(0f, t3));

        disappearSequence.AppendCallback( () => redrawCanvasGameObject.SetActive(false) );

        warningBackgroundMat.DOFloat(0.0f, MaterialProperties.Activate, warningDisappearTime).SetEase(warningDisappearEase)
            .OnComplete(()=>warningBackground.gameObject.SetActive(false));

        hintDecoder.ClearDecoder();
    }


    private void UpdateRedrawsLeftText()
    {
        int redrawsLeft = hand.GetRedrawsLeft();
        redrawsLeftCounterText.text = redrawsLeft.ToString();
        redrawsLeftCounterText.rectTransform.DOPunchScale(Vector3.one * -0.8f, 0.7f, 7);
        redrawsLeftCounterText.DOBlendableColor(Color.cyan, 0.35f).OnComplete(() => redrawsLeftCounterText.DOBlendableColor(Color.white, 0.35f));


        if (redrawsLeft > 0 && redrawsLeftWireImages.Length >= redrawsLeft)
        {
            redrawsLeftWireImages[redrawsLeft - 1].DOFillAmount(0f, 0.25f);
        }
        
    }

    private void ButtonFadeIn(Button button, TextMeshProUGUI buttonText, bool onEndFadeOut = true)
    {
        if (!button.interactable) { return; }

        button.transform.DOScale(1.1f, 1.0f).OnComplete(() => { if (onEndFadeOut) ButtonFadeOut(button, buttonText); });
        button.image.DOBlendableColor(fadedInColor, 1.0f);
        buttonText.DOBlendableColor(fadedInColor, 1.0f);
    }

    private void ButtonFadeOut(Button button, TextMeshProUGUI buttonText, bool onEndFadeIn = true)
    {
        button.transform.DOScale(1.0f, 1.0f).OnComplete(() => { if (onEndFadeIn) ButtonFadeIn(button, buttonText); });
        button.image.DOBlendableColor(fadedOutColor, 1.0f);
        buttonText.DOBlendableColor(fadedOutColor, 1.0f);
    }

    private void StopButtonFade(Button button, TextMeshProUGUI buttonText, bool goToFadedOut)
    {
        button.transform.DOKill();
        button.image.DOKill();
        buttonText.DOKill();        

        if (goToFadedOut && button.interactable)
        {
            ButtonFadeOut(button, buttonText, false);
        }
    }

    public void FinishRedrawsButtonHovered()
    {
        StopButtonFade(finishRedrawsButton, finishRedrawsButtonText, false);
        finishRedrawsButton.image.DOBlendableColor(Color.cyan, 0.1f);
        finishRedrawsButtonText.DOBlendableColor(Color.cyan, 0.1f);
        finishRedrawsButtonText.rectTransform.DOScale(Vector3.one * 1.1f, 0.1f);
        GameAudioManager.GetInstance().PlayCardInfoShown();
    }
    public void FinishRedrawsButtonUnhovered()
    {
        finishRedrawsButton.image.color = fadedInColor;
        finishRedrawsButtonText.color = fadedInColor;
        finishRedrawsButtonText.rectTransform.DOScale(Vector3.one, 0.1f);

        ButtonFadeOut(finishRedrawsButton, finishRedrawsButtonText, true);
        GameAudioManager.GetInstance().PlayCardInfoHidden();
    }


    public TurretBuildingCard SpawnTurretCardInDeck(TurretCardParts turretCardParts)
    {
        TurretBuildingCard turretCard = deckCreator.GetUninitializedNewTurretCard();
        turretCard.ResetParts(turretCardParts);

        deck.AddCardToDeckBottom(turretCard);

        battleHUD.AddNewDeckCardIconsAndShow(1);

        return turretCard;
    }
    public TurretBuildingCard SpawnTurretCardInHand(TurretCardParts turretCardParts)
    {
        TurretBuildingCard turretCard = deckCreator.GetUninitializedNewTurretCard();
        turretCard.ResetParts(turretCardParts);

        deck.AddCardToDeckTop(turretCard);
        DrawTopCard();
        hand.InitCardsInHand();

        battleHUD.AddNewDeckCardIconsAndShow(1);

        return turretCard;
    }

    public BuildingCard[] GetCardsInHand()
    {
        return hand.GetCards().ToArray();
    }

    public void CheckHandCardsCost()
    {
        hand.CheckCardsCost();
    }

}
