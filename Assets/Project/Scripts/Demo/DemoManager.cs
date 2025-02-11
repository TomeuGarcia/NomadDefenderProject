using System;
using System.Collections;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

public class DemoManager : MonoBehaviour, DemoManager.IVictoryDialogue
{
    public interface IVictoryDialogue
    {
        IEnumerator PlayVictoryDialogue();
    }
    
    [SerializeField] private DemoManagerConfig _config;
    [SerializeField] private ScriptedSequence _victoryScriptedSequence;
    [SerializeField] private CanvasGroup _dialogueFadeCG;
    [SerializeField] private GameManager _gameManager;

    private void Awake()
    {
        _dialogueFadeCG.alpha = 0;
    }

    public void Init(OWMap_Node[][] mapNodes, OverworldMapCreator owMapCreator, OWCameraMovement cameraMovement)
    {
        if (!_config.DemoEnabled)
        {
            return;
        }

        InitWall(owMapCreator);
        InitCamera(mapNodes, cameraMovement);
        InitLockNodesInteraction(mapNodes);
    }

    private void InitWall(OverworldMapCreator owMapCreator)
    {
        Instantiate(_config.DemoWallPrefab, transform).Init(owMapCreator, _config.NodeIndex);
    }
    
    private void InitCamera(OWMap_Node[][] mapNodes, OWCameraMovement cameraMovement)
    {
        cameraMovement.UpdateMaxDragDistance(mapNodes[_config.NodeIndex][0].Position.z);
    }

    private void InitLockNodesInteraction(OWMap_Node[][] mapNodes)
    {
        // TODO: somehow make nodes not hoverable
        
        // Also remove connections
        OWMap_Node[] nodesInLevel = mapNodes[_config.NodeIndex];
        for (int nodeI = 0; nodeI < nodesInLevel.Length; ++nodeI)
        {
            OWMap_Connection[] connectionsInLevel = nodesInLevel[nodeI].GetNextLevelConnections();
            for (int connectionI = 0; connectionI < connectionsInLevel.Length; ++connectionI)
            {
                connectionsInLevel[connectionI].gameObject.SetActive(false);
            }
        }
    }


    public void CheckEndOfDemo(OWMap_Node node)
    {
        if (!_config.DemoEnabled)
        {
            return;
        }

        if (node.GetMapReferencesData().ownerlevelIndex < _config.NodeIndex)
        {
            return;
        }

        FixEndOfDemoInteractions(node);
        StartEndOfDemo();
    }

    private void FixEndOfDemoInteractions(OWMap_Node node)
    {
        OWMap_Node[] nextLevelEnabledNodes = node.GetMapReferencesData().nextLevelNodes;
        for (int i = 0; i < nextLevelEnabledNodes.Length; ++i)
        {
            //nextLevelEnabledNodes[i].GetNextLevelConnections()[0].
            nextLevelEnabledNodes[i].DisableInteraction();
        }
    }

    [Button()]
    private void StartEndOfDemo()
    {
        _gameManager.StartDemoVictory(this);
    }

    public IEnumerator PlayVictoryDialogue()
    {
        _dialogueFadeCG.DOFade(1f, 0.5f).SetEase(Ease.InOutSine);
        yield return new WaitForSeconds(1f);
        
        // 0
        _victoryScriptedSequence.NextLine();
        yield return new WaitUntil(() => _victoryScriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(3f);
        _victoryScriptedSequence.Clear();
        
        
        _dialogueFadeCG.DOFade(0f, 0.5f).SetEase(Ease.InOutSine);

        
        yield break;
        
        // 1
        _victoryScriptedSequence.NextLine();
        yield return new WaitUntil(() => _victoryScriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(3f);

        // 2
        _victoryScriptedSequence.Clear();
        _victoryScriptedSequence.NextLine();
        yield return new WaitUntil(() => _victoryScriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(1f);
        
        // 3
        _victoryScriptedSequence.NextLine();
        yield return new WaitUntil(() => _victoryScriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(3f);

        // 4
        _victoryScriptedSequence.Clear();
        _victoryScriptedSequence.NextLine();
        yield return new WaitUntil(() => _victoryScriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(1f);
        
        // 5
        _victoryScriptedSequence.NextLine();
        yield return new WaitUntil(() => _victoryScriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(3f);

        // 6
        _victoryScriptedSequence.Clear();
        _victoryScriptedSequence.NextLine();
        yield return new WaitUntil(() => _victoryScriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(1f);
        
        // 7
        _victoryScriptedSequence.NextLine();
        yield return new WaitUntil(() => _victoryScriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(3f);
    }
}
