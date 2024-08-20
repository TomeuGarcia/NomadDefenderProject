using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TurretPartAttack", 
    menuName = SOAssetPaths.TURRET_PARTS + "TurretPartAttack")]
public class TurretPartAttack : ScriptableObject
{
    [Header("STATS")]
    [SerializeField, Min(0)] public float _numberOfHittableTargets = 1f;

    [Header("PREFAB")]
    [SerializeField] public GameObject prefab;

    [Header("VISUALS")]
    [SerializeField] public Texture texture;
    [SerializeField] public Sprite abilitySprite;
    [SerializeField] public Color materialColor = Color.white;


    [Header("ABILITY INFO")]
    [Header("Name")]
    [SerializeField] public string abilityName;
    [Header("Description")]
    [SerializeField, TextArea(3, 5)] public string abilityDescription;

    public float NumberOfHittableTargets => _numberOfHittableTargets;

    

    public virtual void OnTurretPlaced(TurretBuilding owner, Material turretMaterial) 
    {    }


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

}
