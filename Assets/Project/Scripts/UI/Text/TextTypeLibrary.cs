using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum TextTypes { SYSTEM, WATCHER, DRAMATICWATCHER, INSTRUCTION, INFO };

[System.Serializable]
public class TextType
{
    public string name;
    public string context;
    public Color32 color;
    public DecodingParameters decodingParameters;

    [HideInInspector] public string openingContextColor;
    [HideInInspector] public string closeingContextColor = "</color>";
};

[CreateAssetMenu(fileName = "TextTypeLibrary", menuName = "UI/Text/TextTypeLibrary")]
public class TextTypeLibrary : ScriptableObject
{
    public List<TextType> textTypes;

    public TextType GetTextType(TextTypes demandedTextType)
    {
        return textTypes[(int)demandedTextType];
    }

    private void OnValidate()
    {
        Init();
    } 
    public void Init()
    {
        foreach(TextType tType in textTypes)
        {
            tType.openingContextColor = "<color=#" + ColorUtility.ToHtmlStringRGB(tType.color) + ">";
        }
    }
}

