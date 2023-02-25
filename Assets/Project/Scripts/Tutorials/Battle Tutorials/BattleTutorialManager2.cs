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


    //Speed Up Button (can set alpha to 0)
    [SerializeField] private GameObject speedUpButton;

    //DeckUI (can set alpha to 0)
    [SerializeField] private GameObject deckInterface;

    //Card Drawer -> Canvas -> BackgroundImage (can set alpha to 0)
    [SerializeField] private GameObject redrawInterface;


    //Get Scripted Sequence
    [SerializeField] private ScriptedSequence scriptedSequence;


    // Start is called before the first frame update

    private void Awake()
    {
        cardDrawer.displayRedrawsOnGameStart = false;
    }
    void Start()
    {
        speedUpButton.GetComponent<CanvasGroup>().alpha = 0;
        speedUpButton.SetActive(false);


        redrawInterface.GetComponent<CanvasGroup>().alpha = 0.0f;
        redrawInterface.SetActive(false);

        //Make cards no interactable
        SetCardsNonInteractable();


        StartCoroutine(Tutorial());
        hand.cheatDrawCardActivated = false;

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

        redrawInterface.SetActive(true);
        redrawInterface.GetComponent<CanvasGroup>().DOFade(1.0f, 0.25f);
        yield return new WaitForSeconds(0.5f);
        cardDrawer.StartRedrawButtonAnimation();
        yield return new WaitForSeconds(2.0f);



        //Redraw shown

        scriptedSequence.NextLine(); //2 -> Successfully showing
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitUntil(() => redrawInterface.GetComponent<CanvasGroup>().alpha >= 0.95f);
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


        //After losing 1 base!!
        yield return new WaitUntil(() => firstBase.IsDead);
        //Slow time with a lerp


        for(float i = 0; i < 1.0f; i+= 0.005f)
        {
            Time.timeScale = Mathf.Lerp(1.0f, 0.1f, i);
            yield return null;
        }

        Time.timeScale = 0.1f;

        scriptedSequence.NextLine(); //8 -> I see you are struggling
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSecondsRealtime(1.5f);

        scriptedSequence.NextLine(); //9 -> One of the nodes got destroyed
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSecondsRealtime(1.5f);

        scriptedSequence.NextLine(); //10 -> Let me help you with this gift
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSecondsRealtime(1.5f);

        //TODO Create new turret

        //Wait until Wathcer's turret is placed

        scriptedSequence.NextLine(); //11 -> Don't get used to it
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSecondsRealtime(1.0f);

        for (float i = 0; i < 1.0f; i += 0.005f)
        {
            Time.timeScale = Mathf.Lerp(0.1f, 1.0f, i);
            yield return null;
        }

        Time.timeScale = 1.0f;
        //Resume time with a lerp

        /*
        //tutoCardDrawer.tutorialCard.isInteractable = false;
        yield return new WaitForSeconds(1.5f);


     


        scriptedSequence.NextLine();// 13
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(1.0f);

        //Finishes Cards Tutorial



        scriptedSequence.Clear();
        //tutoCardDrawer.tutorialCard.isInteractable = true;

        yield return new WaitForSeconds(1.0f);



        //Starts Deck Tutorial
        yield return new WaitUntil(() => deckInterface.activeInHierarchy);


        //Second Wave (2/2)

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        scriptedSequence.Clear();

        //Last Wave
        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSecondsRealtime(4.0f);
        */
    }
}
