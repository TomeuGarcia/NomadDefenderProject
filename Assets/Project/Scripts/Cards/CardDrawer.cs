using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDrawer : MonoBehaviour
{
    [SerializeField] private HandBuildingCards hand;
    [SerializeField] private DeckBuildingCards deck;

    [SerializeField, Min(0)] private int numCardsHandStart = 2;

    [SerializeField] private Image drawCooldownImage;
    [SerializeField, Range(10, 60)] private float drawTimeCooldown;
    private float drawCountdown;



    private void OnEnable()
    {
        HandBuildingCards.OnQueryDrawCard += TryDrawCard;
    }

    private void OnDisable()
    {
        HandBuildingCards.OnQueryDrawCard -= TryDrawCard;
    }

    private void Start()
    {
        deck.Init();      
        DrawStartHand();
        hand.Init();

        deck.GetDeckData().SetStarterCardComponentsAsSaved();

        drawCountdown = drawTimeCooldown;

        drawCooldownImage.gameObject.SetActive(false);
        HandBuildingCards.OnCardPlayed += StartDrawOverTime;
    }


    private void TryDrawCard()
    {
        if (deck.HasCardsLeft())
            DrawCard();
    }

    private void DrawCard()
    {
        hand.AddCard(deck.GetTopCard());
    }

    private void DrawStartHand()
    {
        for(int i = 0; i < numCardsHandStart; i++)
        {
            TryDrawCard();
        }
    }

    private void StartDrawOverTime()
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
    }

}
