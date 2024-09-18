using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using static InBattleBuildingUpgrader;

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

        public void Init()
        {
            _blackImageHighlight.alpha = 0;
            _object.SetActive(false);
        }
        
        public CanvasGroup BlackImageHighlight => _blackImageHighlight;
        public GameObject Object => _object;
        public TextDecoder Text => _text;
        public int TextLinesCount => _textLinesCount;
    }
    
    [Header("NEW TUTORIAL")] 
    [SerializeField] private TutorialGroup _currencyTutorialGroup;
    [SerializeField] private TutorialGroup _drawCardTutorialGroup;
    [SerializeField] private TutorialCardOverviewAddOn _differentProjectilesTutorialAddOn;
    [SerializeField] private TutorialCardOverviewAddOn _multiplePassivesTutorialAddOn;

    

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
        tutoCardDrawer.tutorialCard.MotionEffectsController.DisableMotion();


        scriptedSequence.NextLine(); //Initializing User's Deck Line 5
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );
        yield return new WaitForSeconds(1.0f);

        scriptedSequence.NextLine(); //6
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );

        yield return new WaitForSeconds(0.5f); 
        tutoCardDrawer.tutorialCard.isInteractable = false;
        yield return new WaitForSeconds(0.5f);

        tutoCardDrawer.tutorialCard.ShowTurret = true;
        yield return new WaitForSeconds(0.5f);

        scriptedSequence.NextLine(); //7
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );
        yield return StartCoroutine(WaitForInput());

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


        yield return StartCoroutine(
            PlayDifferentProjectileTutorial(tutoCardDrawer.tutorialCard, _differentProjectilesTutorialAddOn));
        yield return StartCoroutine(
            PlayDifferentProjectileTutorial(tutoCardDrawer.tutorialCard, _multiplePassivesTutorialAddOn));
        
        tutoCardDrawer.tutorialCard.Finish = true;
        
        
        
        scriptedSequence.NextLine();// 8
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );
        yield return new WaitUntil(() => tutoCardDrawer.tutorialCard.FinishedAnimation);

        //Finishes Cards Tutorial
        _cardsMotionConfig.SetTDGameplayHandMode();


        //Skips Redraw
        scriptedSequence.Clear();
        tutoCardDrawer.FinishRedraws();
        
        
        yield return new WaitForSeconds(0.5f);
        tutoCardDrawer.tutorialCard.MotionEffectsController.EnableMotion();
        tutoCardDrawer.UtilityTryDrawRandomCardOfType(BuildingCard.CardBuildingType.TURRET, 1f);
        yield return new WaitForSeconds(0.5f);
        tutoCardDrawer.UtilityTryDrawRandomCardOfType(BuildingCard.CardBuildingType.TURRET, 1f);
        yield return new WaitForSeconds(0.5f);


        yield return new WaitForSeconds(1.5f);
       

        

        //Starts Deck Tutorial
        yield return new WaitUntil(() => deckInterface.activeInHierarchy );

        scriptedSequence.Clear();

        tutoCardDrawer.FinishRedrawSetupUI();

        scriptedSequence.NextLine(); //9
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );
        yield return new WaitUntil(() => waveStarted );
        
        yield return StartCoroutine(PlayTutorial(_currencyTutorialGroup));

        

        scriptedSequence.NextLine(); //10
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );
        yield return new WaitForSeconds(1.5f);
        scriptedSequence.Clear();
        
        

        scriptedSequence.NextLine(); //11
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );
        yield return new WaitForSeconds(1.5f);

        //First Wave 1/1
        scriptedSequence.NextLine();//12
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );
        
        
        
        yield return new WaitForSeconds(0.5f);
        

        
        scriptedSequence.NextLine();//13

        //Second Wave (2/3)
        yield return new WaitUntil(() => wavesCounter > 1);
        
        scriptedSequence.Clear();
        scriptedSequence.NextLine(); //14
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );

        
        yield return new WaitForSeconds(2.0f);
        yield return StartCoroutine(PlayTutorial(_drawCardTutorialGroup));

        //Place Another Turret
        yield return new WaitUntil(() => currencyCounter.HasEnoughCurrency(125) );
        scriptedSequence.NextLine(); //15 Place another Turret
        GameTime.SetTimeScale(0f);
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );
        yield return new WaitUntil(() =>  PlacedSecondBuilding);
        scriptedSequence.Clear();
        GameTime.SetTimeScale(1f);
        
        //Wave 3/3
        yield return new WaitUntil(() => wavesCounter > 2 );
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


        //Last Wave (4/3)
        yield return new WaitUntil(() => wavesCounter > 3);
        enemyWaveManager.HideWaveSpawnersInfoDisplay();
        scriptedSequence.Clear();

        // Disable Time speed (x3 etc.)
        speedUpButton.CompletelyDisableTimeSpeed();
        speedUpButtonHolder.SetActive(false);

        scriptedSequence.NextLine(); //25 Defense Finished?
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
        

        scriptedSequence.NextLine(); //26
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );
        yield return new WaitForSecondsRealtime(0.25f);

        scriptedSequence.NextLine(); //27
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );
        yield return new WaitForSecondsRealtime(0.25f);

        scriptedSequence.NextLine(); //28
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );
        yield return new WaitForSecondsRealtime(0.25f);

        scriptedSequence.NextLine(); //29
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );
        yield return new WaitForSecondsRealtime(0.25f);

        scriptedSequence.NextLine(); //30
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );
        yield return new WaitForSecondsRealtime(0.25f);

        scriptedSequence.NextLine(); //31
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );
        yield return new WaitForSecondsRealtime(0.25f);

        scriptedSequence.NextLine(); //32
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );
        yield return new WaitForSecondsRealtime(0.25f);

        scriptedSequence.NextLine(); //33
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );
        yield return new WaitForSecondsRealtime(0.25f);

        //Random Bullshit Text
        scriptedSequence.NextLine(); //34
        yield return new WaitForSecondsRealtime(1.5f);

        scriptedSequence.NextLine(); //35
        yield return new WaitForSecondsRealtime(1.5f);

        scriptedSequence.NextLine(); //36
        yield return new WaitForSecondsRealtime(1.5f);

        scriptedSequence.NextLine();//37
        yield return new WaitForSecondsRealtime(1.0f);

        scriptedSequence.NextLine();//38
        yield return new WaitForSecondsRealtime(1.0f);

        scriptedSequence.NextLine();//39
        yield return new WaitForSecondsRealtime(1.0f);

        scriptedSequence.NextLine();//40
        yield return new WaitForSecondsRealtime(0.75f);

        scriptedSequence.NextLine();//41
        yield return new WaitForSecondsRealtime(0.75f);

        scriptedSequence.NextLine();//42
        yield return new WaitForSecondsRealtime(0.75f);

        scriptedSequence.NextLine();//43
        yield return new WaitForSecondsRealtime(0.75f);

        scriptedSequence.NextLine();//44
        yield return new WaitForSecondsRealtime(0.75f);
        

        //Finish scene and load next
        GameAudioManager.GetInstance().ChangeMusic(GameAudioManager.MusicType.OWMAP, 1f);
        TutorialsSaverLoader.GetInstance().SetTutorialDone(Tutorials.BATTLE);
        tDGameManager.ForceFinishScene();
    }


    private IEnumerator WaitForInput()
    {
        yield return new WaitForSeconds(0.2f);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Mouse0) );
    }

    
    private IEnumerator PlayTutorial(TutorialGroup group)
    {
        GameTime.SetTimeScale(0f);
        
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
        yield return group.BlackImageHighlight.DOFade(0f, 0.5f)
            .SetUpdate(true)
            .SetEase(Ease.InOutSine);
        
        GameTime.SetTimeScale(1f);
    }


    private IEnumerator PlayDifferentProjectileTutorial(TurretBuildingCard card, TutorialCardOverviewAddOn tutorialAddOnPrefab)
    {
        TutorialCardOverviewAddOn tutorialAddOn = Instantiate(tutorialAddOnPrefab, card.CardTransform);
        yield return StartCoroutine(tutorialAddOn.Play());
        yield return StartCoroutine(WaitForInput());
        tutorialAddOn.Finish();
    }
    
}
