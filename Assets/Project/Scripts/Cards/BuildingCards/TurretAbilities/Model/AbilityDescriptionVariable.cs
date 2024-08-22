using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;


[System.Serializable]
public class AbilityDescriptionVariable
{
    [SerializeField] private string _name;
    
    [SerializeField] private bool _isDynamic = true;
    
    [AllowNesting]
    [HideIf("_isDynamic"), SerializeField] private int _constantValue;


    public string Name => _name;
    public int Value => _constantValue;
    public string ValueAsString()
    {
        return _constantValue.ToString();
    }
    
}