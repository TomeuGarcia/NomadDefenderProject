using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TextLine
{
    public TextTypes textType;
    public string text;
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

    private void InitVariables()
    {
        currentLine = 0;
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
