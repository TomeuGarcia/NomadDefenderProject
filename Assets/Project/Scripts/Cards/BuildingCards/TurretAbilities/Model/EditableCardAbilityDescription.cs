
using System.Collections.Generic;

public class EditableCardAbilityDescription
{
    public readonly string Name;
    public string Description { get; private set; }
    public readonly CardAbilityKeyword[] Keywords;
    

    public EditableCardAbilityDescription(string name, string description, CardAbilityKeyword[] keywords)
    {
        Name = name;
        Description = description;
        Keywords = keywords;
    }

    public void ApplyDescriptionModifications(Dictionary<string, string> keywordsToValues)
    {
        foreach (KeyValuePair<string,string> keywordToValue in keywordsToValues)
        {
            Description = Description.Replace(keywordToValue.Key, keywordToValue.Value);
        }
    }
    
    
}