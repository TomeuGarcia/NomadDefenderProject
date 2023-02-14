using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using static InBattleBuildingUpgrader;

public class BattleTutorialManager : MonoBehaviour
{

    //Needs
    //CurrencyBackgroundImg (can set alpha to 0)
    [SerializeField] private TutorialCardDrawer tutoCardDrawer;
    [SerializeField] private HandBuildingCards hand;

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

    private bool waveFinished = false;

    private bool waveStarted = false;

    private bool allWavesFinished = false;

    private bool firstUpgradeDone = false;

    // Start is called before the first frame update
    void Start()
    {
        currencyBackgroundImg.GetComponent<CanvasGroup>().alpha = 0;
        currencyBackgroundImg.SetActive(false);

        speedUpButton.GetComponent<CanvasGroup>().alpha = 0;
        speedUpButton.SetActive(false);

        deckInterface.GetComponent<CanvasGroup>().alpha = 0;
        deckInterface.SetActive(false);

        redrawInterface.GetComponent<CanvasGroup>().alpha = 0;
        redrawInterface.SetActive(false);

        blackImg.alpha = 1.0f;

        StartCoroutine(Tutorial());
        hand.cheatDrawCardActivated = false;

        //Init Events
        HandBuildingCards.OnCardPlayed += WaveStarted;
        EnemyWaveManager.OnWaveFinished += WaveStarted;
        EnemyWaveManager.OnStartNewWaves += WaveFinished;
        EnemyWaveManager.OnAllWavesFinished += AllWavesFinished;
        InBattleBuildingUpgrader.OnTurretUpgrade += CheckFirstUpgrade;
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

    IEnumerator Tutorial()
    {

        yield return new WaitForSeconds(1.0f);


        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSeconds(1.0f);


        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
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



        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSeconds(1.0f);

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => Input.GetKeyDown("space") == true);
        yield return null;

        scriptedSequence.Clear();

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSeconds(0.5f);

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSeconds(2.0f);

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSeconds(0.5f);

        //Show Currency
        currencyBackgroundImg.SetActive(true);

        tutoCardDrawer.ActivateCurrencyUI();

        yield return new WaitForSeconds(0.5f);

        scriptedSequence.NextLine();
        yield return new WaitForSeconds(2.0f);

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => Input.GetKeyDown("space") == true);
        yield return null;

        //Currency Finished showing


        //Starts Cards Tutorial

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        tutoCardDrawer.DoGameStartSetup();

        yield return new WaitForSeconds(1.0f);
        tutoCardDrawer.tutorialCard.isInteractable = false;
        yield return new WaitForSeconds(1.5f);

        //Cards Stats
        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSeconds(0.5f);



        //Card Level
        


        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        tutoCardDrawer.tutorialCard.showLevel = true;
        yield return new WaitForSeconds(2.5f);

        //Range

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        tutoCardDrawer.tutorialCard.showRangeStat = true;
        yield return new WaitForSeconds(2.5f);

        //Cadency

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        tutoCardDrawer.tutorialCard.showCadenceStat = true;
        yield return new WaitForSeconds(2.5f);

        //Attack

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        tutoCardDrawer.tutorialCard.showAttackStat = true;
        yield return new WaitForSeconds(2.5f);

        //Cost

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        tutoCardDrawer.tutorialCard.showPlayCost = true;
        yield return new WaitForSeconds(2.5f);

        //Projectile

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        tutoCardDrawer.tutorialCard.showAttackIcon = true;
        yield return new WaitForSeconds(2.5f);

        //Passive
        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        tutoCardDrawer.tutorialCard.showBasePassiveIcon = true;
        yield return new WaitForSeconds(1.0f);


        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSeconds(1.0f);

        //Finishes Cards Tutorial



        //Starts Redraw Tutorial
        scriptedSequence.Clear();
        yield return new WaitForSeconds(5.0f);

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSeconds(1.0f);

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSeconds(1.0f);

        //Show Redraw
        redrawInterface.SetActive(true);

        for (float i = 0.0f; i < 1.0f; i += 0.01f)
        {
            redrawInterface.GetComponent<CanvasGroup>().alpha = i;
            yield return null;
        }
        redrawInterface.GetComponent<CanvasGroup>().alpha = 1.0f;

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSeconds(1.0f);


        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSeconds(1.0f);

        tutoCardDrawer.tutorialCard.isInteractable= true;

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSeconds(1.0f);

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSeconds(1.0f);

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSeconds(0.5f);

        //Finishes Redraw Tutorial


        //Starts Deck Tutorial
        yield return new WaitUntil(() =>deckInterface.active == true);

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSeconds(0.5f);

        scriptedSequence.NextLine(); //Initializing User's Deck Line
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSeconds(5.0f);

        scriptedSequence.Clear();

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSeconds(1.5f);

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSeconds(1.0f);

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSeconds(1.0f);

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => waveStarted == true);

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSeconds(0.5f);

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSeconds(0.5f);

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => currencyCounter.HasEnoughCurrency(150) == true);
        scriptedSequence.Clear();

        Time.timeScale = 0.0f;

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSecondsRealtime(0.5f);

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSecondsRealtime(0.5f);

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSecondsRealtime(0.5f);

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitUntil(() => firstUpgradeDone == true);

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSecondsRealtime(0.5f);

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSecondsRealtime(1.0f);

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSecondsRealtime(1.0f);

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSecondsRealtime(0.5f);

        Time.timeScale = 1.0f;

        yield return new WaitUntil(() => wavesCounter > 1);

        //Second Wave (2/2)

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitUntil(() => wavesCounter > 2);
        scriptedSequence.Clear();

        //Last Wave
        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSecondsRealtime(4.0f);

        //Black Image animation


        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSecondsRealtime(0.5f);



        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSecondsRealtime(0.25f);

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSecondsRealtime(0.25f);

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSecondsRealtime(0.25f);

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSecondsRealtime(0.25f);

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSecondsRealtime(0.25f);

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSecondsRealtime(0.25f);

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSecondsRealtime(0.25f);

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSecondsRealtime(0.25f);

        //Random Bullshit Text
        scriptedSequence.NextLine();
        yield return new WaitForSecondsRealtime(1.5f);

        scriptedSequence.NextLine();
        yield return new WaitForSecondsRealtime(1.5f);

        scriptedSequence.NextLine();
        yield return new WaitForSecondsRealtime(1.5f);

        scriptedSequence.NextLine();
        yield return new WaitForSecondsRealtime(1.0f);

        scriptedSequence.NextLine();
        yield return new WaitForSecondsRealtime(1.0f);

        scriptedSequence.NextLine();
        yield return new WaitForSecondsRealtime(1.0f);

        scriptedSequence.NextLine();
        yield return new WaitForSecondsRealtime(0.75f);

        scriptedSequence.NextLine();
        yield return new WaitForSecondsRealtime(0.75f);

        scriptedSequence.NextLine();
        yield return new WaitForSecondsRealtime(0.75f);

        scriptedSequence.NextLine();
        yield return new WaitForSecondsRealtime(0.75f);

        scriptedSequence.NextLine();
        yield return new WaitForSecondsRealtime(0.75f);

        scriptedSequence.NextLine();
        yield return new WaitForSecondsRealtime(0.75f);

        scriptedSequence.NextLine();
        yield return new WaitForSecondsRealtime(3.5f);

        //Finish scene and load next
        TutorialsSaverLoader.GetInstance().SetTutorialDone(Tutorials.BATTLE);
        tDGameManager.ForceFinishScene();


    }

}
