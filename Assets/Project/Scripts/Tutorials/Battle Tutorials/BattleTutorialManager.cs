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
    [SerializeField] private GameObject card;
    [SerializeField] private CanvasGroup attackStat;
    [SerializeField] private CanvasGroup cadencyStat;
    [SerializeField] private CanvasGroup rangeStat;
    [SerializeField] private CanvasGroup projectile;
    [SerializeField] private CanvasGroup passive;
    [SerializeField] private CanvasGroup cost;
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

        card.GetComponent<CanvasGroup>().alpha = 0;
        card.SetActive(false);

        attackStat.alpha = 0;
        cadencyStat.alpha = 0;
        rangeStat.alpha = 0;
        projectile.alpha = 0;
        passive.alpha = 0;
        cost.alpha = 0;

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

        scriptedSequence.NextLine(); //Line 27
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSeconds(1.0f);

        //Falta ensenyar a sel·leccionar una carta
        //Falta jugar una carta

        //Falta pausar el joc i obligar al jugador a millorar torreta.
        //Falta error final i finalitzar partida

        //Finishes Deck Tutorial

        //scriptedSequence.Clear();
    }

}
