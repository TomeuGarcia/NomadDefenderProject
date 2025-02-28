using System.Collections;
using System.Collections.Generic;
using NodeEnums;
using UnityEngine;

public class ConsoleCommentary_OWMap : AOWMapLifetimeListener
{
    [System.Serializable]
    private class CommentPool
    {
        [SerializeField, Min(0)] private float _delay = 1.0f;
        [SerializeField] private TextLine[] _possibleTextLines;

        public IEnumerator PrintDialogue(ConsoleDialogSystem consoleDialog)
        {
            yield return new WaitForSeconds(_delay);
            consoleDialog.PrintLine(_possibleTextLines[Random.Range(0, _possibleTextLines.Length)]);
        }
    }


    [Header("CONSOLE")]
    [SerializeField] private ConsoleDialogSystem _consoleDialog;
    
    [Header("REACHING")]
    [SerializeField] private CommentPool _reachMidComments;
    [SerializeField] private CommentPool _reachLateComments;
    [SerializeField] private CommentPool _reachBossComments;
    
    [Header("LOST")]
    [SerializeField] private CommentPool _lostEarlyComments;
    [SerializeField] private CommentPool _lostMidComments;
    [SerializeField] private CommentPool _lostLateComments;

    private ProgressionState _previousNodeProgression;



    private void ClearTexts()
    {
        StopAllCoroutines();
        _consoleDialog.Clear();
    }

    private IEnumerator DelayedClearTexts(float delay)
    {
        yield return new WaitForSeconds(delay);
        _consoleDialog.Clear();
    }
    
    private void PrintComment(CommentPool commentPool)
    {
        StartCoroutine(commentPool.PrintDialogue(_consoleDialog));
    }
    private void PrintCommentAndClearAfter(CommentPool commentPool, float clearAfterDelay)
    {
        StartCoroutine(DoPrintCommentAndClearAfter(commentPool, clearAfterDelay));
    }
    private IEnumerator DoPrintCommentAndClearAfter(CommentPool commentPool, float clearAfterDelay)
    {
        yield return StartCoroutine(commentPool.PrintDialogue(_consoleDialog));
        StartCoroutine(DelayedClearTexts(clearAfterDelay));
    }

    public override void OnNodeSelected(OWMap_Node selectedNode)
    {
        _previousNodeProgression = selectedNode.nodeClass.progressionState;
        ClearTexts();
    }

    public override void OnBattleResultsSet(BattleStateResult.NodeBattleStateResult[] nodeResults)
    {
        bool survived = true;
        foreach (var nodeResult in nodeResults)
        {
            survived &= nodeResult.healthState != HealthState.DESTROYED;
        }
        
        if (!survived)
        {
            CheckPrintLostAtStage();
        }
    }

    public override void OnComeBackFromNodeScene(OWMap_Node currentNode, OWMap_Node firstNextNode)
    {
        if (firstNextNode == null)
        {
            return;
        }

        CheckPrintReachedProgression(currentNode, firstNextNode);
    }




    private void CheckPrintLostAtStage()
    {
        const float clearAfterDelay = 3.0f;

        if (_previousNodeProgression == ProgressionState.EARLY)
        {
            PrintCommentAndClearAfter(_lostEarlyComments, clearAfterDelay);
        }
        else if (_previousNodeProgression == ProgressionState.MID)
        {
            PrintCommentAndClearAfter(_lostMidComments, clearAfterDelay);
        }
        else if (_previousNodeProgression == ProgressionState.LATE)
        {
            PrintCommentAndClearAfter(_lostLateComments, clearAfterDelay);
        }
    }
    
    private void CheckPrintReachedProgression(OWMap_Node currentNode, OWMap_Node firstNextNode)
    {
        ProgressionState currentProgressionState = currentNode.nodeClass.progressionState;
        ProgressionState nextProgressionState = firstNextNode.nodeClass.progressionState;
        
        bool reachedMid = currentProgressionState == ProgressionState.EARLY &&
                          nextProgressionState == ProgressionState.MID;
        bool reachedLate = currentProgressionState == ProgressionState.MID &&
                           nextProgressionState == ProgressionState.LATE;
        bool reachedBoss = currentProgressionState == ProgressionState.LATE &&
                           nextProgressionState == ProgressionState.BOSS;

        
        if (reachedMid)
        {
            PrintComment(_reachMidComments);
        }
        else if (reachedLate)
        {
            PrintComment(_reachLateComments);
        }
        else if (reachedBoss)
        {
            PrintComment(_reachBossComments);
        }
    }
}
