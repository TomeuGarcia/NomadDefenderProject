using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleTutorialManager2 : MonoBehaviour
{
    //Needs
    //CurrencyBackgroundImg (can set alpha to 0)
    [SerializeField] private HandBuildingCards hand;

    [SerializeField] private CardDrawer cardDrawer;

    [SerializeField] private PathLocation firstBase;

    [SerializeField] private TurretCardParts watcherCardScriptableObject;

    [SerializeField] private GameObject obstacleTile;

    [SerializeField] private DeckCreator deckCreator;

    [SerializeField] private Tile watcherCardTile;
    [SerializeField] private BuildingPlacer buildingPlacer;

    [SerializeField] private CurrencyCounter currencyCounter;


    //Speed Up Button (can set alpha to 0)
    [SerializeField] private GameObject speedUpButton;

    //Card Drawer -> Canvas -> BackgroundImage (can set alpha to 0)
    [SerializeField] private GameObject redrawInterface;


    //Get Scripted Sequence
    [SerializeField] private ScriptedSequence scriptedSequence;


    [SerializeField] private ParticleSystem turretSpawnParticles;
    
    private bool waveStarted = false;
    private int wavesCounter = 0;


    // Start is called before the first frame update

    private void Awake()
    {
        //cardDrawer.displayRedrawsOnGameStart = false;
    }
    void Start()
    {
        speedUpButton.GetComponent<CanvasGroup>().alpha = 0;
        speedUpButton.SetActive(false);


        //redrawInterface.GetComponent<CanvasGroup>().alpha = 0.0f;
        //redrawInterface.SetActive(false);

        watcherCardTile.gameObject.SetActive(false);

        //Make cards no interactable
        SetCardsNonInteractable();


        StartCoroutine(Tutorial());
        hand.cheatDrawCardActivated = false;
        HandBuildingCards.OnCardPlayed += WaveStarted;
        EnemyWaveManager.OnWaveFinished += WaveStarted;

    }

    private void SetCardsInteractable()
    {
        foreach(BuildingCard card in hand.GetCards())
        {
            card.isInteractable= true;
        }
    }

    private void SetCardsNonInteractable()
    {
        foreach (BuildingCard card in hand.GetCards())
        {
            card.isInteractable = false;
        }
    }
    
    private void WaveStarted()
    {
        HandBuildingCards.OnCardPlayed -= WaveStarted;
        waveStarted = true;
        wavesCounter++;
    }


    IEnumerator Tutorial()
    {
        yield return new WaitForSeconds(2.0f);

        scriptedSequence.NextLine(); //0 -> Redraw Initializing
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(1.0f);

        scriptedSequence.NextLine(); //1 -> Activating...
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(1.0f);

        //Activate redraw

        //redrawInterface.SetActive(true);
        //redrawInterface.GetComponent<CanvasGroup>().DOFade(1.0f, 0.25f);
        yield return new WaitForSeconds(0.5f);
        cardDrawer.StartRedrawButtonAnimation();
        yield return new WaitForSeconds(2.0f);



        //Redraw shown

        scriptedSequence.NextLine(); //2 -> Successfully showing
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        //yield return new WaitUntil(() => redrawInterface.GetComponent<CanvasGroup>().alpha >= 0.95f);
        yield return new WaitForSeconds(1.5f);

        scriptedSequence.Clear();

        scriptedSequence.NextLine(); //3 -> Generating info...
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(1.0f);

        scriptedSequence.NextLine(); //4 -> Click a Card to redraw it
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(2.5f);

        scriptedSequence.NextLine(); //5 -> Redraw ends when no redraws left or finish button clicked
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(2.5f);

        scriptedSequence.NextLine(); //6 -> /Redraw> Waiting to finish Redraw...
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted()); //Wait until button clicked
        yield return new WaitUntil(() => !hand.isInRedrawPhase); //Wait until button clicked

        //Make cards interactable
        SetCardsInteractable();



        scriptedSequence.Clear();


        scriptedSequence.NextLine(); //7 -> /Redraw> Finished
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(2.5f);
        scriptedSequence.Clear();
        
        

        
        yield return new WaitUntil(() => waveStarted);
        scriptedSequence.NextLine(); //8 -> Initializing Enemy Waves
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(1.0f);
        scriptedSequence.NextLine(); //9 -> Wave 1 / 3
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());

        //After losing 1 base!!
        yield return new WaitUntil(() => firstBase.IsDead);
        //Slow time with a lerp


        for(float i = 0; i < 1.0f; i+= 0.005f)
        {
            GameTime.SetTimeScale(Mathf.Lerp(1.0f, 0.0f, i));
            yield return null;
        }

        GameTime.SetTimeScale(0.0f);

        scriptedSequence.NextLine(); //10 -> I see you are struggling
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSecondsRealtime(1.5f);

        scriptedSequence.NextLine(); //11 -> One of the nodes got destroyed
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSecondsRealtime(1.5f);

        scriptedSequence.NextLine(); //12 -> Let me help you with this gift
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSecondsRealtime(1.5f);

        //Resume time with a lerp

        for (float i = 0; i < 1.0f; i += 0.005f)
        {
            GameTime.SetTimeScale(Mathf.Lerp(0.0f, 1.0f, i));
            yield return null;
        }

        GameTime.SetTimeScale(1.0f);

        //Create new turret

        TurretBuildingCard card = deckCreator.GetUninitializedNewTurretCard();
        card.ResetParts(watcherCardScriptableObject);

        //Place new Turret
        obstacleTile.SetActive(false);
        watcherCardTile.gameObject.SetActive(true);

        yield return new WaitForSecondsRealtime(0.5f);

        card.CreateCopyBuildingPrefab(this.transform, currencyCounter);

        turretSpawnParticles.Play();

        GameAudioManager.GetInstance().PlayWatcherCard();
        yield return new WaitForSeconds(1.0f);

        PlaceSelectedBuilding(watcherCardTile, card);
        
        //Wait until Wathcer's turret is placed
        yield return new WaitForSecondsRealtime(0.25f);

        scriptedSequence.NextLine(); //13 -> Don't get used to it
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());


        //TODO YEAH YEAH
        
        yield return new WaitUntil(() => wavesCounter > 1);
        scriptedSequence.Clear();
        scriptedSequence.NextLine(); //14 -> Wave 2 / 3
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        
        yield return new WaitUntil(() => wavesCounter > 2);
        scriptedSequence.Clear();
        scriptedSequence.NextLine(); //15 -> Wave 3 / 3
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());

    }

    private void PlaceSelectedBuilding(Tile tile, BuildingCard selectedBuildingCard)
    {
        /*
        tile.isOccupied = true;

        selectedBuildingCard.copyBuildingPrefab.SetActive(true);
        selectedBuildingCard.copyBuildingPrefab.transform.position = tile.buildingPlacePosition;

        Building selectedBuilding = selectedBuildingCard.copyBuildingPrefab.GetComponent<Building>();
        selectedBuilding.ShowRangePlane();

        selectedBuilding.GotPlaced();
        */

        Building selectedBuilding = selectedBuildingCard.copyBuildingPrefab.GetComponent<Building>();
        buildingPlacer.PlaceTutorialBuilding(selectedBuildingCard, selectedBuilding, tile);


        //Maybe play a special sound
    }
}
