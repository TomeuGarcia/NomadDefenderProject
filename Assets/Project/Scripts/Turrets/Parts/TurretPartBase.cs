using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Building;

[CreateAssetMenu(fileName = "TurretPartBase", menuName = "TurretParts/TurretPartBase")]

public class TurretPartBase : ScriptableObject
{
    [Header("STATS")]
    [SerializeField, Min(0)] public int cost;
    [SerializeField, Min(1)] public int attackRange;
    [SerializeField] public GameObject passive;
    [SerializeField] public GameObject prefab;
}
