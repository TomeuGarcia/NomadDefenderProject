using System.Collections;
using UnityEngine;

[System.Serializable]
public class BossDialogue
{
    [SerializeField, Min(0f)] private float _startDelay = 2.0f;
    [SerializeField, Min(0f)] private float _betweenLinesDelay = 1.0f;
    [SerializeField, Min(0f)] private float _finishDelay = 2.0f;
    [SerializeField] private TextLine[] _textLines;
    

    public IEnumerator PlayDialogues(ConsoleDialogSystem dialogueSystem)
    {
        if (_textLines.Length < 1)
        {
            yield break;
        }

        yield return new WaitForSeconds(_startDelay);
        
        dialogueSystem.Clear();        
        for (int i = 0; i < _textLines.Length; ++i)
        {
            dialogueSystem.PrintLine(_textLines[i]);
            yield return new WaitUntil(() => dialogueSystem.IsLinePrinted());
            yield return new WaitForSeconds(_betweenLinesDelay);
        }
        yield return new WaitForSeconds(_finishDelay);
        
        dialogueSystem.Clear();
    }
}