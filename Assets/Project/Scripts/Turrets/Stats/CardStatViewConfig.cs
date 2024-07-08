using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardStatConfig_NAME", menuName = "Cards/CardStatConfig")]
public class CardStatViewConfig : ScriptableObject
{
    [Header("META TYPE")]
    [SerializeField] private CardStatMetaType _metaType;

    [Header("PARAMETERS")]
    [SerializeField] private string _name;
    [SerializeField] private Sprite _icon;
    [SerializeField] private Color _color;
    

    public string GameObjectName => _metaType.GetGameObjectName(_name);
    public string ValueGameObjectName => _metaType.GetValueGameObjectName(_name);
    public Sprite Icon => _icon;
    public Color Color => _color;
}
