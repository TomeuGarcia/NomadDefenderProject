using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Linq;
using UnityEngine.TextCore.Text;
using Unity.VisualScripting;
using UnityEngine.UI;

public class TextDecoder : MonoBehaviour
{
    private bool nextLine;
    private TMP_Text textComponent;

    private int indexLine;              //number of lines decoded
    private int indexChar;              //number of chars already decoded

    private static System.Random random = new System.Random();

    private string decodingDictionary;  //the chars that can be seen when decoding
    private static string lowerCase_s = "abcdefghijklmnopqrstuvwxyz";
    private static string upperCase_s = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private static string digits_s = "0123456789";
    private static string symbols_s = "#@$^*?~&";

    [Header("GENERAL PARAMETERS")]
    [SerializeField] private float updateDecodeTime;
    [SerializeField] private float updateCharIndexTime;
    [SerializeField] private bool respectSpaces;
    [SerializeField] private bool cutCodedText;
    [SerializeField] private int forwardRevealAmount;

    [Header("DECODING DICTIONERIES")]
    [SerializeField] private bool lowerCase;
    [SerializeField] private bool upperCase;
    [SerializeField] private bool digits;
    [SerializeField] private bool symbols;

    [Header("STRINGS")]
    [SerializeField] private List<string> textStrings;

    private void OnEnable()
    {
        InitDecodingVariables();

        StartCoroutine(Decode());
    }

    private void InitDecodingVariables()
    {
        nextLine = false;
        textComponent = gameObject.GetComponent<TMP_Text>();

        indexLine = 0;
        indexChar = 0;

        textStrings.Insert(0, textComponent.text);

        UpdateDecodingChars();
    }

    private void UpdateDecodingChars()
    {
        decodingDictionary = "";

        if (lowerCase)
            decodingDictionary += lowerCase_s;
        if (upperCase)
            decodingDictionary += upperCase_s;
        if (digits)
            decodingDictionary += digits_s;
        if (symbols)
            decodingDictionary += symbols_s;
    }

    private IEnumerator Decode()
    {
        while (indexLine < textStrings.Count)
        {
            yield return StartCoroutine(DecodeString(indexLine));
            yield return new WaitForSeconds(1.5f);
            indexLine++;
            indexChar = 0;
        }
    }

    private IEnumerator DecodeString(int currentIndexLine)
    {
        StartCoroutine(UpdateCharIndex(currentIndexLine));

        while (indexChar <= textStrings[currentIndexLine].Length)
        {
            DecodeUpdate(textStrings[currentIndexLine], decodingDictionary, indexChar);
            yield return new WaitForSeconds(updateDecodeTime);
        }

        StopCoroutine(UpdateCharIndex(currentIndexLine));
    }

    IEnumerator UpdateCharIndex(int currentIndexLine)
    {
        while (indexChar <= textStrings[currentIndexLine].Length)
        {
            indexChar++;
            yield return new WaitForSeconds(updateCharIndexTime);
        }
    }

    private void DecodeUpdate(string realText, string charDictionary, int realTextIndex)
    {
        string randomString = new string(Enumerable.Repeat(charDictionary, realText.Length).Select(d => d[random.Next(d.Length)]).ToArray());

        if (realText.Contains('\n'))
            randomString = RespectOriginalChar('\n', realText, randomString);
        if (respectSpaces && realText.Contains(' '))
            randomString = RespectOriginalChar(' ', realText, randomString);

        if (cutCodedText)
            textComponent.text = realText.Substring(0, realTextIndex) + randomString.Substring(realTextIndex, Math.Clamp(forwardRevealAmount, 0, randomString.Substring(realTextIndex).Length));
        else
            textComponent.text = realText.Substring(0, realTextIndex) + randomString.Substring(realTextIndex);
    }

    private string RespectOriginalChar(char charToRespect, string realText, string stringToModify)
    {
        for (int i = realText.IndexOf(charToRespect, 0); i > (-1); i = realText.IndexOf(charToRespect, i + 1)) // for loop end when i=-1 ('a' not found)
        {
            stringToModify = stringToModify.Remove(i, 1).Insert(i, charToRespect.ToString());
        }

        return stringToModify;
    }

    public void NextLine()
    {
        nextLine = true;
    }
}
