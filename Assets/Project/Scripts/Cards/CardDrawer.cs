using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using TMPro;

public class CardDrawer : MonoBehaviour
{
    [SerializeField] private HandBuildingCards hand;
    [SerializeField] private DeckBuildingCards deck;
    [SerializeField] private BattleHUD battleHUD;
    [SerializeField] private TextMeshProUGUI redrawingText; // works for 1 waveSpawner
    [SerializeField] private GameObject canvas; // works for 1 waveSpawner

    [SerializeField, Min(1)] private int numCardsHandStart = 2;

    [SerializeField] private Image drawCooldownImage;
    
    [SerializeField, Range(10, 60)] private float drawTimeCooldown;
    private float drawCountdown;

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
        redrawingText.text = "Redraws Left: " + hand.GetRedrawsLeft();
        deck.Init();
        battleHUD.InitDeckCardIcons(deck.NumCards);

        DrawStartHand();

        hand.Init();                

        deck.GetDeckData().SetStarterCardComponentsAsSaved();

        drawCountdown = drawTimeCooldown;

        drawCooldownImage.gameObject.SetActive(false);
        //HandBuildingCards.OnCardPlayed += StartDrawOverTime;
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
        canvas.SetActive(false);
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
        if (deck.HasCardsLeft())
        {
            for (int i = 0; i < cardsToDrawPerWave; i++)
            {
                TryDrawCard();
            }

            hand.InitCardsInHand();
        }        
    }


    private void DrawStartHand()
    {
        DrawTopCard();

        for (int i = 1; i < numCardsHandStart; i++)
        {
            TryDrawCard();
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
