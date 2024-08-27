using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "AbilityKeyword_NAME", 
    menuName = SOAssetPaths.CARDS + "CardAbilityKeyword")]
public class CardAbilityKeyword : ScriptableObject
{
    [Header("CONFIG")] 
    [Header("Name")] 
    [SerializeField] private bool _hasPlural = false;
    [SerializeField] private string _name = "Name";
    [ShowIf("_hasPlural"), SerializeField] private string _namePlural = "";
    
    [Header("Description")]
    [SerializeField, TextArea(2, 5)] private string _description = "Description text.";
    
    
    [Header("Card Tooltip")]
    [SerializeField] private string _descriptionVariable = "KEYWORD_NAME";
    [SerializeField] private Color _descriptionVariableColor = Color.cyan;
    [SerializeField] private bool _descriptionVariableBold = true;
    
    private string DescriptionVariablePlural => _descriptionVariable + 's';

    public string Name => _name;
    public string Description => _description;
    public Color NameColor => _descriptionVariableColor;


    public void ApplyDescriptionModifications(EditableCardAbilityDescription abilityDescription)
    {
        Dictionary<string, string> variableToKeyword = new Dictionary<string, string>()
        {
            { _descriptionVariable, NameForDescription(false) }
        };

        if (_hasPlural)
        {
            variableToKeyword.Add(DescriptionVariablePlural, NameForDescription(true));
        }
        
        abilityDescription.ApplyDescriptionModifications(variableToKeyword);
    }

    private string NameForDescription(bool plural)
    {
        string prefix = "<color=#" + ColorUtility.ToHtmlStringRGB(_descriptionVariableColor) + ">";
        string suffix = "</color>";

        if (_descriptionVariableBold)
        {
            prefix = prefix + "<b>";
            suffix = "</b>" + suffix;
        }
            
        return prefix + (plural ? _namePlural : _name) + suffix;
    }
    
}