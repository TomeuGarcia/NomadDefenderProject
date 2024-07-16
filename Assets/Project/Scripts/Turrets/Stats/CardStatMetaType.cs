using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "CardStatMetaType_NAME", menuName = "Cards/CardStatMetaType")]
public class CardStatMetaType : ScriptableObject
{
    [SerializeField] private string _typeName = "CardStat";

    private const string GAMEOBJECT_NAME_SUFFIX = "_Holder";
    private const string VALUE_GAMEOBJECT_NAME_SUFFIX = "_Value_Text";

    public string GetGameObjectName(string name)
    {
        return _typeName + "_" + name + GAMEOBJECT_NAME_SUFFIX;
    }
    public string GetValueGameObjectName(string name)
    {
        return name + VALUE_GAMEOBJECT_NAME_SUFFIX;
    }
}
