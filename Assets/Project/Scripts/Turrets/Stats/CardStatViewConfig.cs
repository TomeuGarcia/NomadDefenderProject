using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardStatViewConfig_NAME", 
    menuName = SOAssetPaths.CARDS_STATS + "CardStatViewConfig")]
public class CardStatViewConfig : ScriptableObject
{
    [Header("PARAMETERS")]
    [SerializeField] private string _name;
    [SerializeField] private Sprite _icon;
    [SerializeField] private Color _iconColor = Color.black;
    [SerializeField] private Color _textColor = Color.black;
    

    public string Name => _name;
    public Sprite Icon => _icon;
    public Color IconColor => _iconColor;
    public Color TextColor => _textColor;
}
