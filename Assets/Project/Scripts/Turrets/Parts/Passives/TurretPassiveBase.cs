using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "TurretPassiveBase", menuName = "TurretParts/TurretPassiveBase")]
public class TurretPassiveBase : ScriptableObject
{
    [System.Serializable]
    public struct VisualInformation
    {
        public Sprite sprite;
        public Texture2D spriteAsTexture;
        public Color32 color;

        [TextArea] public String description;
    }

    [Header("STATS")]
    [SerializeField, Min(0)] public int cost;

    [Header("PASSIVE")]
    public BasePassive passive;

    [Header("VISUALS")]
    public VisualInformation visualInformation;


    public void InitAsCopy(TurretPassiveBase other)
    {
        this.cost = other.cost;
        this.passive = other.passive;
        this.visualInformation = other.visualInformation;
    }


    // Operator Overloads
    public static bool operator ==(TurretPassiveBase obj1, TurretPassiveBase obj2)
    {
        return obj1.passive.abilityName == obj2.passive.abilityName;
    }

    public static bool operator !=(TurretPassiveBase obj1, TurretPassiveBase obj2)
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
