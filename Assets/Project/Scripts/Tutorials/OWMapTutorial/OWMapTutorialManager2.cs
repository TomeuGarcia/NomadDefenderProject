using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OWMapTutorialManager2 : MonoBehaviour
{

    [SerializeField] private ScriptedSequence scriptedSequence;
    [SerializeField] private OverworldMapGameManager owMapGameManager;

    private OWMap_Node[] lastNodes;
    public void StartTutorial()
    {
            Init();
            StartCoroutine(Tutorial());
    }

    private void Init()
    {
        lastNodes = owMapGameManager.GetMapNodes()[owMapGameManager.GetMapNodes().Length - 1];
    }

    private void EnableLastNodes()
    {
        foreach(OWMap_Node node in lastNodes)
        {
            node.EnableInteraction();
        }
    }

    private void DisableLastNodes()
    {
        foreach (OWMap_Node node in lastNodes)
        {
            node.DisableInteraction();
        }
    }
    IEnumerator Tutorial()
    {
        DisableLastNodes();
        yield return new WaitForSeconds(1.0f);


        scriptedSequence.NextLine(); //0 -> Computing Battle Result...
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(3.0f);

        //Wait until node animation is done

        scriptedSequence.NextLine(); //1
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(1.5f);

        scriptedSequence.NextLine(); //2
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(1.5f);


        scriptedSequence.NextLine(); //3 -> Simulation will FAIL if no nodes are available
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(2.0f);

        scriptedSequence.Clear();
        yield return new WaitForSeconds(1.0f);

        scriptedSequence.NextLine(); //4
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(2.0f);


        scriptedSequence.NextLine(); //5
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(2.0f);


        scriptedSequence.NextLine(); //6 -> It better not happen again
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(2.5f);

        scriptedSequence.Clear();
        yield return new WaitForSeconds(1.0f);


        scriptedSequence.NextLine(); //7 -> Don't you want to get to the end of this?
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(2.0f);

        scriptedSequence.Clear();
        yield return new WaitForSeconds(1.0f);


        scriptedSequence.NextLine(); //8 -> I won't be always arround
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(1.0f);

        scriptedSequence.NextLine(); //9 -> But...
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(1.0f);

        scriptedSequence.Clear();
        yield return new WaitForSeconds(1.0f);

        scriptedSequence.NextLine(); //10 -> I
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(1.5f);

        scriptedSequence.NextLine(); //11 -> Will
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(1.5f);

        scriptedSequence.NextLine(); //12 -> Be
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(1.5f);

        scriptedSequence.NextLine(); //13 -> Watching
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(1.5f);

        scriptedSequence.Clear();
        yield return new WaitForSeconds(1.0f);

        EnableLastNodes();
    }
}
