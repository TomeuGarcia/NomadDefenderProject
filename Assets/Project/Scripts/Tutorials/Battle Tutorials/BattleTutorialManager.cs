using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static InBattleBuildingUpgrader;

public class BattleTutorialManager : MonoBehaviour
{

    //Needs
    //CurrencyBackgroundImg (can set alpha to 0)
    [SerializeField] private GameObject currencyBackgroundImg;

    //Speed Up Button (can set alpha to 0)
    [SerializeField] private GameObject speedUpButton;

    //DeckUI (can set alpha to 0)
    [SerializeField] private GameObject deckInterface;

    //Card Drawer -> Canvas -> BackgroundImage (can set alpha to 0)
    [SerializeField] private GameObject redrawInterface;

    //Cards
    [SerializeField] private GameObject card;
    [SerializeField] private CanvasGroup attackStat;
    [SerializeField] private CanvasGroup cadencyStat;
    [SerializeField] private CanvasGroup rangeStat;
    [SerializeField] private CanvasGroup projectile;
    [SerializeField] private CanvasGroup passive;
    [SerializeField] private CanvasGroup cost;
    [SerializeField] private CanvasGroup cardLevel;
    [SerializeField] private CanvasGroup blackImg;
    //List<Cards>cards -> only show cards[0] -> dont show cards[0] stats
    //Maybe create a Card object to do the animation

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

        card.GetComponent<CanvasGroup>().alpha = 0;
        card.SetActive(false);

        attackStat.alpha = 0;
        cadencyStat.alpha = 0;
        rangeStat.alpha = 0;
        projectile.alpha = 0;
        passive.alpha = 0;
        cost.alpha = 0;
        cardLevel.alpha = 0;
        blackImg.alpha = 1.0f;

        StartCoroutine(Tutorial());


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

        for(float i = 0.0f; i < 1.0f; i+=0.01f)
        {
            currencyBackgroundImg.GetComponent<CanvasGroup>().alpha = i;
            yield return null;
        }
        currencyBackgroundImg.GetComponent<CanvasGroup>().alpha = 1.0f;
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
        yield return new WaitForSeconds(0.5f);

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);

        card.SetActive(true);
        for (float i = 0.0f; i < 1.0f; i += 0.01f)
        {
            card.GetComponent<CanvasGroup>().alpha = i;
            yield return null;
        }
        card.GetComponent<CanvasGroup>().alpha = 1.0f;

        //Cards Stats


        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSeconds(0.5f);


        //Card Level
        for (float i = 0.0f; i < 1.0f; i += 0.01f)
        {
            cardLevel.GetComponent<CanvasGroup>().alpha = i;
            yield return null;
        }
        cardLevel.GetComponent<CanvasGroup>().alpha = 1.0f;


        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSeconds(1.0f);

        //Range
        for (float i = 0.0f; i < 1.0f; i += 0.01f)
        {
            rangeStat.GetComponent<CanvasGroup>().alpha = i;
            yield return null;
        }
        rangeStat.GetComponent<CanvasGroup>().alpha = 1.0f;

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSeconds(1.0f);

        //Cadency
        for (float i = 0.0f; i < 1.0f; i += 0.01f)
        {
            cadencyStat.GetComponent<CanvasGroup>().alpha = i;
            yield return null;
        }
        cadencyStat.GetComponent<CanvasGroup>().alpha = 1.0f;

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSeconds(1.0f);

        //Attack
        for (float i = 0.0f; i < 1.0f; i += 0.01f)
        {
            attackStat.GetComponent<CanvasGroup>().alpha = i;
            yield return null;
        }
        attackStat.GetComponent<CanvasGroup>().alpha = 1.0f;

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSeconds(1.0f);

        //Cost
        for (float i = 0.0f; i < 1.0f; i += 0.01f)
        {
            cost.GetComponent<CanvasGroup>().alpha = i;
            yield return null;
        }
        cost.GetComponent<CanvasGroup>().alpha = 1.0f;

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSeconds(1.0f);

        //Projectile
        for (float i = 0.0f; i < 1.0f; i += 0.01f)
        {
            projectile.GetComponent<CanvasGroup>().alpha = i;
            yield return null;
        }
        projectile.GetComponent<CanvasGroup>().alpha = 1.0f;

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSeconds(1.0f);

        //Passive
        for (float i = 0.0f; i < 1.0f; i += 0.01f)
        {
            passive.GetComponent<CanvasGroup>().alpha = i;
            yield return null;
        }
        passive.GetComponent<CanvasGroup>().alpha = 1.0f;

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSeconds(1.0f);



        //Erases Card
        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSeconds(1.0f);

        
        for (float i = 1.0f; i > 0.0f; i -= 0.01f)
        {
            card.GetComponent<CanvasGroup>().alpha = i;
            yield return null;
        }
        card.GetComponent<CanvasGroup>().alpha = 0.0f;
        card.SetActive(false);

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
        yield return new WaitForSeconds(0.5f);

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        //yield return new WaitForSeconds(0.5f);

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        //yield return new WaitForSeconds(0.5f);

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        //yield return new WaitForSeconds(0.5f);

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        //yield return new WaitForSeconds(0.5f);

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
