using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "TurretPartBody", 
    menuName = SOAssetPaths.TURRET_PARTS + "TurretPartBody")]
public class TurretPartBody : ScriptableObject
{
    
    public enum BodyType
    {
        SENTRY,
        BLASTER,
        SPAMMER
    }

    [Header("TYPE")]
    [SerializeField] public BodyType bodyType;

    [Header("STATS")]
    [SerializeField] private CardStatConfig _damageStat;
    [SerializeField] private CardStatConfig _shotsPerSecondStat;
    [SerializeField] private CardStatConfig _radiusRangeStat;
    
    public CardStatConfig DamageStat => _damageStat;
    public CardStatConfig ShotsPerSecondStat => _shotsPerSecondStat;
    public CardStatConfig RadiusRangeStat => _radiusRangeStat;

    
    [Header("BASE PRIMITIVE")] 
    [SerializeField] private BasePartPrimitive _basePartPrimitive;
    public BasePartPrimitive BasePartPrimitive => _basePartPrimitive;
    

    [Header("PREFAB")]
    [SerializeField] public GameObject prefab;
    [SerializeField] public GameObject previewPrefab;

    [Header("VISUALS")]
    [SerializeField] public Texture2D materialTextureMap;

    [Header("ABILITY INFO")]
    [Header("Name")]
    [SerializeField] public string partName;
    [Header("Description")]
    [SerializeField, TextArea(3, 5)] public string abilityDescription = "No ability.";

    
    // Operator Overloads
    public static bool operator ==(TurretPartBody obj1, TurretPartBody obj2)
    {
        if (!obj1 || !obj2) return false;
        return obj1.prefab == obj2.prefab;
    }

    public static bool operator !=(TurretPartBody obj1, TurretPartBody obj2)
    {
        return !(obj1 == obj2);
    }

}
