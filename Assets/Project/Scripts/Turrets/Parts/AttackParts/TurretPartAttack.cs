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


    public void InitAsCopy(TurretPartAttack other)
    {
        this.cost = other.cost;
        this.prefab = other.prefab;
        this.materialTexture = other.materialTexture;
        this.materialColor = other.materialColor;
    }


}
