using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TurretPartBase", menuName = "TurretParts/TurretPartBase")]

public class TurretPartBase : ScriptableObject
{
    private static int[] rangePerLvl = new int[] { 1, 2, 3, 4, 5 };


    [Header("STATS")]
    [SerializeField, Min(0)] public int cost;
    [SerializeField, Range(1, 5), Tooltip("{ 1, 2, 3, 4, 5 }")] public int rangeLvl;

    [Header("PREFAB")]
    [SerializeField] public GameObject prefab;

    [Header("PASSIVES")]
    [SerializeField] public BasePassive passive;
    [SerializeField] public GameObject supportPassive;

    [Header("VISUALS")]
    [SerializeField] public Texture2D materialTexture;
    [SerializeField] public Color materialColor;

    [SerializeField] public Sprite abilitySprite;
    [SerializeField] public Color32 spriteColor;


    public int Range { get => rangePerLvl[rangeLvl-1]; }

    public float GetRangePer1()
    {
        return (float)rangeLvl / (float)5;
    }

    public bool HasAbilitySprite()
    {
        return abilitySprite != null;
    }


    public void InitAsCopy(TurretPartBase other)
    {
        this.cost = other.cost;
        this.rangeLvl = other.rangeLvl;

        this.prefab = other.prefab;

        this.passive = other.passive;

        this.materialTexture = other.materialTexture;
        this.materialColor = other.materialColor;
        this.abilitySprite = other.abilitySprite;
        this.spriteColor = other.spriteColor;
    }

}
