using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using NodeEnums;
using UnityEngine;

public class ConsoleCommentary_OWMap : AOWMapLifetimeListener
{
    private interface ICommentPool
    {
        IEnumerator PrintDialogue(MonoBehaviour source, ConsoleDialogSystem consoleDialog);
    }
    
    [System.Serializable]
    private class CommentPool : ICommentPool
    {
        [SerializeField, Min(0)] private float _delay = 1.5f;
        [SerializeField] private TextLine[] _possibleTextLines;

        public IEnumerator PrintDialogue(MonoBehaviour source, ConsoleDialogSystem consoleDialog)
        {
            yield return new WaitUntil(consoleDialog.IsLinePrinted);
            yield return new WaitForSeconds(_delay);
            consoleDialog.PrintLine(_possibleTextLines[Random.Range(0, _possibleTextLines.Length)]);
            yield return new WaitUntil(consoleDialog.IsLinePrinted);
        }
    }

    [System.Serializable]
    private class CommentPoolProgressive : ICommentPool
    {
        [SerializeField] private CommentPool[] _progressiveComments;
        private int _nextCommentIndex = 0;
        public int SkippedAmount { get; private set; }
        
        public IEnumerator PrintDialogue(MonoBehaviour source, ConsoleDialogSystem consoleDialog)
        {
            if (_progressiveComments.Length <= _nextCommentIndex)
            {
                yield break;
            }

            yield return new WaitUntil(consoleDialog.IsLinePrinted);
            source.StartCoroutine(_progressiveComments[_nextCommentIndex++].PrintDialogue(source, consoleDialog));
        }

        public void SkipNext()
        {
            ++_nextCommentIndex;
            IncrementSkipAmount();
        }
        public void IncrementSkipAmount()
        {
            ++SkippedAmount;
        }
    }


    [Header("CONSOLE")]
    [SerializeField] private ConsoleDialogSystem _consoleDialog;
    [SerializeField] private RunState _runState;
    
    [SerializeField] [Foldout("REACHING")] private CommentPool _reachMidComments;
    [SerializeField] [Foldout("REACHING")] private CommentPool _reachLateComments;
    [SerializeField] [Foldout("REACHING")] private CommentPool _reachBossComments;
    
    [SerializeField] [Foldout("LOST")] private CommentPool _lostEarlyComments;
    [SerializeField] [Foldout("LOST")] private CommentPool _lostMidComments;
    [SerializeField] [Foldout("LOST")] private CommentPool _lostLateComments;
    [SerializeField] [Foldout("LOST")] private CommentPool _lostBossComments;

    [SerializeField] [Foldout("PERFECT")] private CommentPoolProgressive _perfectDefenseComments;

    private ProgressionState _previousNodeProgression;

    private Queue<ICommentPool> _queuedComments = new Queue<ICommentPool>(2);
    private bool _processingQueuedComments;

    private void ClearTexts()
    {
        StopAllCoroutines();
        _consoleDialog.Clear();
        _queuedComments.Clear();
    }

    private IEnumerator DelayedClearTexts(float delay)
    {
        yield return new WaitForSeconds(delay);
        _consoleDialog.Clear();
    }
    
    private void QueuePrintComment(ICommentPool commentPool)
    {
        _queuedComments.Enqueue(commentPool);
        if (!_processingQueuedComments)
        {
            StartCoroutine(ProcessQueuedComments());
        }   
    }

    private IEnumerator ProcessQueuedComments()
    {
        _processingQueuedComments = true;
        while (_queuedComments.Count > 0)
        {
            ICommentPool commentPool = _queuedComments.Dequeue();
            yield return StartCoroutine(commentPool.PrintDialogue(this, _consoleDialog));
            yield return new WaitForSeconds(4f);
        }
        _processingQueuedComments = false;
    }

    private void PrintCommentAndClearAfter(ICommentPool commentPool, float clearAfterDelay)
    {
        StartCoroutine(DoPrintCommentAndClearAfter(commentPool, clearAfterDelay));
    }
    private IEnumerator DoPrintCommentAndClearAfter(ICommentPool commentPool, float clearAfterDelay)
    {
        yield return StartCoroutine(commentPool.PrintDialogue(this, _consoleDialog));
        StartCoroutine(DelayedClearTexts(clearAfterDelay));
    }

    
    public override void OnNodeSelected(OWMap_Node selectedNode)
    {
        _previousNodeProgression = selectedNode.nodeClass.progressionState;
        ClearTexts();
    }

    public override void OnBattleResultsSet(BattleStateResult.NodeBattleStateResult[] nodeResults)
    {
        bool survived = false;
        foreach (var nodeResult in nodeResults)
        {
            survived |= nodeResult.healthState != HealthState.DESTROYED;
        }
        
        if (!survived)
        {
            CheckPrintLostAtStage();
        }
    }

    public override void OnComeBackFromNodeScene(OWMap_Node currentNode, OWMap_Node firstNextNode, bool cameFromBattle)
    {
        if (firstNextNode == null || firstNextNode.healthState == HealthState.DESTROYED)
        {
            return;
        }
        
        if (cameFromBattle)
        {
            CheckPrintPerfectDefense();
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
        else if (_previousNodeProgression == ProgressionState.BOSS)
        {
            PrintCommentAndClearAfter(_lostBossComments, clearAfterDelay);
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
            QueuePrintComment(_reachMidComments);
        }
        else if (reachedLate)
        {
            QueuePrintComment(_reachLateComments);
        }
        else if (reachedBoss)
        {
            QueuePrintComment(_reachBossComments);
        }
    }

    private void CheckPrintPerfectDefense()
    {
        if (_runState.BattleVictories < 2) // Skip first battle perfect
        {
            if (_runState.PerfectDefenseBattleVictories < 1) // If not perfect, increment equate
            {
                _perfectDefenseComments.IncrementSkipAmount();
            }
            
            return;
        }
        
        if (_runState.PerfectDefenseBattleVictories + _perfectDefenseComments.SkippedAmount == _runState.BattleVictories)
        {
            QueuePrintComment(_perfectDefenseComments);
        }
        else
        {
            _perfectDefenseComments.SkipNext();
        }
    }


    [Button()]
    private void Test()
    {
        _previousNodeProgression = ProgressionState.BOSS;
        CheckPrintLostAtStage();
    }
}
