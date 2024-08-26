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
    
    private string DescriptionVariablePlural => _descriptionVariable + 's';

    public string Name => _name;
    public string Description => _description;


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
        return 
            "<color=#" + ColorUtility.ToHtmlStringRGB(_descriptionVariableColor) + ">" +
            (plural ? _namePlural : _name) +
            "</color>";
    }
    
}