using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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
    //List<Cards>cards -> only show cards[0] -> dont show cards[0] stats
    //Maybe create a Card object to do the animation

    //Get Scripted Sequence
    [SerializeField] private ScriptedSequence scriptedSequence;

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

        StartCoroutine(Tutorial());
    }

    // Update is called once per frame
    void Update()
    {
        
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

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);

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

        //Finishes Deck Tutorial

        //scriptedSequence.Clear();
    }

}
