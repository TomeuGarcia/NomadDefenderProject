using UnityEngine;

[System.Serializable]
public class SupportCardPartsGroup
{
    [SerializeField] private TurretPartBase _base;
        
    public TurretPartBase Base => _base;
    
    public SupportCardPartsGroup(SupportCardPartsGroup other)
    {
        _base = other._base;
    }
}