using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OWMapTutorialManager : MonoBehaviour
{

    //Get Scripted Sequence
    [SerializeField] private ScriptedSequence scriptedSequence;

    [SerializeField] private GameObject mapHolder;



    private GameObject[] battleNodes;

    private GameObject[] upgradeNodes;

    private GameObject[] nodesConnections;

    // Start is called before the first frame update
    void Start()
    {
        Init();
        StartCoroutine(Tutorial());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Init()
    {
        //Initializing battle Nodes
        battleNodes = new GameObject[2];
        battleNodes[0] = mapHolder.transform.GetChild(2).transform.GetChild(0).transform.GetChild(0).gameObject;
        battleNodes[1] = mapHolder.transform.GetChild(2).transform.GetChild(0).transform.GetChild(1).gameObject;
        SetActiveBattleNodes(false);

        //Initializing upgrade nodes
        upgradeNodes = new GameObject[5];
        upgradeNodes[0] = mapHolder.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).gameObject;
        upgradeNodes[1] = mapHolder.transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).gameObject;
        upgradeNodes[2] = mapHolder.transform.GetChild(3).transform.GetChild(0).transform.GetChild(0).gameObject;
        upgradeNodes[3] = mapHolder.transform.GetChild(3).transform.GetChild(0).transform.GetChild(1).gameObject;
        upgradeNodes[4] = mapHolder.transform.GetChild(3).transform.GetChild(0).transform.GetChild(2).gameObject;

        SetActiveUpgradeNodes(false);

        //Initializing nodes connections
        nodesConnections = new GameObject[9];

        nodesConnections[0] = mapHolder.transform.GetChild(0).transform.GetChild(1).transform.GetChild(0).gameObject;
        nodesConnections[1] = mapHolder.transform.GetChild(1).transform.GetChild(1).transform.GetChild(0).gameObject;
        nodesConnections[2] = mapHolder.transform.GetChild(1).transform.GetChild(1).transform.GetChild(1).gameObject;
        nodesConnections[3] = mapHolder.transform.GetChild(2).transform.GetChild(1).transform.GetChild(0).gameObject;
        nodesConnections[4] = mapHolder.transform.GetChild(2).transform.GetChild(1).transform.GetChild(1).gameObject;
        nodesConnections[5] = mapHolder.transform.GetChild(2).transform.GetChild(1).transform.GetChild(2).gameObject;
        nodesConnections[6] = mapHolder.transform.GetChild(3).transform.GetChild(1).transform.GetChild(0).gameObject;
        nodesConnections[7] = mapHolder.transform.GetChild(3).transform.GetChild(1).transform.GetChild(1).gameObject;
        nodesConnections[8] = mapHolder.transform.GetChild(3).transform.GetChild(1).transform.GetChild(2).gameObject;
        SetActiveNodesConnections(false);

    }

    private void SetActiveUpgradeNodes(bool active)
    {
        foreach (GameObject gameObject in upgradeNodes)
        {
            gameObject.SetActive(active);
        }
    }

    private void SetActiveBattleNodes(bool active)
    {
        foreach (GameObject gameObject in battleNodes)
        {
            gameObject.SetActive(active);
        } 
    }

    private void SetActiveNodesConnections(bool active)
    {
        foreach (GameObject gameObject in nodesConnections)
        {
            gameObject.SetActive(active);
        }
    }

    IEnumerator Tutorial()
    {
        yield return new WaitForSeconds(2.0f);


        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSeconds(1.0f);

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSeconds(1.0f);

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSeconds(1.5f);

        scriptedSequence.Clear();

        yield return new WaitForSeconds(1.0f);

        scriptedSequence.NextLine(); //Rendering Map...
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSeconds(1.0f);

        scriptedSequence.NextLine(); //Loading Upgrade Nodes
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSeconds(1.0f);

        SetActiveUpgradeNodes(true);

        yield return new WaitForSeconds(1.0f);

        scriptedSequence.NextLine(); //Loading Battle Nodes
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSeconds(1.0f);

        SetActiveBattleNodes(true);

        yield return new WaitForSeconds(1.0f);

        scriptedSequence.NextLine(); //Loading Nodes Connection
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSeconds(2.5f);

        SetActiveNodesConnections(true);

        yield return new WaitForSeconds(1.0f);

        scriptedSequence.NextLine(); //Map Loaded
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSeconds(1.0f);
    }
}
