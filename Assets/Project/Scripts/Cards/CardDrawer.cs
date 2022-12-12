using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDrawer : MonoBehaviour
{
    [SerializeField] private HandBuildingCards hand;
    [SerializeField] private DeckBuildingCards deck;

    [SerializeField, Min(1)] private int numCardsHandStart = 2;

    [SerializeField] private Image drawCooldownImage;
    [SerializeField, Range(10, 60)] private float drawTimeCooldown;
    private float drawCountdown;

    [SerializeField] private int cardsToDrawPerWave;


    private void OnEnable()
    {
        HandBuildingCards.OnQueryDrawCard += TryDrawCard;
        EnemyWaveManager.OnWaveFinished += DrawCardAfterWave;

    }

    private void OnDisable()
    {
        HandBuildingCards.OnQueryDrawCard -= TryDrawCard;
        EnemyWaveManager.OnWaveFinished -= DrawCardAfterWave;
    }

    private void Start()
    {
        deck.Init();      
        DrawStartHand();
        hand.Init();

        deck.GetDeckData().SetStarterCardComponentsAsSaved();

        drawCountdown = drawTimeCooldown;

        drawCooldownImage.gameObject.SetActive(false);
        //HandBuildingCards.OnCardPlayed += StartDrawOverTime;
    }


    private void TryDrawCard()
    {
        if (deck.HasCardsLeft())
            DrawRandomCard();
    }

    private void DrawTopCard()
    {
        hand.AddCard(deck.GetTopCard());
    }
    private void DrawRandomCard()
    {
        hand.AddCard(deck.GetRandomCard());
    }

    private void DrawCardAfterWave()
    {
        for (int i = 0; i < cardsToDrawPerWave; i++)
            TryDrawCard();
    }


    private void DrawStartHand()
    {
        DrawTopCard();

        for (int i = 1; i < numCardsHandStart; i++)
        {
            TryDrawCard();
        }
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
