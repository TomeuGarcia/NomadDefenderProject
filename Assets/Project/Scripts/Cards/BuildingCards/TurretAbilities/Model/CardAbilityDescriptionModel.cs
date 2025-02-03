
using System;
using Project.Scripts.Cards.BuildingCards.TurretAbilities.Model;
using UnityEngine;

[System.Serializable]
public class CardAbilityDescriptionModel
{
    [SerializeField] private string _abilityName;
    [SerializeField, TextArea(3, 5)] private string _abilityDescription;
    [SerializeField] private CardAbilityKeyword[] _descriptionKeywords;
    [SerializeField] private CardAbilityKeyword[] _descriptionlessKeywords = Array.Empty<CardAbilityKeyword>();
    [SerializeField] private AbilityDescriptionPattern[] _descriptionPatterns = Array.Empty<AbilityDescriptionPattern>();
    
    public string AbilityName => _abilityName;
    

    public EditableCardAbilityDescription MakeEditableDescription()
    {
        EditableCardAbilityDescription editableDescription = 
            new EditableCardAbilityDescription(_abilityName, _abilityDescription, _descriptionKeywords);

        foreach (CardAbilityKeyword descriptionKeyword in _descriptionKeywords)
        {
            descriptionKeyword.ApplyModificationsToDescription(editableDescription);
        }
        foreach (CardAbilityKeyword descriptionlessKeyword in _descriptionlessKeywords)
        {
            descriptionlessKeyword.ApplyModificationsToDescription(editableDescription);
        }
        foreach (AbilityDescriptionPattern descriptionPattern in _descriptionPatterns)
        {
            descriptionPattern.ApplyModificationsToDescription(editableDescription);
        }
        
        return editableDescription;
    }
    
}