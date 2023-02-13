using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialCardDrawer : CardDrawer
{
    [Header("TUTORIAL")]
    [SerializeField] private GameObject tutorialTurretCardPrefab;
    [SerializeField] private TurretCardParts tutorialCardParts;

    [HideInInspector] public TutoTurretCard tutorialCard;

    private void Awake()
    {
        tutorialCard = GetUninitializedNewTutorailTurretCard();
        tutorialCard.ResetParts(tutorialCardParts);
    }

    private void Start()
    {
        // OVERRIDING Start()

        battleHUD.DeactivateCurrencyUI();
    }

    public void DoGameStartSetup()
    {
        battleHUD.DisableSpeedUpUI();
        battleHUD.canShowDrawCardButton = false;
        GameStartSetup();
    }

    protected override void SetupDeck()
    {
        deck.Init();

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

}
