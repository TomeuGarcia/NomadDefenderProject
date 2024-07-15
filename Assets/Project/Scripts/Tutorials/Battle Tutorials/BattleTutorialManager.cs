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

    //Needs
    //CurrencyBackgroundImg (can set alpha to 0)
    [SerializeField] private TutorialCardDrawer tutoCardDrawer;
    [SerializeField] private HandBuildingCards hand;
    [SerializeField] private EnemyWaveManager enemyWaveManager;

    [SerializeField] private GameObject currencyBackgroundImg;

    //Speed Up Button (can set alpha to 0)
    [SerializeField] private GameObject speedUpButton;

    //DeckUI (can set alpha to 0)
    [SerializeField] private GameObject deckInterface;

    //Card Drawer -> Canvas -> BackgroundImage (can set alpha to 0)
    [SerializeField] private GameObject redrawInterface;


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

        speedUpButton.GetComponent<CanvasGroup>().alpha = 0;
        speedUpButton.SetActive(false);

        deckInterface.GetComponent<CanvasGroup>().alpha = 0;
        deckInterface.SetActive(false);

        //redrawInterface.GetComponent<CanvasGroup>().alpha = 0;
        //redrawInterface.SetActive(false);

        blackImg.alpha = 1.0f;

        StartCoroutine(Tutorial());
        hand.cheatDrawCardActivated = false;
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

        yield return new WaitForSeconds(1.0f);
        scriptedSequence.NextLine(); //0
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );
        yield return new WaitForSeconds(1.0f);


        scriptedSequence.NextLine(); //1
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );
        yield return new WaitForSeconds(1.0f);

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
        yield return new WaitForSeconds(1.0f);

        scriptedSequence.Clear();

        scriptedSequence.NextLine(); //3
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );
        yield return new WaitForSeconds(0.5f);

        //Show Currency
        currencyBackgroundImg.SetActive(true);

        tutoCardDrawer.ActivateCurrencyUI();

        yield return new WaitForSeconds(2.0f);


        scriptedSequence.NextLine(); //4
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Mouse0) );
        yield return null;

        scriptedSequence.Clear();

        //Currency Finished showing


        //Starts Cards Tutorial
        tutoCardDrawer.DoGameStartSetup();

        scriptedSequence.NextLine(); //Initializing User's Deck Line 5
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );
        yield return new WaitForSeconds(2.5f);

        scriptedSequence.NextLine(); //6
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );

        yield return new WaitForSeconds(1.0f); 
        tutoCardDrawer.tutorialCard.isInteractable = false;
        yield return new WaitForSeconds(1.5f);


        //Cards Visuals
        scriptedSequence.NextLine(); //7
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(0.5f);
        tutoCardDrawer.tutorialCard.showTurret = true;
        yield return new WaitForSeconds(1.5f);
        scriptedSequence.Clear();


        //Cards Stats
        scriptedSequence.NextLine();// 8
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );
        yield return new WaitForSeconds(0.5f);


        //Cost
        scriptedSequence.NextLine();// 9
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        tutoCardDrawer.tutorialCard.showPlayCost = true;
        yield return new WaitForSeconds(1.5f);

        //Attack
        scriptedSequence.NextLine();// 10
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );
        tutoCardDrawer.tutorialCard.showAttackStat = true;
        yield return new WaitForSeconds(1.5f);

        //Cadency
        scriptedSequence.NextLine();// 11
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );
        tutoCardDrawer.tutorialCard.showCadenceStat = true;
        yield return new WaitForSeconds(1.5f);

        //Range
        scriptedSequence.NextLine();// 12
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );
        tutoCardDrawer.tutorialCard.showRangeStat = true;
        yield return new WaitForSeconds(1.5f);



        scriptedSequence.NextLine();// 13
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );
        yield return new WaitForSeconds(1.0f);

        //Finishes Cards Tutorial


        
        //Skips Redraw
        scriptedSequence.Clear();
        tutoCardDrawer.FinishRedraws();
        tutoCardDrawer.tutorialCard.isInteractable = true;

        yield return new WaitForSeconds(1.0f);

       

        

        //Starts Deck Tutorial
        yield return new WaitUntil(() =>deckInterface.activeInHierarchy );

        scriptedSequence.Clear();

        tutoCardDrawer.FinishRedrawSetupUI();

        scriptedSequence.NextLine(); //14
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );
        yield return new WaitForSeconds(1.5f);

        scriptedSequence.NextLine(); //15
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );
        yield return new WaitUntil(() => waveStarted );

        scriptedSequence.NextLine(); //16
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );
        yield return new WaitForSeconds(1.5f);
        scriptedSequence.Clear();

        scriptedSequence.NextLine();//17
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );
        yield return new WaitForSeconds(0.5f);
        

        //First Wave 1/1
        scriptedSequence.NextLine();//18
        
        //Second Wave (2/2)
        yield return new WaitUntil(() => wavesCounter > 1);
        scriptedSequence.Clear();
        scriptedSequence.NextLine(); //19
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );


        yield return new WaitUntil(() => currencyCounter.HasEnoughCurrency(150) );
        yield return new WaitForSeconds(0.5f); //Extra wait
        scriptedSequence.Clear();

        GameTime.SetTimeScale(0f);

        scriptedSequence.NextLine();//20
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );
        yield return new WaitForSecondsRealtime(0.5f);

        scriptedSequence.NextLine(); //21
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitUntil(() => firstUpgradeDone == true);

        scriptedSequence.NextLine(); //22
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );
        yield return new WaitForSecondsRealtime(0.5f);
        
        GameTime.SetTimeScale(1f);


        
        scriptedSequence.NextLine(); //23
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );
        yield return new WaitUntil(() => currencyCounter.HasEnoughCurrency(170) );
        scriptedSequence.Clear();
        
        scriptedSequence.NextLine(); //24 Place another Turret
        GameTime.SetTimeScale(0f);
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );
        yield return new WaitUntil(() =>  PlacedSecondBuilding);
        GameTime.SetTimeScale(1f);
        
        enemyWaveManager.HideWaveSpawnersInfoDisplay();

        //Last Wave
        yield return new WaitUntil(() => wavesCounter > 3);
        scriptedSequence.Clear();
        
        scriptedSequence.NextLine(); //25 Defense Finished?
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );
        yield return new WaitForSecondsRealtime(4.0f);


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
        yield return new WaitForSecondsRealtime(0.5f);



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

        scriptedSequence.NextLine(); //34
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() );
        yield return new WaitForSecondsRealtime(0.25f);

        //Random Bullshit Text
        scriptedSequence.NextLine(); //35
        yield return new WaitForSecondsRealtime(1.5f);

        scriptedSequence.NextLine(); //36
        yield return new WaitForSecondsRealtime(1.5f);

        scriptedSequence.NextLine(); //37
        yield return new WaitForSecondsRealtime(1.5f);

        scriptedSequence.NextLine();//38
        yield return new WaitForSecondsRealtime(1.0f);

        scriptedSequence.NextLine();//39
        yield return new WaitForSecondsRealtime(1.0f);

        scriptedSequence.NextLine();//40
        yield return new WaitForSecondsRealtime(1.0f);

        scriptedSequence.NextLine();//40
        yield return new WaitForSecondsRealtime(0.75f);

        scriptedSequence.NextLine();//42
        yield return new WaitForSecondsRealtime(0.75f);

        scriptedSequence.NextLine();//43
        yield return new WaitForSecondsRealtime(0.75f);

        scriptedSequence.NextLine();//44
        yield return new WaitForSecondsRealtime(0.75f);

        scriptedSequence.NextLine();//45
        yield return new WaitForSecondsRealtime(0.75f);

        scriptedSequence.NextLine();//46
        yield return new WaitForSecondsRealtime(0.75f);

        scriptedSequence.NextLine();//47
        yield return new WaitForSecondsRealtime(2.8f);

        //Finish scene and load next
        GameAudioManager.GetInstance().ChangeMusic(GameAudioManager.MusicType.OWMAP, 1f);
        TutorialsSaverLoader.GetInstance().SetTutorialDone(Tutorials.BATTLE);
        tDGameManager.ForceFinishScene();

    }

}
