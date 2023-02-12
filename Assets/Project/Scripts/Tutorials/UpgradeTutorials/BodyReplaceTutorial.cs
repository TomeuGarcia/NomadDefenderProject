using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyReplaceTutorial : MonoBehaviour
{
    [SerializeField] private GameObject cards;
    [SerializeField] private GameObject bodies;
    [SerializeField] private GameObject mask1;
    [SerializeField] private GameObject mask2;
    [SerializeField] private GameObject mask3;
    [SerializeField] private CardPartReplaceManager cardPartReplaceManager;
    [SerializeField] private ScriptedSequence scriptedSequence;
    [SerializeField] Tutorials tutoType;
    private bool buttonPressed;
    // Start is called before the first frame update
    void Start()
    {
        cards.GetComponent<CanvasGroup>().alpha = 0;
        cards.SetActive(false);
        bodies.GetComponent<CanvasGroup>().alpha = 0;
        bodies.SetActive(false);
        mask1.SetActive(false);
        mask2.SetActive(false);
        if (!TutorialsSaverLoader.GetInstance().IsTutorialDone(tutoType))
        {
            StartCoroutine(Tutorial());

        }
    }

    IEnumerator Tutorial()
    {
        yield return null;

        yield return new WaitForSeconds(1.0f);

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSeconds(1.0f);

        //load cards
        scriptedSequence.NextLine();
        yield return new WaitForSeconds(1.0f);
        cards.SetActive(true);
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        for (float i = 0.0f; i < 1; i += Time.deltaTime)
        {
            cards.GetComponent<CanvasGroup>().alpha = i;
            yield return null;
        }
        mask1.SetActive(true);
        scriptedSequence.NextLine();
        yield return new WaitUntil(() => cardPartReplaceManager.GetCardIsReady() == true); 
        mask1.SetActive(false);
        scriptedSequence.Clear();
            //load bodies
            scriptedSequence.NextLine();
        yield return new WaitForSeconds(1.0f);
        bodies.SetActive(true);
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        for (float i = 0.0f; i < 1; i += Time.deltaTime)
        {
            bodies.GetComponent<CanvasGroup>().alpha = i;
            yield return null;
        }

        mask2.SetActive(true);
        scriptedSequence.NextLine();
        yield return new WaitUntil(() => cardPartReplaceManager.GetPartIsReady()); 
        mask2.SetActive(false);
        yield return null;

        scriptedSequence.Clear();


        mask3.SetActive(true);
        scriptedSequence.NextLine();
        yield return new WaitUntil(() => cardPartReplaceManager.GetReplacementDone() == true); 
        mask3.SetActive(false);
        yield return null;

        scriptedSequence.Clear();


        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSeconds(1.0f);

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSeconds(1.0f);

        TutorialsSaverLoader.GetInstance().SetTutorialDone(tutoType);
    }

}
