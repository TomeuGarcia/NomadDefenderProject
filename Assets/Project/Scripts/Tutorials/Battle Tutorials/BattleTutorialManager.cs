using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class BattleTutorialManager : MonoBehaviour
{
    [SerializeField] private CardMotionConfig _cardsMotionConfig;

    //Needs
    //CurrencyBackgroundImg (can set alpha to 0)
    [SerializeField] private TutorialCardDrawer tutoCardDrawer;
    [SerializeField] private HandBuildingCards hand;
    [SerializeField] private EnemyWaveManager enemyWaveManager;

    [SerializeField] private GameObject currencyBackgroundImg;

    //Speed Up Button (can set alpha to 0)
    [SerializeField] private GameObject speedUpButtonHolder;
    [SerializeField] private SpeedUpButton speedUpButton;

    //DeckUI (can set alpha to 0)
    [SerializeField] private GameObject deckInterface;



    [SerializeField] private CanvasGroup blackImg;


    //Get Scripted Sequence
    [SerializeField] private ScriptedSequence scriptedSequence;

    //Get Currency Counter
    [SerializeField] private CurrencyCounter currencyCounter;

    [SerializeField] private int wavesCounter = 0;


    [SerializeField] private TDGameManager tDGameManager;

    [Header("GLITCH")]
    [SerializeField] private Volume globalVolume;
    [SerializeField] private VolumeProfile initVol;
    [SerializeField] private VolumeProfile glitchVol;

    private bool waveFinished = false;

    private bool waveStarted = false;

    private bool allWavesFinished = false;

    private bool firstUpgradeDone = false;


    [SerializeField] private BuildingPlacer _buildingPlacer;
    private int _placedBuildingsCounter;
    private bool PlacedSecondBuilding => _placedBuildingsCounter >= 2;


    [System.Serializable]
    public class TutorialGroup
    {
        [SerializeField] private CanvasGroup _blackImageHighlight;
        [SerializeField] private GameObject _object;
        [SerializeField] private TextDecoder _text;
        [SerializeField, Min(0)] private int _textLinesCount = 1;
        [SerializeField] private bool _showTopWarning = false;

        public void Init()
        {
            _blackImageHighlight.alpha = 0;
            _object.SetActive(false);
        }
        
        public CanvasGroup BlackImageHighlight => _blackImageHighlight;
        public GameObject Object => _object;
        public TextDecoder Text => _text;
        public int TextLinesCount => _textLinesCount;
        public bool ShowTopWarning => _showTopWarning;
    }

    [Header("NEW TUTORIAL")] 
    [SerializeField] private SpriteRenderer _backBackgroundCardsHighlight;
    private float _backBackgroundCardsHighlightAlpha;
    [SerializeField] private HandBuildingCards _hand;
    [SerializeField] private CardOverviewPositioner _cardOverviewPositioner;
    [SerializeField] private TutorialGroup _placeABuildingGroup;
    [SerializeField] private TutorialGroup _currencyTutorialGroup;
    [SerializeField] private TutorialGroup _drawCardTutorialGroup;
    [SerializeField] private TutorialCardOverviewAddOn _differentProjectilesTutorialAddOn;
    [SerializeField] private TutorialCardOverviewAddOn _multiplePassivesTutorialAddOn;
    [SerializeField] private TutorialCardOverviewAddOn _supportsTutorialAddOn;
    [SerializeField] private ScriptedSequence _waitForInputSequence; 
    

    private void Awake()
    {
        tutoCardDrawer.finishRedrawSetup = false;

        _placedBuildingsCounter = 0;
    }

    void Start()
    {
        GameAudioManager.GetInstance().ChangeMusic(GameAudioManager.MusicType.BATTLE, 1f);

        currencyBackgroundImg.GetComponent<CanvasGroup>().alpha = 0;
        currencyBackgroundImg.SetActive(false);

        speedUpButtonHolder.GetComponent<CanvasGroup>().alpha = 0;
        speedUpButtonHolder.SetActive(false);

        deckInterface.GetComponent<CanvasGroup>().alpha = 0;
        deckInterface.SetActive(false);

        blackImg.alpha = 1.0f;

        StartCoroutine(Tutorial());
        hand.cheatDrawCardActivated = false;

        _backBackgroundCardsHighlightAlpha = _backBackgroundCardsHighlight.color.a;
        _backBackgroundCardsHighlight.DOFade(0, 0);
        
        _placeABuildingGroup.Init();
        _currencyTutorialGroup.Init();
        _drawCardTutorialGroup.Init();
    }

    private void OnEnable()
    {
        HandBuildingCards.OnCardPlayed += WaveStarted;
        EnemyWaveManager.OnWaveFinished += WaveStarted;
        EnemyWaveManager.OnStartNewWaves += WaveFinished;
        EnemyWaveManager.OnAllWavesFinished += AllWavesFinished;
        InBattleBuildingUpgrader.OnTurretUpgrade += CheckFirstUpgrade;

        _buildingPlacer.OnBuildingPlaced += OnBuildingPlaced;
    }
    private void OnDisable()
    {
        HandBuildingCards.OnCardPlayed -= WaveStarted;
        EnemyWaveManager.OnWaveFinished -= WaveStarted;
        EnemyWaveManager.OnStartNewWaves -= WaveFinished;
        EnemyWaveManager.OnAllWavesFinished -= AllWavesFinished;
        InBattleBuildingUpgrader.OnTurretUpgrade -= CheckFirstUpgrade;
        
        _buildingPlacer.OnBuildingPlaced -= OnBuildingPlaced;
    }

    private void WaveFinished()
    {
        waveFinished = true;
    }

    private void WaveStarted()
    {
        HandBuildingCards.OnCardPlayed -= WaveStarted;
        waveStarted = true;
        wavesCounter++;
    }

    private void AllWavesFinished()
    {
        allWavesFinished = true;
    }

    private void CheckFirstUpgrade(int newLevel)
    {
        firstUpgradeDone = true;
        InBattleBuildingUpgrader.OnTurretUpgrade -= CheckFirstUpgrade;
    }

    private void OnBuildingPlaced()
    {
        _placedBuildingsCounter++;
    }
    
    IEnumerator Tutorial()
    {
        _cardsMotionConfig.SetTutorialDisplayMode();

        yield return new WaitForSeconds(0.5f);
        scriptedSequence.NextLine(); //0
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );
        yield return new WaitForSeconds(0.5f);

        

        scriptedSequence.NextLine(); //1
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );
        yield return new WaitForSeconds(0.5f);

        //Black Image animation
        for (float i = 1.0f; i < 0.0f; i -= 0.01f)
        {
            blackImg.alpha = i;
            yield return null;
        }
        blackImg.alpha = 0.0f;
        yield return new WaitForSeconds(0.3f);
        blackImg.alpha = 1.0f;
        yield return new WaitForSeconds(0.1f);
        blackImg.alpha = 0.0f;
        yield return new WaitForSeconds(0.07f);
        blackImg.alpha = 1.0f;
        yield return new WaitForSeconds(0.03f);
        blackImg.alpha = 0.0f;
        yield return new WaitForSeconds(0.5f);



        scriptedSequence.NextLine(); //2
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );
        yield return new WaitForSeconds(0.75f);

        scriptedSequence.Clear();

        scriptedSequence.NextLine(); //3
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );
        yield return new WaitForSeconds(0.5f);

        //Show Currency
        currencyBackgroundImg.SetActive(true);

        tutoCardDrawer.ActivateCurrencyUI();

        yield return new WaitForSeconds(1.0f);
        //Currency Finished showing

        
        scriptedSequence.NextLine(); //4
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Mouse0) );
        yield return null;

        scriptedSequence.Clear();
        

        //Starts Cards Tutorial
        tutoCardDrawer.DoGameStartSetup();
        tutoCardDrawer.tutorialCard.InitOverviewPositioner(_cardOverviewPositioner);
        tutoCardDrawer.tutorialCard.MotionEffectsController.DisableMotion();
        

        scriptedSequence.NextLine(); //Initializing User's Deck Line 5
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );
        yield return new WaitForSeconds(1.0f);

        scriptedSequence.NextLine(); //6
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );

        _backBackgroundCardsHighlight.DOFade(_backBackgroundCardsHighlightAlpha, 0.3f);
        
        yield return new WaitForSeconds(0.5f); 
        tutoCardDrawer.tutorialCard.isInteractable = false;
        yield return new WaitForSeconds(0.5f);

        tutoCardDrawer.tutorialCard.ShowTurret = true;
        yield return new WaitForSeconds(0.5f);
        
        scriptedSequence.NextLine(); //7
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );
        yield return StartCoroutine(WaitForInput(false));

        scriptedSequence.Clear();

        tutoCardDrawer.tutorialCard.ShowProjectile = true;
        yield return StartCoroutine(WaitForInput());
        
        tutoCardDrawer.tutorialCard.ShowDamageStat = true;
        yield return StartCoroutine(WaitForInput());
        
        tutoCardDrawer.tutorialCard.ShowShotsPerSecondStat = true;
        yield return StartCoroutine(WaitForInput());
        
        tutoCardDrawer.tutorialCard.ShowRangeStat = true;
        yield return StartCoroutine(WaitForInput());
        
        tutoCardDrawer.tutorialCard.ShowPlayCost = true;
        yield return StartCoroutine(WaitForInput());

        tutoCardDrawer.tutorialCard.Finish = true;
        _backBackgroundCardsHighlight.DOFade(0, 0.3f);
        
        
        scriptedSequence.NextLine();// 8
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );
        yield return new WaitUntil(() => tutoCardDrawer.tutorialCard.FinishedAnimation);
        _hand.InitCardsInHand();

        //Finishes Cards Tutorial
        _cardsMotionConfig.SetTDGameplayHandMode();


        //Skips Redraw
        scriptedSequence.Clear();
        tutoCardDrawer.FinishRedraws();
        // Disable Time speed (x3 etc.)
        speedUpButton.CompletelyDisableTimeSpeed();
        speedUpButtonHolder.SetActive(false);
        
        
        yield return new WaitForSeconds(0.5f);
        tutoCardDrawer.tutorialCard.MotionEffectsController.EnableMotion();
        tutoCardDrawer.UtilityTryDrawRandomCardOfType(BuildingCard.CardBuildingType.TURRET, 1f);
        yield return new WaitForSeconds(2.0f);
        

        

        //Starts Deck Tutorial
        yield return new WaitUntil(() => deckInterface.activeInHierarchy );

        scriptedSequence.Clear();

        tutoCardDrawer.FinishRedrawSetupUI();

        
        StartCoroutine(PlayTutorial(_placeABuildingGroup));
        yield return new WaitUntil(() => waveStarted );
        StopTutorial(_placeABuildingGroup);

        BuildingCard.LockAllCardsFromHover = true;
        yield return new WaitForSeconds(0.25f);

        _backBackgroundCardsHighlight.DOFade(_backBackgroundCardsHighlightAlpha, 0.3f);
        yield return StartCoroutine(PlayTutorial(_currencyTutorialGroup));
        _backBackgroundCardsHighlight.DOFade(0, 0.3f);
        BuildingCard.LockAllCardsFromHover = false;
        

        scriptedSequence.NextLine(); //9
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );
        yield return new WaitForSeconds(1.5f);
        scriptedSequence.Clear();
        
        

        scriptedSequence.NextLine(); //10
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );
        yield return new WaitForSeconds(1.5f);

        //First Wave 1/1
        scriptedSequence.NextLine();//11
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );
        yield return new WaitForSeconds(0.5f);
        
        

        
        

        //Second Wave (2/3)
        yield return new WaitUntil(() => wavesCounter > 1);
        scriptedSequence.Clear();


        _hand.CanBeHidden = false;
        BuildingCard.LockAllCardsFromHover = true;
        TurretBuildingCard projectileCard = tutoCardDrawer.UtilityTryDrawRandomCardOfType(BuildingCard.CardBuildingType.TURRET, 1f) as TurretBuildingCard;
        TurretBuildingCard passivesCard = tutoCardDrawer.UtilityTryDrawRandomCardOfType(BuildingCard.CardBuildingType.TURRET, 1f) as TurretBuildingCard;
        
        
        yield return new WaitForSeconds(1.0f);
        _backBackgroundCardsHighlight.DOFade(_backBackgroundCardsHighlightAlpha, 0.3f);
        yield return StartCoroutine(PlayTutorial(_drawCardTutorialGroup));


        yield return new WaitForSeconds(0.25f);

        GameTime.SetTimeScale(0);
        yield return new WaitForSeconds(0.25f);


        _cardOverviewPositioner.Init(projectileCard);
        yield return StartCoroutine(_cardOverviewPositioner.PositionToSpot());
        yield return StartCoroutine(PlayDifferentProjectileTutorial(projectileCard));
        yield return StartCoroutine(_cardOverviewPositioner.UndoPositioning());
        _hand.InitCardsInHand(false);
        yield return new WaitForSeconds(0.25f);
        
        _cardOverviewPositioner.Init(passivesCard);
        yield return StartCoroutine(_cardOverviewPositioner.PositionToSpot());
        yield return StartCoroutine(PlayDifferentPassivesTutorial(passivesCard));
        yield return StartCoroutine(_cardOverviewPositioner.UndoPositioning());
        _hand.InitCardsInHand(false);
        yield return new WaitForSeconds(1.0f);
        

        
        GameTime.SetTimeScale(1);
        _backBackgroundCardsHighlight.DOFade(0, 0.3f);
        BuildingCard.LockAllCardsFromHover = false;
        _hand.CanBeHidden = true;
        _hand.InitCardsInHand();

        
        yield return new WaitForSeconds(1.5f);
        scriptedSequence.NextLine();//12
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );
        
        

        //Wave 3/3
        yield return new WaitUntil(() => wavesCounter > 2 );
        scriptedSequence.Clear();
        yield return new WaitForSeconds(0.5f);
        

        _hand.CanBeHidden = false;
        BuildingCard.LockAllCardsFromHover = true;
        BuildingCard supportCard1 = tutoCardDrawer.UtilityTryDrawRandomCardOfType(BuildingCard.CardBuildingType.SUPPORT, 1f);
        
        GameTime.SetTimeScale(0);
        _backBackgroundCardsHighlight.DOFade(_backBackgroundCardsHighlightAlpha, 0.3f);
        yield return new WaitForSeconds(0.25f);
        
        _cardOverviewPositioner.Init(supportCard1);
        yield return StartCoroutine(_cardOverviewPositioner.PositionToSpot());
        yield return StartCoroutine(PlaySupportTutorial(supportCard1));
        yield return StartCoroutine(_cardOverviewPositioner.UndoPositioning());
        BuildingCard anyCard2 = tutoCardDrawer.UtilityTryDrawAnyRandomCard(1f);

        yield return new WaitForSeconds(1.0f);
        
        
        GameTime.SetTimeScale(1);
        _backBackgroundCardsHighlight.DOFade(0, 0.3f);
        BuildingCard.LockAllCardsFromHover = false;
        _hand.CanBeHidden = true;
        _hand.InitCardsInHand();
        
        scriptedSequence.NextLine();//13
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );

        //Wave 4/5
        yield return new WaitUntil(() => wavesCounter > 3 );
        scriptedSequence.Clear();
        yield return new WaitForSeconds(0.5f);
        scriptedSequence.NextLine();//14
        
        //Wave 5/5
        yield return new WaitUntil(() => wavesCounter > 4 );
        scriptedSequence.Clear();
        yield return new WaitForSeconds(0.5f);
        scriptedSequence.NextLine();//15
        
        // TODO: REDO FROM BELOW ONWARDS
        // yield return StartCoroutine(MakePlayerUpgradeTurret_Tutorial());


        //Last Wave (4/3)
        yield return new WaitUntil(() => wavesCounter > 3);
        enemyWaveManager.HideWaveSpawnersInfoDisplay();
        scriptedSequence.Clear();


        

        scriptedSequence.NextLine(); //16 Wave 6/5
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );
        yield return new WaitForSeconds(4.0f);


        GameAudioManager.GetInstance().MusicFadeOut(1f);


        yield return new WaitForSecondsRealtime(1.0f);
        GameAudioManager.GetInstance().PlayGlitchSound(2);
        globalVolume.profile = glitchVol;
        yield return new WaitForSecondsRealtime(0.2f);
        globalVolume.profile = initVol;
        yield return new WaitForSecondsRealtime(0.2f);
        globalVolume.profile = glitchVol;
        yield return new WaitForSecondsRealtime(0.1f);
        globalVolume.profile = initVol;
        yield return new WaitForSecondsRealtime(0.1f);
        globalVolume.profile = glitchVol;
        yield return new WaitForSecondsRealtime(0.1f);
        globalVolume.profile = initVol;
        

        scriptedSequence.NextLine(); //17
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );
        yield return new WaitForSecondsRealtime(0.25f);

        scriptedSequence.NextLine(); //18
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );
        yield return new WaitForSecondsRealtime(0.25f);

        scriptedSequence.NextLine(); //19
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );
        yield return new WaitForSecondsRealtime(0.25f);

        scriptedSequence.NextLine(); //20
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );
        yield return new WaitForSecondsRealtime(0.25f);

        scriptedSequence.NextLine(); //21
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );
        yield return new WaitForSecondsRealtime(0.25f);
        
        //Random Bullshit Text
        scriptedSequence.NextLine(); //22
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );
        yield return new WaitForSecondsRealtime(0.25f);

        scriptedSequence.NextLine(); //23
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );
        yield return new WaitForSecondsRealtime(0.25f);

        scriptedSequence.NextLine(); //24
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );
        yield return new WaitForSecondsRealtime(0.25f);
        
        scriptedSequence.NextLine(); //25
        yield return new WaitForSecondsRealtime(1.5f);

        scriptedSequence.NextLine(); //26
        yield return new WaitForSecondsRealtime(1.5f);

        scriptedSequence.NextLine(); //27
        yield return new WaitForSecondsRealtime(1.5f);

        scriptedSequence.NextLine();//28
        yield return new WaitForSecondsRealtime(1.0f);

        scriptedSequence.NextLine();//29
        yield return new WaitForSecondsRealtime(1.0f);

        scriptedSequence.NextLine();//30
        yield return new WaitForSecondsRealtime(1.0f);

        scriptedSequence.NextLine();//31
        yield return new WaitForSecondsRealtime(0.75f);

        scriptedSequence.NextLine();//32
        yield return new WaitForSecondsRealtime(0.75f);

        scriptedSequence.NextLine();//33
        yield return new WaitForSecondsRealtime(0.75f);
        

        //Finish scene and load next
        GameAudioManager.GetInstance().ChangeMusic(GameAudioManager.MusicType.OWMAP, 1f);
        TutorialsSaverLoader.GetInstance().SetTutorialDone(Tutorials.BATTLE);
        tDGameManager.ForceFinishScene();
    }


    private IEnumerator WaitForInput(bool withText = true)
    {
        yield return new WaitForSeconds(0.2f);

        DelayedTextLife life = new DelayedTextLife();
        if (withText)
        {
            StartCoroutine(PlayWaitForInputText(life));
        }
        
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Mouse0) );
        life.died = true;
    }

    private IEnumerator PlayWaitForInputText(DelayedTextLife life)
    {
        yield return new WaitForSeconds(1.5f);

        if (life.died)
        {
            yield break;
        }
        
        _waitForInputSequence.ResetDialog();
        _waitForInputSequence.NextLine();
        
        yield return new WaitUntil(() => life.died );
        _waitForInputSequence.Clear(); 
    } 

    private class DelayedTextLife
    {
        public bool died = false;
    }

    
    private IEnumerator PlayTutorial(TutorialGroup group)
    {
        GameTime.SetTimeScale(0f);

        if (group.ShowTopWarning)
        {
            ServiceLocator.GetInstance().TutorialViewUtilities.ShowWarningTopBar();
        }
        
        yield return group.BlackImageHighlight.DOFade(1f, 0.5f)
            .SetUpdate(true)
            .SetEase(Ease.InOutSine);

        group.Object.SetActive(true);
        
        group.Text.Activate();
        for (int i = 0; i < group.TextLinesCount - 1; ++i)
        {
            yield return new WaitUntil(() => group.Text.FinishedLine);
            yield return StartCoroutine(WaitForInput());
            
            group.Text.NextLine();
        }
        yield return new WaitUntil(() => group.Text.IsDoneDecoding());
        yield return StartCoroutine(WaitForInput());
        
        group.Object.SetActive(false);
        
        if (group.ShowTopWarning)
        {
            ServiceLocator.GetInstance().TutorialViewUtilities.HideWarningTopBar();
        }
        yield return group.BlackImageHighlight.DOFade(0f, 0.5f)
            .SetUpdate(true)
            .SetEase(Ease.InOutSine);
        
        GameTime.SetTimeScale(1f);
    }

    private void StopTutorial(TutorialGroup group)
    {
        group.Object.SetActive(false);
    }


    private IEnumerator PlayDifferentProjectileTutorial(TurretBuildingCard card)
    {
        TutorialCardOverviewAddOn tutorialAddOn = Instantiate(_differentProjectilesTutorialAddOn, card.CardTransform);
        tutorialAddOn.SetExtra(new TutorialCardOverviewAddOnExtra_Projectile(card));
        yield return StartCoroutine(PlayCardAddOnTutorial(tutorialAddOn));
    }
    private IEnumerator PlayDifferentPassivesTutorial(TurretBuildingCard card)
    {
        TutorialCardOverviewAddOn tutorialAddOn = Instantiate(_multiplePassivesTutorialAddOn, card.CardTransform);
        tutorialAddOn.SetExtra(new TutorialCardOverviewAddOnExtra_Passives(card));
        yield return StartCoroutine(PlayCardAddOnTutorial(tutorialAddOn));
    }
    private IEnumerator PlaySupportTutorial(BuildingCard card)
    {
        TutorialCardOverviewAddOn tutorialAddOn = Instantiate(_supportsTutorialAddOn, card.CardTransform);
        yield return StartCoroutine(PlayCardAddOnTutorial(tutorialAddOn));
    }
    private IEnumerator PlayCardAddOnTutorial(TutorialCardOverviewAddOn tutorialAddOn)
    {
        yield return StartCoroutine(tutorialAddOn.Play());
        yield return StartCoroutine(WaitForInput());
        tutorialAddOn.Finish();
    }



    private IEnumerator MakePlayerUpgradeTurret_Tutorial()
    {
                
        //Place Another Turret
        yield return new WaitUntil(() => currencyCounter.HasEnoughCurrency(125) );
        scriptedSequence.NextLine(); //14 Place another Turret
        GameTime.SetTimeScale(0f);
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );
        yield return new WaitUntil(() =>  PlacedSecondBuilding);
        scriptedSequence.Clear();
        GameTime.SetTimeScale(1f);
        
        scriptedSequence.NextLine(); //16 Wave 3/3
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );

        //START UPGRADE TURRET TUTORIAL
        yield return new WaitUntil(() => currencyCounter.HasEnoughCurrency(150) );
        yield return new WaitForSeconds(0.5f); //Extra wait
        scriptedSequence.Clear();

        GameTime.SetTimeScale(0f);

        scriptedSequence.NextLine();//17
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );
        yield return new WaitForSecondsRealtime(0.5f);

        scriptedSequence.NextLine(); //18
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitUntil(() => firstUpgradeDone == true);

        scriptedSequence.NextLine(); //19
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );
        yield return new WaitForSecondsRealtime(0.5f);
        GameTime.SetTimeScale(1f);
        //FINISH UPGRADE TURRET TUTORIAL

    }
    
}
