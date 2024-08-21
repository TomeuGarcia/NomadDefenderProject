using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "MF_SetupConfig_NAME",
        menuName = "VFX/MF_SetupConfig", order = 0)]

public class MaterialFloatSetupConfig : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] private float _initialValue;

    private int _property;

    public int ID => _property;
    public string Name => _name;
    public float InitialValue => _initialValue;

    private void Awake()
    {
        _property = Shader.PropertyToID(_name);
    }
}