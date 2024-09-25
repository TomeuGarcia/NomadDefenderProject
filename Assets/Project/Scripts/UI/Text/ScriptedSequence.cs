using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TextLine
{
    public TextTypes textType;
    public string text;
    [Tooltip("If set to 0, this variable will have no effect")]
    public float clearTime;
    //EVENT TO GO FOR THE NEXT LINE?
}

public class ScriptedSequence : MonoBehaviour
{
    public ConsoleDialogSystem dialogSystem;
    public TextLine[] textLines;

    private int currentLine;

    private void Awake()
    {
        InitVariables();
    }

    public void ResetDialog()
    {
        InitVariables();
    }
    
    private void InitVariables()
    {
        currentLine = 0;
    }

    public void SkipLine()
    {
        currentLine++;
    }
    public void NextLine()
    {
        dialogSystem.PrintLine(textLines[currentLine]);
        currentLine++;
    }

    public void Clear()
    {
        dialogSystem.Clear();
    }

    public bool IsLinePrinted()
    {
        return dialogSystem.IsLinePrinted();
    }

    public int GetLineCount()
    {
        return textLines.Length;
    }
}
