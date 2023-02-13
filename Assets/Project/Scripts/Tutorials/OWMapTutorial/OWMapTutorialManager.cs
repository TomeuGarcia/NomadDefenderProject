using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class OWMapTutorialManager : MonoBehaviour
{

    //Get Scripted Sequence
    [SerializeField] private ScriptedSequence scriptedSequence;

    [SerializeField] private GameObject mapHolder;

    [SerializeField] private OverworldMapGameManager owMapGameManager;



    private List<OWMap_Node> battleNodes;

    private List<OWMap_Node> upgradeNodes;

    private GameObject[] nodesConnections;


    public void StartTutorial()
    {
        if (TutorialsSaverLoader.GetInstance().IsTutorialDone(Tutorials.OW_MAP))
            Destroy(this.gameObject);

        Init();
        StartCoroutine(Tutorial());
    }

    private void Init()
    {
        OWMap_Node[][] tempOWMapNodes = owMapGameManager.GetMapNodes();


        //Initializing battle Nodes and upgrade nodes
        battleNodes = new List<OWMap_Node>();
        upgradeNodes = new List<OWMap_Node>();
        upgradeNodes.Add(tempOWMapNodes[0][0]);

        for(int i = 0; i < tempOWMapNodes.Length; i++)
        {
            for(int j = 0; j < tempOWMapNodes[i].Length; j++)
            {
                if(tempOWMapNodes[i][j].GetNodeType() == NodeEnums.NodeType.BATTLE)
                {
                    battleNodes.Add(tempOWMapNodes[i][j]);
                }
                else if(tempOWMapNodes[i][j].GetNodeType() == NodeEnums.NodeType.UPGRADE)
                {
                    upgradeNodes.Add(tempOWMapNodes[i][j]);
                }
            }
        }

        SetInactiveBattleNodes();
        SetInactiveUpgradeNodes();

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

    private void SetInactiveUpgradeNodes()
    {
        foreach (OWMap_Node node in upgradeNodes)
        {
            node.PlayFadeOutAnimation();
        }
    }

    private void SetInactiveBattleNodes()
    {
        foreach (OWMap_Node node in battleNodes)
        {
            node.PlayFadeOutAnimation();
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
        yield return new WaitForSeconds(5.0f);


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

        //Show Upgrade Nodes
        foreach (OWMap_Node node in upgradeNodes)
        {
            node.PlayFadeInAnimation();
            yield return new WaitForSeconds(0.25f);
        }

        yield return new WaitForSeconds(1.0f);

        scriptedSequence.NextLine(); //Loading Battle Nodes
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSeconds(1.0f);

        //Show Upgrade Nodes
        foreach (OWMap_Node node in battleNodes)
        {
            node.PlayFadeInAnimation();
            yield return new WaitForSeconds(0.25f);
        }

        yield return new WaitForSeconds(1.0f);

        scriptedSequence.NextLine(); //Loading Nodes Connection
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSeconds(2.5f);

        SetActiveNodesConnections(true);

        yield return new WaitForSeconds(1.0f);

        scriptedSequence.NextLine(); //Map Loaded
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSeconds(1.0f);

        owMapGameManager.StartCommunicationWithNextNodes(owMapGameManager.GetCurrentNode());

        scriptedSequence.NextLine(); //Select a node to go to it
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSeconds(5.0f);

        scriptedSequence.Clear();

        //Set OW_Map Tutorial as done
        //TutorialsSaverLoader.GetInstance().SetTutorialDone(Tutorials.OW_MAP);
    }
}
