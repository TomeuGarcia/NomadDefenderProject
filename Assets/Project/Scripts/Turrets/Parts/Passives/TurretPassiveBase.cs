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
}
