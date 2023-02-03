using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Linq;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using DG.Tweening;

public class TextDecoder : MonoBehaviour
{
    private bool doneDecoding;

    private bool nextLine;

    private int indexLine;              //number of lines decoded
    private int indexChar;              //number of chars already decoded

    private static System.Random random = new System.Random();

    private string decodingDictionary;  //the chars that can be seen when decoding
    private static string lowerCase_s = "abcdefghijklmnopqrstuvwxyz";
    private static string upperCase_s = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private static string digits_s = "0123456789";
    private static string symbols_s = "#@$^*?~&";

    private TextTypes textType = 0;

    [Header("PARAMETERS")]
    public DecodingParameters decodingParameters;

    [Header("STRINGS")]
    [SerializeField] public List<string> textStrings;

    [Header("COMPONENTS")]
    [SerializeField] public TMP_Text textComponent;

    private void Awake()
    {
        InitDecodingVariables();
        if(textStrings != null && textComponent != null)
        {
            textStrings.Insert(0, textComponent.text);
        }
    }

    public void Activate(TextTypes newTextType)
    {
        textType = newTextType;
        Activate();
    }

    public void Activate()
    {
        doneDecoding = false;
        StartCoroutine(Decode());
        textComponent.enabled = true;
    }

    private void InitDecodingVariables()
    {
        nextLine = false;
        if(textComponent != null)
        {
            textComponent.enabled = false;
        }

        indexLine = 0;
        indexChar = 0;

        if(decodingParameters != null)
        {
            UpdateDecodingChars();
        }
    }

    private void UpdateDecodingChars()
    {
        decodingDictionary = "";

        if (decodingParameters.lowerCase)
            decodingDictionary += lowerCase_s;
        if (decodingParameters.upperCase)
            decodingDictionary += upperCase_s;
        if (decodingParameters.digits)
            decodingDictionary += digits_s;
        if (decodingParameters.symbols)
            decodingDictionary += symbols_s;
    }

    private IEnumerator Decode()
    {
        nextLine = true;
        while (indexLine < textStrings.Count)
        {
            if(!nextLine)
                yield return new WaitUntil(() => nextLine == true);
            nextLine = false;

            yield return StartCoroutine(DecodeString(indexLine));
            indexLine++;
        }

        doneDecoding = true;
    }

    private IEnumerator DecodeString(int currentIndexLine)
    {
        StartCoroutine(UpdateCharIndex(currentIndexLine));

        while (indexChar <= textStrings[currentIndexLine].Length)
        {
            DecodeUpdate(textStrings[currentIndexLine], decodingDictionary, indexChar);
            yield return new WaitForSeconds(decodingParameters.updateDecodeTime);
        }

        DecodeUpdate(textStrings[currentIndexLine], decodingDictionary, textStrings[currentIndexLine].Length);
        StopCoroutine(UpdateCharIndex(currentIndexLine));
    }

    IEnumerator UpdateCharIndex(int currentIndexLine)
    {
        //skip to startDecodingIndex
        indexChar = decodingParameters.startDecodingIndex;

        while (indexChar <= textStrings[currentIndexLine].Length)
        {
            //skip all the tags
            if (decodingParameters.ignoreTags && textStrings[currentIndexLine][indexChar] == '<')
            {
                do {
                    indexChar++;
                } while (textStrings[currentIndexLine][indexChar] != '>');
            }
            //

            GameAudioManager.GetInstance().PlayConsoleTyping((int)textType);

            indexChar++;
            yield return new WaitForSeconds(decodingParameters.updateCharIndexTime);
        }
    }

    private void DecodeUpdate(string realText, string charDictionary, int realTextIndex)
    {
        string randomString = new string(Enumerable.Repeat(charDictionary, realText.Length).Select(d => d[random.Next(d.Length)]).ToArray());

        if (realText.Contains('\n'))
            randomString = RespectOriginalChar('\n', realText, randomString);
        if (decodingParameters.respectSpaces && realText.Contains(' '))
            randomString = RespectOriginalChar(' ', realText, randomString);

        if (decodingParameters.cutCodedText)
            randomString = realText.Substring(0, realTextIndex) + randomString.Substring(realTextIndex, Math.Clamp(decodingParameters.forwardRevealAmount, 0, randomString.Substring(realTextIndex).Length));
        else
            randomString = realText.Substring(0, realTextIndex) + randomString.Substring(realTextIndex);

        //TODO
        //if (decodingParameters.ignoreTags && realText.Contains("</"))
        //    randomString = IgnoreTags(realText, randomString);

        textComponent.text = randomString;
    }

    private string RespectOriginalChar(char charToRespect, string realText, string stringToModify)
    {
        for (int i = realText.IndexOf(charToRespect, 0); i > (-1); i = realText.IndexOf(charToRespect, i + 1)) // for loop end when i=-1 ('a' not found)
        {
            stringToModify = stringToModify.Remove(i, 1).Insert(i, charToRespect.ToString());
        }

        return stringToModify;
    }

    private string IgnoreTags(string realText, string stringToModify) //always print the tags
    {
        char charIndicator = '<';
        for (int i = realText.IndexOf(charIndicator, 0); i > (-1); i = realText.IndexOf(charIndicator, i + 1)) // for loop end when i=-1 ('a' not found)
        {
            int localI = i;
            stringToModify = stringToModify.Remove(localI, 1).Insert(localI, realText[localI].ToString());

            do {
                localI++;
                stringToModify = stringToModify.Remove(localI, 1).Insert(localI, realText[localI].ToString());
            } while (realText[localI] != '>');
        }

        return stringToModify;
    }

    public void NextLine()
    {
        nextLine = true;
    }

    public bool IsDoneDecoding()
    {
        return doneDecoding;
    }

    public void SetDecodingParameters(DecodingParameters newDecodingParameters)
    {
        decodingParameters = newDecodingParameters;
    }

    public void SetTextComponent(TMP_Text newTextComponent)
    {
        textComponent = newTextComponent;
    }

    public void SetTextStrings(List<string> newTextStrings)
    {
        textStrings = newTextStrings;
    }

    public void SetTextStrings(string newTextStrings)
    {
        textStrings.Clear();
        textStrings.Add(newTextStrings);
    }

    public void SetStartDecodingIndex(int newStartDecodingIndex)
    {
        decodingParameters.startDecodingIndex = newStartDecodingIndex;
    }

    public void ClearText()
    {
        textComponent.text = "";
    }

    public void ResetDecoder()
    {
        textStrings.Clear();
        ClearText();
        InitDecodingVariables();
    }


    /*
    // Tomeu: I did this for Card Level
    public void SetIndexLine(int indexLine)
    {
        this.indexLine = indexLine;
        textComponent.enabled = true;
    }
    public void DecodeCurrentLine()
    {        
        StartCoroutine(DoDecodeCurrentLine());
    }
    private IEnumerator DoDecodeCurrentLine()
    {
        //doneDecoding = false;
        yield return StartCoroutine(DecodeString(indexLine));
        //doneDecoding = true;
    }
    */
}
