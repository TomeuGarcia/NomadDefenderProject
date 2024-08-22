using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;


[System.Serializable]
public class AbilityDescriptionVariable
{
    [SerializeField] private string _name;
    
    [SerializeField] private bool _isDynamic = true;
    
    [AllowNesting] [HideIf("_isDynamic"), SerializeField] private bool _isFloat = false;
    [AllowNesting] [ShowIf("IsConstantInt"), SerializeField] private int _constantValue;
    [AllowNesting] [ShowIf("IsConstantFloat"), SerializeField] private float _floatConstantValue;

    private bool IsConstantInt => !_isDynamic && !_isFloat;
    private bool IsConstantFloat => !_isDynamic && _isFloat;
    

    public string Name => _name;
    public int Value => _constantValue;
    public float FloatValue => _floatConstantValue;
    
    public string ValueAsString()
    {
        if (_isFloat)
        {
            return _floatConstantValue.ToString("0.0");
        }
        
        return _constantValue.ToString();
    }
    
}