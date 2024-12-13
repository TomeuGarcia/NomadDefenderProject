
using System;
using UnityEngine;

[System.Serializable]
public class CardAbilityDescriptionModel
{
    [SerializeField] private string _abilityName;
    [SerializeField, TextArea(3, 5)] private string _abilityDescription;
    [SerializeField] private CardAbilityKeyword[] _descriptionKeywords;
    [SerializeField] private CardAbilityKeyword[] _descriptionlessKeywords = Array.Empty<CardAbilityKeyword>();
    
    public string AbilityName => _abilityName;
    

    public EditableCardAbilityDescription MakeEditableDescription()
    {
        EditableCardAbilityDescription editableDescription = 
            new EditableCardAbilityDescription(_abilityName, _abilityDescription, _descriptionKeywords);

        foreach (CardAbilityKeyword descriptionKeyword in _descriptionKeywords)
        {
            descriptionKeyword.ApplyDescriptionModifications(editableDescription);
        }
        foreach (CardAbilityKeyword descriptionlessKeyword in _descriptionlessKeywords)
        {
            descriptionlessKeyword.ApplyDescriptionModifications(editableDescription);
        }
        
        return editableDescription;
    }
    
}