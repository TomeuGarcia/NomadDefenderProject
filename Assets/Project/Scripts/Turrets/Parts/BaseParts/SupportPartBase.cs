using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "TurretPartBase", 
    menuName = SOAssetPaths.TURRET_PARTS_BASES + "TurretPartBase")]
public class SupportPartBase : ScriptableObject
{
    [Header("STATS")]
    [SerializeField] private CardStatConfig _radiusRangeStat;
    public CardStatConfig RadiusRangeStat => _radiusRangeStat;

    [Header("PREFAB")] 
    [SerializeField] private BasePartPrimitive _basePartPrimitive;
    public BasePartPrimitive BasePartPrimitive => _basePartPrimitive;
    
    [Header("VISUALS")]
    [SerializeField] public Sprite abilitySprite;
    [SerializeField] public Color32 spriteColor;

    [Header("ABILITY INFO")]
    [Header("Name")]
    [SerializeField] public string abilityName;
    [Header("Description")]
    [SerializeField, TextArea(3, 5)] public string abilityDescription;

    [Header("UPGRADE DESCRIPTIONS")]
    [SerializeField, TextArea(2, 5)] public string upgrade1Description;
    [SerializeField, TextArea(2, 5)] public string upgrade2Description;
    [SerializeField, TextArea(2, 5)] public string upgrade3Description;


    
    // Operator Overloads
    public static bool operator ==(SupportPartBase obj1, SupportPartBase obj2)
    {
        if (!obj1 || !obj2) return false;
        return obj1.BasePartPrimitive.Prefab == obj2.BasePartPrimitive.Prefab;
    }

    public static bool operator !=(SupportPartBase obj1, SupportPartBase obj2)
    {
        return !(obj1 == obj2);
    }

    public string GetUpgradeDescriptionByLevel(int level)
    {
        if (level == 1) return upgrade1Description;
        else if (level == 2) return upgrade2Description;
        else if (level == 3) return upgrade3Description;

        return "";
    }

}
