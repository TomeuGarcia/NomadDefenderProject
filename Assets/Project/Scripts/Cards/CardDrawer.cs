using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using TMPro;
using DG.Tweening;

public class CardDrawer : MonoBehaviour
{
    [SerializeField] private HandBuildingCards hand;
    [SerializeField] protected DeckBuildingCards deck;
    [SerializeField] protected BattleHUD battleHUD;
    [SerializeField] protected GameObject redrawCanvasGameObject; // works for 1 waveSpawner
    [SerializeField] private TextMeshProUGUI redrawingText; // works for 1 waveSpawner
    [SerializeField] private CanvasGroup finishRedrawsButtonCG;

    [SerializeField, Min(1)] private int numCardsHandStart = 2;

    
    [SerializeField] private int cardsToDrawPerWave;

    public delegate void CardDrawerAction();
    public static event CardDrawerAction OnStartSetupBattleCanvases;

    [HideInInspector] public bool cheatDrawCardActivated = true;
    [HideInInspector] public bool canRedraw = true;
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
    }

    protected void GameStartSetup(float startDelay, bool displayRedrawsOnEnd, bool finishRedrawSetup)
    {
        SetupRedraws();
        SetupDeck();

        DrawStartHand(startDelay, displayRedrawsOnEnd, finishRedrawSetup);

        hand.Init();

        deck.GetDeckData().SetStarterCardComponentsAsSaved();

        //HandBuildingCards.OnCardPlayed += StartDrawOverTime;
    }

    private void SetupRedraws()
    {
        redrawingText.text = "Redraws Left: " + hand.GetRedrawsLeft();
        finishRedrawsButtonCG.alpha = 0f;
        finishRedrawsButtonCG.blocksRaycasts = false;
        redrawCanvasGameObject.SetActive(false);
    }
    protected virtual void SetupDeck()
    {
        deck.Init();
        battleHUD.InitDeckCardIcons(deck.NumCards);
    }



    public void TryDrawCard()
    {
        if (deck.HasCardsLeft())
            DrawRandomCard();
    }
    public void TryRedrawCard()
    {
        if (deck.HasCardsLeft() && canRedraw) 
        {
            DrawTopCard();
            
            redrawingText.text = "Redraws Left: " + hand.GetRedrawsLeft();

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
        hand.FinishedRedrawing();
    }
    public void FinishRedrawSetupUI()
    {
        redrawCanvasGameObject.SetActive(false);

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
        AddCardToHand(deck.GetRandomCard());
        //TryHideDeckHUD();
    }

    private void AddCardToHand(BuildingCard card)
    {
        hand.CorrectCardsBeforeAddingCard();
        hand.HintedCardWillBeAdded();
        hand.AddCard(card);

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
            yield return new WaitForSeconds(0.5f);

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
        card.SetCannotBePlayedAnimation();

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
        StartCoroutine(PlayStartRedrawButtonAnimation());
    }
    private IEnumerator PlayStartRedrawButtonAnimation()
    {
        finishRedrawsButtonCG.alpha = 0f;
        finishRedrawsButtonCG.blocksRaycasts = false;

        yield return new WaitForSeconds(0.5f);

        redrawCanvasGameObject.SetActive(true);
        yield return new WaitForSeconds(1f);

        float t = 0.1f;
        finishRedrawsButtonCG.DOFade(1f, t);
        yield return new WaitForSeconds(t);
        finishRedrawsButtonCG.DOFade(0f, t);
        yield return new WaitForSeconds(t);
        finishRedrawsButtonCG.DOFade(1f, t);
        yield return new WaitForSeconds(t);

        finishRedrawsButtonCG.blocksRaycasts = true;
    }


}
