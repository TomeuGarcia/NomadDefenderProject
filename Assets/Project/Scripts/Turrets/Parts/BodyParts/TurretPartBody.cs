using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[CreateAssetMenu(fileName = "TurretPartBody", menuName = "TurretParts/TurretPartBody")]

public class TurretPartBody : ScriptableObject
{
    public static int[] damagePerLvl = new int[] { 6, 10, 20, 30, 40 };
    public static float[] cadencePerLvl = new float[] { 3f, 2f, 1.5f, 1f, 0.5f };
    public static float GetSlowestCadence() { return cadencePerLvl[0]; }
    
    public enum BodyType
    {
        SENTRY,
        BLASTER,
        SPAMMER
    }

    [SerializeField] public BodyType bodyType;

    [Header("STATS")]
    [SerializeField, Min(0)] public int cost;
    [SerializeField, Range(1, 5), Tooltip("{ 6, 10, 20, 30, 40 }")] public int damageLvl;
    [SerializeField, Range(1, 5), Tooltip("{ 3f, 2f, 1.5f, 1f, 0.5f }")] public int cadenceLvl;

    [Header("PREFAB")]
    [SerializeField] public GameObject prefab;

    [Header("VISUALS")]
    [SerializeField] public Texture2D materialTextureMap;

    [Header("ABILITY INFO")]
    [Header("Name")]
    [SerializeField] public string partName;
    [Header("Description")]
    [SerializeField, TextArea(3, 5)] public string abilityDescription = "No ability.";


    public int Damage { get => damagePerLvl[damageLvl-1]; }
    public float Cadence { get => cadencePerLvl[cadenceLvl-1]; }

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
}
