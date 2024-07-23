using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "TurretPartBase", 
    menuName = SOAssetPaths.TURRET_PARTS_BASES + "TurretPartBase")]
public class TurretPartBase : ScriptableObject
{
    [Header("STATS")]
    [SerializeField, Min(0)] public int cost;
    [SerializeField] private CardStatConfig _radiusRangeStat;
    public CardStatConfig RadiusRangeStat => _radiusRangeStat;

    [Header("PREFAB")]
    [SerializeField] public GameObject prefab;

    [Header("VISUALS")]
    [SerializeField] public Texture2D materialTexture;
    [SerializeField] public Color materialColor;

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

    public bool HasAbilitySprite()
    {
        return abilitySprite != null;
    }


    public void InitAsCopy(TurretPartBase other)
    {
        this.cost = other.cost;
        this._radiusRangeStat = other._radiusRangeStat;

        this.prefab = other.prefab;

        this.materialTexture = other.materialTexture;
        this.materialColor = other.materialColor;
        this.abilitySprite = other.abilitySprite;
        this.spriteColor = other.spriteColor;

        this.abilityName = other.abilityName;
        this.abilityDescription = other.abilityDescription;

        this.upgrade1Description = other.upgrade1Description;
        this.upgrade2Description = other.upgrade2Description;
        this.upgrade3Description = other.upgrade3Description;
    }


    // Operator Overloads
    public static bool operator ==(TurretPartBase obj1, TurretPartBase obj2)
    {
        if (!obj1 || !obj2) return false;
        return obj1.prefab == obj2.prefab;
    }

    public static bool operator !=(TurretPartBase obj1, TurretPartBase obj2)
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


    public string GetUpgradeDescriptionByLevel(int level)
    {
        if (level == 1) return upgrade1Description;
        else if (level == 2) return upgrade2Description;
        else if (level == 3) return upgrade3Description;

        return "";
    }

}
