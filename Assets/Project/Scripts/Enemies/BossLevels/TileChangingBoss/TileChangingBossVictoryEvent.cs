using System;
using System.Collections;
using UnityEngine;

public class TileChangingBossVictoryEvent : MonoBehaviour
{
    [SerializeField] private BossDialogue _victoryDialogue;


    public IEnumerator PlayVictory(ConsoleDialogSystem dialogueSystem)
    {
        yield return StartCoroutine(_victoryDialogue.PlayDialogues(dialogueSystem));
    }
}