
using System.Collections.Generic;

public class TurretAbilityDescription
{
    public readonly string Name;
    public string Description { get; private set; }
    

    public TurretAbilityDescription(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public void ApplyDescriptionModifications(Dictionary<string, int> keywordsToValues)
    {
        foreach (KeyValuePair<string,int> keywordToValue in keywordsToValues)
        {
            Description = Description.Replace(keywordToValue.Key, keywordToValue.Value.ToString());
        }
    }
    
    
}