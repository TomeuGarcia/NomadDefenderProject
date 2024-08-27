using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "TurretPassiveBase", 
    menuName = SOAssetPaths.TURRET_PARTS_BASEPASSIVES + "TurretPassiveBase")]
public class TurretPassiveBase : ScriptableObject
{
    [System.Serializable]
    public struct VisualInformation
    {
        public Sprite sprite;
        public Texture2D spriteAsTexture;
        public Color32 color;
    }
    
    [Header("PASSIVE")]
    public BasePassive passive;

    [Header("VISUALS")]
    public VisualInformation visualInformation;

    

    // Operator Overloads
    public static bool operator ==(TurretPassiveBase obj1, TurretPassiveBase obj2)
    {
        if (!obj1 || !obj2) return false;
        return obj1.passive.abilityName == obj2.passive.abilityName;
    }

    public static bool operator !=(TurretPassiveBase obj1, TurretPassiveBase obj2)
    {
        return !(obj1 == obj2);
    }
    
}
