using DG.Tweening;
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

    private List<OWMap_Connection> nodesConnections;

    private void OnEnable()
    {
        owMapGameManager.OnMapNodeSceneStartsLoading += ClearConsoleOnNodeSceneStarts;
    }
    private void OnDisable()
    {
        owMapGameManager.OnMapNodeSceneStartsLoading -= ClearConsoleOnNodeSceneStarts;
    }

    public void StartTutorial()
    {
        if (TutorialsSaverLoader.GetInstance().IsTutorialDone(Tutorials.OW_MAP))
        {
            Destroy(this.gameObject);
        }
        else
        {
            Init();
            StartCoroutine(Tutorial());
        }        
    }

    private void Init()
    {
        OWMap_Node[][] tempOWMapNodes = owMapGameManager.GetMapNodes();


        //Initializing battle Nodes and upgrade nodes
        battleNodes = new List<OWMap_Node>();
        upgradeNodes = new List<OWMap_Node>();

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

        for (int j = 0; j < tempOWMapNodes[tempOWMapNodes.Length-1].Length; j++)
        {
            upgradeNodes.Add(tempOWMapNodes[tempOWMapNodes.Length - 1][j]);
        }


        SetInactiveBattleNodes();
        SetInactiveUpgradeNodes();
        upgradeNodes[0].InvokeOnNodeInfoInteractionDisabled();


        //Initializing nodes connections
        nodesConnections = new List<OWMap_Connection>();
        for (int levelI = 0; levelI < tempOWMapNodes.Length-1; ++levelI)
        {
            for (int nodeI = 0; nodeI < tempOWMapNodes[levelI].Length; ++nodeI)
            {
                OWMap_Connection[] nextLevelConnections = tempOWMapNodes[levelI][nodeI].GetNextLevelConnections();                
                for (int connectionI = 0; connectionI < nextLevelConnections.Length; ++connectionI)
                {
                    nodesConnections.Add(nextLevelConnections[connectionI]);
                }
            }
        }
        
        foreach(OWMap_Connection connection in nodesConnections)
        {
            connection.cable.StartNodesConnectionsScaleZToZero();
        }

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
        for (int i = 0; i < upgradeNodes.Count; ++i)        
        {
            GameAudioManager.GetInstance().PlayUpgradeNodeSpawnSound();
            upgradeNodes[i].PlayFadeInAnimation();
            yield return new WaitForSeconds(0.25f);
        }

        yield return new WaitForSeconds(1.0f);

        scriptedSequence.NextLine(); //Loading Battle Nodes
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSeconds(1.0f);

        //Show Battle Nodes
        foreach (OWMap_Node node in battleNodes)
        {
            GameAudioManager.GetInstance().PlayBattleNodeSpawnSound();
            node.PlayFadeInAnimation();
            
            yield return new WaitForSeconds(0.25f);
        }

        yield return new WaitForSeconds(1.0f);

        scriptedSequence.NextLine(); //Loading Nodes Connection
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSeconds(2.5f);

        GameAudioManager.GetInstance().PlayConnectionsNodeSpawnSound();

        foreach (OWMap_Connection connection in nodesConnections)
        {
            yield return StartCoroutine(connection.cable.ShowNodesConnections(0.25f));
        }

        yield return new WaitForSeconds(1.0f);

        scriptedSequence.NextLine(); //Map Loaded
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);
        yield return new WaitForSeconds(2.0f);
        scriptedSequence.Clear();

        owMapGameManager.StartCommunicationWithNextNodes(owMapGameManager.GetCurrentNode());

        scriptedSequence.NextLine(); //Select a node to go to it
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted() == true);

        //Set OW_Map Tutorial as done
        TutorialsSaverLoader.GetInstance().SetTutorialDone(Tutorials.OW_MAP);
    }


    private void ClearConsoleOnNodeSceneStarts()
    {
        scriptedSequence.Clear();
    }
}
