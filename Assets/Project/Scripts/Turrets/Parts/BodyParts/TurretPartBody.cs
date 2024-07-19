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

    [SerializeField] public BodyType bodyType;

    [Header("STATS")]
    [SerializeField, Min(0)] public int cost;
    [SerializeField] private CardStatConfig _damageStat;
    [SerializeField] private CardStatConfig _shotsPerSecondStat;

    [Header("PREFAB")]
    [SerializeField] public GameObject prefab;

    [Header("VISUALS")]
    [SerializeField] public Texture2D materialTextureMap;

    [Header("ABILITY INFO")]
    [Header("Name")]
    [SerializeField] public string partName;
    [Header("Description")]
    [SerializeField, TextArea(3, 5)] public string abilityDescription = "No ability.";


    public int BaseDamage => GetDamageByLevel(0); 
    public string BaseDamageText => GetDamageByLevelText(0);

    public float BaseShotsPerSecond => GetShotsPerSecondByLevel(0);
    public string BaseShotsPerSecondText => GetShotsPerSecondByLevelText(0);
    public float BaseShotsPerSecondInverted => GetShotsPerSecondInvertedByLevel(0);

    public void InitAsCopy(TurretPartBody other)
    {
        this.cost = other.cost;
        
        this._damageStat = other._damageStat;
        this._shotsPerSecondStat = other._shotsPerSecondStat;

        this.prefab = other.prefab;
        this.materialTextureMap = other.materialTextureMap;

        this.partName = other.partName;
        this.abilityDescription = other.abilityDescription;

        this.bodyType = other.bodyType;
    }


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

    public override bool Equals(object o)
    {
        return base.Equals(o);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public void SetStatTexts(TMP_Text damageText, TMP_Text fireRateText)
    {
        damageText.text = BaseDamageText;
        fireRateText.text = BaseShotsPerSecondText;
    }


    public int GetDamageByLevel(int level)
    {
        return (int)_damageStat.ComputeValueByLevel(level);
    }

    public string GetDamageByLevelText(int level)
    {
        return GetDamageByLevel(level).ToString();
    }


    public float GetShotsPerSecondByLevel(int level)
    {
        return _shotsPerSecondStat.ComputeValueByLevel(level);
    }
    public string GetShotsPerSecondByLevelText(int level)
    {
        return GetShotsPerSecondByLevel(level).ToString("0.0").Replace(',', '.');
    }
    public float GetShotsPerSecondInvertedByLevel(int level)
    {
        return 1f / GetShotsPerSecondByLevel(level);
    }
}
