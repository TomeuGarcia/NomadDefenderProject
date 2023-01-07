using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Linq;
using UnityEngine.TextCore.Text;

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

            if(indexLine < textStrings.Count)
            {
                indexLine = 0;
            }
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
        
        char charToReplace = ' ';
        if (respectSpaces)
        {
            for (int i = realText.IndexOf(charToReplace, 0); i > (-1); i = realText.IndexOf(charToReplace, i + 1)) // for loop end when i=-1 ('a' not found)
            {
                randomString = randomString.Remove(i, 1).Insert(i, charToReplace.ToString());
                Debug.Log(randomString);
            }
        }

        textComponent.text = realText.Substring(0, realTextIndex) + randomString.Substring(realTextIndex);
    }

    public void NextLine()
    {
        nextLine = true;
    }
}
