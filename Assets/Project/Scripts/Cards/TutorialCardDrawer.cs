using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialCardDrawer : CardDrawer
{
    [Header("TUTORIAL")]
    [SerializeField] private GameObject tutorialTurretCardPrefab;

    [HideInInspector] public TutoTurretCard tutorialCard;

    private void Awake()
    {
        tutorialCard = GetUninitializedNewTutorailTurretCard();
    }

    private void Start()
    {
        // OVERRIDING Start()

        battleHUD.DeactivateCurrencyUI();
        redrawCanvasGameObject.SetActive(false);
    }


    public void DoGameStartSetup()
    {
        battleHUD.DisableSpeedUpUI();
        battleHUD.canShowDrawCardButton = false;
        GameStartSetup(0f, false, finishRedrawSetup);
    }

    protected override void SetupDeck()
    {
        deck.Init();

        // Transform the 1st card in the deck (1st DeckData turret) into a Tutorial Card
        tutorialCard.ResetParts(deck.GetAndRemoveCard(0).GetComponent<TurretBuildingCard>().turretCardParts); 

        deck.AddCardToDeckTop(tutorialCard); // Adding the tutorial card to the top     

        battleHUD.InitDeckCardIcons(deck.NumCards);
    }


    private TutoTurretCard GetUninitializedNewTutorailTurretCard()
    {
        return Instantiate(tutorialTurretCardPrefab, transform).GetComponent<TutoTurretCard>();
    }


    public void ActivateCurrencyUI()
    {
        battleHUD.ActivateCurrencyUI();
    }

    public BattleHUD GetBattleHUD()
    {
        return battleHUD;
    }
}
