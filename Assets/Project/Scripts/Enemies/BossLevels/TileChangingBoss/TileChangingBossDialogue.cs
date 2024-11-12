using System.Collections;
using UnityEngine;

public class TileChangingBossDialogue : MonoBehaviour
{
    [SerializeField, Min(0f)] private float _startDelay = 2.0f;
    [SerializeField, Min(0f)] private float _betweenLinesDelay = 2.0f;
    [SerializeField, Min(0f)] private float _finishDelay = 4.0f;
    [SerializeField] private TextLine[] _textLinesBeforeAnimation;
    [SerializeField] private TextLine[] _textLinesAfterAnimation;
    
    private ConsoleDialogSystem _dialogueSystem;

    public void Init(ConsoleDialogSystem dialogueSystem)
    {
        _dialogueSystem = dialogueSystem;
    }

    public IEnumerator PlayBeforeAnimationLines()
    {
        yield return StartCoroutine(DoPlayAnimationLines(_textLinesBeforeAnimation));
    }
    public IEnumerator PlayAfterAnimationLines()
    {
        yield return StartCoroutine(DoPlayAnimationLines(_textLinesAfterAnimation));
    }

    private IEnumerator DoPlayAnimationLines(TextLine[] textLines)
    {
        if (textLines.Length < 1)
        {
            yield break;
        }

        yield return new WaitForSeconds(_startDelay);
        
        _dialogueSystem.Clear();        
        for (int i = 0; i < textLines.Length; ++i)
        {
            _dialogueSystem.PrintLine(textLines[i]);
            yield return new WaitUntil(() => _dialogueSystem.IsLinePrinted());
            yield return new WaitForSeconds(_betweenLinesDelay);
        }
        yield return new WaitForSeconds(_finishDelay);
        
        _dialogueSystem.Clear();
    }
}