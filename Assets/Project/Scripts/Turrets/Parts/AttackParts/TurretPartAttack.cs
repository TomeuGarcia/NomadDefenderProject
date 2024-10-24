using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TurretPartAttack", menuName = "TurretParts/TurretPartAttack")]

public class TurretPartAttack : ScriptableObject
{
    [Header("STATS")]
    [SerializeField, Min(0)] public int cost;

    [Header("PREFAB")]
    [SerializeField] public GameObject prefab;

    [Header("VISUALS")]
    [SerializeField] public Texture2D materialTexture;
    [SerializeField] public Color materialColor;

    [Header("ABILITY INFO")]
    [Header("Name")]
    [SerializeField] public string abilityName;
    [Header("Description")]
    [SerializeField, TextArea(3, 5)] public string abilityDescription;

    public void InitAsCopy(TurretPartAttack other)
    {
        this.cost = other.cost;
        this.prefab = other.prefab;
        this.materialTexture = other.materialTexture;
        this.materialColor = other.materialColor;

        this.abilityName = other.abilityName;
        this.abilityDescription = other.abilityDescription;
    }


    // Operator Overloads
    public static bool operator== (TurretPartAttack obj1, TurretPartAttack obj2)
    {
        if (!obj1 || !obj2 ) return false;
        return obj1.prefab == obj2.prefab;
    }

    public static bool operator !=(TurretPartAttack obj1, TurretPartAttack obj2)
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
