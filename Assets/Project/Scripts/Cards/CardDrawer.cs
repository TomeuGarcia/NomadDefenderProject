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
    [SerializeField] private TextMeshProUGUI redrawingText; // works for 1 waveSpawner
    [SerializeField] private CanvasGroup finishRedrawsButtonCG;

    [SerializeField, Min(1)] private int numCardsHandStart = 2;

    
    [SerializeField] private int cardsToDrawPerWave;

    public delegate void CardDrawerAction();
    public static event CardDrawerAction OnStartSetupBattleCanvases;


    private void OnEnable()
    {
        HandBuildingCards.OnQueryDrawCard += TryDrawCardAndUpdateHand;

        HandBuildingCards.OnQueryRedrawCard += TryRedrawCard;
        HandBuildingCards.OnFinishRedrawing += SetupUIBattleCanvases;
        HandBuildingCards.ReturnCardToDeck += ReturnCardToDeck;

        EnemyWaveManager.OnStartNewWaves += DrawCardAfterWave;

    }

    private void OnDisable()
    {
        HandBuildingCards.OnQueryDrawCard -= TryDrawCardAndUpdateHand;

        HandBuildingCards.OnQueryRedrawCard -= TryRedrawCard;
        HandBuildingCards.OnFinishRedrawing -= SetupUIBattleCanvases;
        HandBuildingCards.ReturnCardToDeck -= ReturnCardToDeck;

        EnemyWaveManager.OnStartNewWaves -= DrawCardAfterWave;
    }

    private void Start()
    {
        GameStartSetup();
    }

    protected void GameStartSetup()
    {
        SetupRedraws();
        SetupDeck();

        DrawStartHand();

        hand.Init();

        deck.GetDeckData().SetStarterCardComponentsAsSaved();

        //HandBuildingCards.OnCardPlayed += StartDrawOverTime;
    }

    private void SetupRedraws()
    {
        redrawingText.text = "Redraws Left: " + hand.GetRedrawsLeft();
        StartRedrawButtonAnimation();
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
        if (deck.HasCardsLeft()) 
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
    private void SetupUIBattleCanvases()
    {
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
        TryHideDeckHUD();
    }
    private void DrawRandomCard()
    {
        AddCardToHand(deck.GetRandomCard());
        TryHideDeckHUD();
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
            for (int i = 0; i < cardsToDrawPerWave; i++)
            {
                yield return new WaitForSeconds(0.5f);
                TryDrawCard();
                hand.InitCardsInHand();
            }
        }
    }


    private void DrawStartHand()
    {
        StartCoroutine(DoDrawStartHand());
    }

    private IEnumerator DoDrawStartHand()
    {
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
        
    }


    public void ReturnCardToDeck(BuildingCard card)
    {
        card.SetCannotBePlayedAnimation();

        hand.RemoveCard(card);
        //hand.InitCardsInHand();
        deck.AddCardToDeckBottom(card);

        battleHUD.AddHasDeckCardIcon();
    }

    private void StartRedrawButtonAnimation()
    {
        StartCoroutine(PlayStartRedrawButtonAnimation());
    }
    private IEnumerator PlayStartRedrawButtonAnimation()
    {
        finishRedrawsButtonCG.alpha = 0f;
        finishRedrawsButtonCG.blocksRaycasts = false;

        yield return new WaitForSeconds(3.5f);

        float t = 0.1f;
        finishRedrawsButtonCG.DOFade(1f, t);
        yield return new WaitForSeconds(t);
        finishRedrawsButtonCG.DOFade(0f, t);
        yield return new WaitForSeconds(t);
        finishRedrawsButtonCG.DOFade(1f, t);
        yield return new WaitForSeconds(t);

        finishRedrawsButtonCG.blocksRaycasts = true;
    }


    /*private void StartDrawOverTime()
    {
        HandBuildingCards.OnCardPlayed -= StartDrawOverTime;
        StartCoroutine(DrawOverTime());
    }

    private IEnumerator DrawOverTime()
    {
        drawCooldownImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.2f);

        while (deck.HasCardsLeft())
        {
            while (drawCountdown > 0)
            {
                drawCountdown -= Time.deltaTime;
                drawCountdown = Mathf.Clamp(drawCountdown, 0f, drawTimeCooldown);
                drawCooldownImage.fillAmount = drawCountdown / drawTimeCooldown;

                yield return null;
            }
            drawCountdown = drawTimeCooldown;
            TryDrawCard();
        }
        drawCooldownImage.gameObject.SetActive(false);
    }*/

}
