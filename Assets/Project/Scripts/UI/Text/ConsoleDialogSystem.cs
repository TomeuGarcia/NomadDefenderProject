using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleDialogSystem : MonoBehaviour
{
    [SerializeField] TextTypeLibrary textTypeLibrary;
    public Pool textPool;   //the pool should have the prefab of the text format
    private List<GameObject> consoleLines = new List<GameObject>();

    private string openingContext;
    private string closingContext;

    [Header("PARAMETERS")]
    [SerializeField] private int maxLinesOnScreen;
    [SerializeField] private Vector3 lineSeparation;
    [SerializeField] private Vector3 initialLinePosition;
    [SerializeField] private Color32 contextColor;

    [Header("CLEAR")]
    [SerializeField] private bool clearWithTime;
    [SerializeField] private float clearTime;

    private void OnValidate()
    {
        openingContext = "<color=#" + ColorUtility.ToHtmlStringRGB(contextColor) + ">" + "|:";// + System.Convert.ToChar(92);
        closingContext = ">" + "</color>";
    }
    private void Awake()
    {
        textTypeLibrary.Init();
        openingContext = "<color=#" + ColorUtility.ToHtmlStringRGB(contextColor) + ">" + "|:";// + System.Convert.ToChar(92);
        closingContext = ">" + "</color>";
        
    }
    public void PrintLine(TextLine textLine)
    {

        //Build the whole line of text
        TextType textType = textTypeLibrary.GetTextType(textLine.textType);
        string wholeContext = openingContext + textType.openingContextColor + textType.context + textType.closeingContextColor + closingContext;
        string lineToPrint = wholeContext + " " + textLine.text;

        //Remove the upper line if needed
        if (consoleLines.Count >= maxLinesOnScreen)
        {
            RemoveLine(consoleLines.Count - 1);
            for (int i = 0; i < consoleLines.Count; i++)
            {
                consoleLines[i].GetComponent<RectTransform>().position += lineSeparation;
            }
        }

        //Make room for the new line of text
        //if (consoleLines.Count > 0)
        //{
        //    for(int i = 0; i < consoleLines.Count; i++)
        //    {
        //        consoleLines[i].GetComponent<RectTransform>().position -= lineSeparation;
        //    }
        //}

        //Get the text
        consoleLines.Insert(0, textPool.GetObject());
        consoleLines[0].SetActive(true);
        consoleLines[0].transform.SetParent(textPool.transform, false);
        consoleLines[0].GetComponent<RectTransform>().pivot = new Vector2(0f, 0.5f);
        consoleLines[0].GetComponent<RectTransform>().anchoredPosition = initialLinePosition - lineSeparation * (consoleLines.Count - 1);

        //Configure the TextDecoder
        TextDecoder decoder = consoleLines[0].GetComponent<TextDecoder>();
        DecodingParameters decodingParams = textType.decodingParameters;
        decodingParams.startDecodingIndex = wholeContext.Length;
        decoder.SetDecodingParameters(decodingParams);
        decoder.SetTextComponent(consoleLines[0].GetComponent<TMP_Text>());
        decoder.ResetDecoder();
        decoder.SetTextStrings(lineToPrint);
        decoder.Activate(textLine.textType);

        if(textLine.clearTime > 0.0f)
        {
            consoleLines[0].GetComponent<EntryTextLine>().ClearWithTime(textLine.clearTime, this);
        }
        else if(clearWithTime)
        {
            consoleLines[0].GetComponent<EntryTextLine>().ClearWithTime(clearTime, this);
        }
    }

    public void RemoveSpecificLine(EntryTextLine entryLine)
    {
        for(int i = 0; i < consoleLines.Count; i++)
        {
            if (consoleLines[i].GetComponent<EntryTextLine>() == entryLine)
            {
                int n = i + 1;
                while (n <= consoleLines.Count - 1)
                {
                    consoleLines[n].GetComponent<RectTransform>().position += lineSeparation;
                    n++;
                }

                RemoveLine(i);
                return;
            }
        }
    }

    private void RemoveLine(int lineIndex)
    {
        consoleLines[lineIndex].SetActive(false);
        consoleLines.RemoveAt(lineIndex);
    }

    public void Clear()
    {
        while (consoleLines.Count > 0)
        {
            RemoveLine(consoleLines.Count - 1);
        }
    }

    public bool IsLinePrinted()
    {
        if(consoleLines.Count > 0)
        {
            return consoleLines[0].gameObject.GetComponent<TextDecoder>().IsDoneDecoding();
        }

        return true;
    }
}
