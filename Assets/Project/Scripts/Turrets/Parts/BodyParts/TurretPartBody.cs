using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "TurretPartBody", menuName = "TurretParts/TurretPartBody")]

public class TurretPartBody : ScriptableObject
{
    public static int[] damagePerLvl = new int[] {8, 20, 60, 120, 180   };
    public static float[] cadencePerLvl = new float[] { 4, 1.33333f, 0.5f, 0.4f, 0.26666f };
    
    public enum BodyType
    {
        SENTRY,
        BLASTER,
        SPAMMER
    }

    [SerializeField] public BodyType bodyType;

    [Header("STATS")]
    [SerializeField, Min(0)] public int cost;
    [SerializeField, Range(1, 5), Tooltip("{ 12, 36, 72, 120, 180  }")] public int damageLvl;
    [SerializeField, Range(1, 5), Tooltip("{ 2f, 0.666f, 0.3333f, 0.2f, 0.13333f }")] public int cadenceLvl;

    [Header("PREFAB")]
    [SerializeField] public GameObject prefab;

    [Header("VISUALS")]
    [SerializeField] public Texture2D materialTextureMap;

    [Header("ABILITY INFO")]
    [Header("Name")]
    [SerializeField] public string partName;
    [Header("Description")]
    [SerializeField, TextArea(3, 5)] public string abilityDescription = "No ability.";


    public int Damage => GetDamageByLevel(damageLvl); 
    public string DamageText => GetDamageByLevelText(damageLvl);
    public float CadenceInverted => GetCadenceByLevelInverted(cadenceLvl);
    public float Cadence => GetCadenceByLevel(cadenceLvl);
    public string CadenceText => GetCadenceByLevelText(cadenceLvl);

    public float GetDamagePer1()
    {
        return (float)damageLvl / (float)5;
    }

    public float GetCadencePer1()
    {
        return (float)cadenceLvl / (float)5;
    }

    public void InitAsCopy(TurretPartBody other)
    {
        this.cost = other.cost;
        this.damageLvl = other.damageLvl;
        this.cadenceLvl = other.cadenceLvl;
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
        damageText.text = DamageText;
        fireRateText.text = CadenceText;
    }

    public int GetDamageByLevel(int level)
    {
        return damagePerLvl[level - 1];
    }

    public string GetDamageByLevelText(int level)
    {
        return GetDamageByLevel(level).ToString();
    }

    public float GetCadenceByLevel(int level)
    {
        return cadencePerLvl[level - 1];
    }
    public float GetCadenceByLevelInverted(int level)
    {
        return 1f / GetCadenceByLevel(level);
    }
    public string GetCadenceByLevelText(int level)
    {
        return GetCadenceByLevelInverted(level).ToString("0.0").Replace(',', '.');
    }
}
