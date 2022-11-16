using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[CreateAssetMenu(fileName = "TurretPartBody", menuName = "TurretParts/TurretPartBody")]

public class TurretPartBody : ScriptableObject
{
    private static int[] damagePerLvl = new int[] { 4, 10, 20, 30, 40 };
    private static float[] cadencePerLvl = new float[] { 3f, 2f, 1.5f, 1f, 0.5f };
    

    [Header("STATS")]
    [SerializeField, Min(0)] public int cost;
    [SerializeField, Range(1, 5), Tooltip("{ 4, 10, 20, 30, 40 }")] public int damageLvl;
    [SerializeField, Range(1, 5), Tooltip("{ 3f, 2f, 1.5f, 1f, 0.5f }")] public int cadenceLvl;

    [Header("PREFAB")]
    [SerializeField] public GameObject prefab;

    [Header("VISUALS")]
    [SerializeField] public Texture2D materialTextureMap;


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
}
