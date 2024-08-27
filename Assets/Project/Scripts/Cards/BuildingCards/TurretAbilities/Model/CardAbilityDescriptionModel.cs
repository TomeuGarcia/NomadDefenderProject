
using UnityEngine;

[System.Serializable]
public class CardAbilityDescriptionModel
{
    [SerializeField] private string _abilityName;
    [SerializeField, TextArea(3, 5)] private string _abilityDescription;
    [SerializeField] private CardAbilityKeyword[] _descriptionKeywords;
    
    public string AbilityName => _abilityName;
    

    public EditableCardAbilityDescription MakeEditableDescription()
    {
        EditableCardAbilityDescription editableDescription = 
            new EditableCardAbilityDescription(_abilityName, _abilityDescription, _descriptionKeywords);

        foreach (CardAbilityKeyword descriptionKeyword in _descriptionKeywords)
        {
            descriptionKeyword.ApplyDescriptionModifications(editableDescription);
        }
        
        return editableDescription;
    }
    
}