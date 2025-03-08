using System.Collections;
using UnityEngine;

public class FinalGameVictoryDialogues : MonoBehaviour, IGameVictoryDialogue
{
    [System.Serializable]
    private class Dialogue
    {
        [System.Serializable]
        private class ExpandedTextLine
        {
            [SerializeField] private bool _clearBeforeStartDelay = false;
            [SerializeField, Min(0)] private float _startDelay = 0f;
            [SerializeField, Min(0)] private float _endDelay = 1.0f;
            [SerializeField] private TextLine _textLine;

            public IEnumerator PrintLine(ConsoleDialogSystem dialogSystem)
            {
                if (_clearBeforeStartDelay)
                {
                    dialogSystem.Clear();
                }
                
                yield return new WaitForSeconds(_startDelay);
                
                dialogSystem.PrintLine(_textLine);
                yield return new WaitUntil(() => dialogSystem.IsLinePrinted());
                
                yield return new WaitForSeconds(_endDelay);
            }
            
        }

        
        
        [SerializeField] private ExpandedTextLine[] _textLines;

        public IEnumerator PrintDialogue(MonoBehaviour source, ConsoleDialogSystem dialogSystem)
        {
            foreach (ExpandedTextLine textLine in _textLines)
            {
                yield return source.StartCoroutine(textLine.PrintLine(dialogSystem));
            }
            dialogSystem.Clear();
        }

    }


    [Header("DIALOGUE SYSTEM")] 
    [SerializeField] private ConsoleDialogSystem _dialogSystem;

    [Header("EASY & NORMAL")]
    [SerializeField] private Dialogue _normalAndEasyDialogue;
    
    [Header("HARD")]
    [SerializeField] private Dialogue _hardDialogueFirstTime;
    [SerializeField] private Dialogue _hardDialogueMultipleBeforeFullPerfect;
    [SerializeField] private Dialogue _hardDialogueMultipleAfterFullPerfect;
    
    [Header("HARD FULL PERFECT")]
    [SerializeField] private Dialogue _hardFullPerfectDialogue;


    private Dialogue _dialogueToPrint;
    

    public void Setup(GameDifficultyType gameDifficulty, int winCountHardDifficulty, 
        bool fullPerfectDefense, bool everBeatFullPerfectDefenseBefore)
    {
        if (gameDifficulty <= GameDifficultyType.Normal)
        {
            _dialogueToPrint = _normalAndEasyDialogue;
            return;
        }

        if (gameDifficulty != GameDifficultyType.Hard)
        {
            return;
        }

        if (fullPerfectDefense)
        {
            _dialogueToPrint = _hardFullPerfectDialogue;
            return;
        }

        if (winCountHardDifficulty <= 0)
        {
            _dialogueToPrint = _hardDialogueFirstTime;
        }
        else
        {
            if (everBeatFullPerfectDefenseBefore)
            {
                _dialogueToPrint = _hardDialogueMultipleAfterFullPerfect;
            }
            else
            {
                _dialogueToPrint = _hardDialogueMultipleBeforeFullPerfect;
            }
        }
    }
    
    
    public IEnumerator PlayVictoryDialogue()
    {
        yield return StartCoroutine(_dialogueToPrint.PrintDialogue(this, _dialogSystem));
    }
}