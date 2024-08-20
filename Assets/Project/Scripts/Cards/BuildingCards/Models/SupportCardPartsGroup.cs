using UnityEngine;

[System.Serializable]
public class SupportCardPartsGroup
{
    [SerializeField] private SupportPartBase _base;
        
    public SupportPartBase Base => _base;
    
    public SupportCardPartsGroup(SupportCardPartsGroup other)
    {
        _base = other._base;
    }
}