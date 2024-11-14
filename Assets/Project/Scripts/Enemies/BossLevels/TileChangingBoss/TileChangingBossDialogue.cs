using System.Collections;
using UnityEngine;

public class TileChangingBossDialogue : MonoBehaviour
{
    [SerializeField] private BossDialogue _linesBeforeAnimation;
    [SerializeField] private BossDialogue _linesAfterAnimation;
    
    private ConsoleDialogSystem _dialogueSystem;

    public void Init(ConsoleDialogSystem dialogueSystem)
    {
        _dialogueSystem = dialogueSystem;
    }

    public IEnumerator PlayBeforeAnimationLines()
    {
        yield return StartCoroutine(_linesBeforeAnimation.PlayDialogues(_dialogueSystem));
    }
    public IEnumerator PlayAfterAnimationLines()
    {
        yield return StartCoroutine(_linesAfterAnimation.PlayDialogues(_dialogueSystem));
    }
    
}