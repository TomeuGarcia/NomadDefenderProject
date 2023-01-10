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
    [SerializeField] private TextMeshProUGUI redrawingText; // works for 1 waveSpawner
    [SerializeField] private GameObject canvas; // works for 1 waveSpawner

    [SerializeField, Min(1)] private int numCardsHandStart = 2;

    [SerializeField] private Image drawCooldownImage;
    
    [SerializeField, Range(10, 60)] private float drawTimeCooldown;
    private float drawCountdown;

    [SerializeField] private int cardsToDrawPerWave;

    public delegate void CardDrawerAction();
    public static event CardDrawerAction activateWaveCanvas;


    private void OnEnable()
    {
        HandBuildingCards.OnQueryDrawCard += TryDrawCardAndUpdateHand;

        HandBuildingCards.OnQueryRedrawCard += TryRedrawCard;
        HandBuildingCards.OnFinishRedrawing += DeactivateRedrawCanvas;
        HandBuildingCards.ReturnCardToDeck += ReturnCardToDeck;

        EnemyWaveManager.OnWaveFinished += DrawCardAfterWave;

    }

    private void OnDisable()
    {
        HandBuildingCards.OnQueryDrawCard -= TryDrawCardAndUpdateHand;

        HandBuildingCards.OnQueryRedrawCard -= TryRedrawCard;
        HandBuildingCards.OnFinishRedrawing -= DeactivateRedrawCanvas;
        HandBuildingCards.ReturnCardToDeck -= ReturnCardToDeck;

        EnemyWaveManager.OnWaveFinished -= DrawCardAfterWave;
    }

    private void Start()
    {
        redrawingText.text = "Redraws Left: " + hand.GetRedrawsLeft();
        deck.Init();      
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
                hand.InitCardsInHandFirstDraw();
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
    private void DeactivateRedrawCanvas()
    {
        canvas.SetActive(false);
        if (activateWaveCanvas != null) activateWaveCanvas();
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
    }
    private void DrawRandomCard()
    {
        AddCardToHand(deck.GetRandomCard());
    }

    private void AddCardToHand(BuildingCard card)
    {
        hand.HintedCardWillBeAdded();
        hand.AddCard(card);
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
