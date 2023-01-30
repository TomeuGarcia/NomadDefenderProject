using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum TextTypes { SYSTEM, WATCHER, SHOPKEEPER };

[System.Serializable]
public class TextType
{
    public string name;
    public string context;
    public Color32 color;
    public DecodingParameters decodingParameters;
};

[CreateAssetMenu(fileName = "TextTypeLibrary", menuName = "UI/Text/TextTypeLibrary")]
public class TextTypeLibrary : ScriptableObject
{
    public List<TextType> textTypes;

    public TextType GetTextType(TextTypes demandedTextType)
    {
        return textTypes[(int)demandedTextType];
    }
}

