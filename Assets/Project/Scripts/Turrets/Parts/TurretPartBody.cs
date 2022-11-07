using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TurretPartBody", menuName = "TurretParts/TurretPartBody")]

public class TurretPartBody : ScriptableObject
{
    [Header("STATS")]
    [SerializeField] public int cost;
    [SerializeField] public int damage;
    [SerializeField] public float attackSpeed;
    [SerializeField] public GameObject prefab;
}
