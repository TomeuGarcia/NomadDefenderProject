using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TurretPartProjectile_NAME", 
    menuName = SOAssetPaths.TURRET_PARTS + "TurretPartProjectile")]
public class TurretPartProjectileDataModel : ScriptableObject
{
    [Header("PREFAB")]
    [SerializeField] protected ATurretProjectileBehaviour.Type _projectileType;
    [SerializeField] private ATurretProjectileBehaviour _projectilePrefab;
    public ATurretProjectileBehaviour.Type ProjectileType => _projectileType;
    public ATurretProjectileBehaviour ProjectilePrefab => _projectilePrefab;

    [Header("VISUALS")]
    [SerializeField] public Texture texture;
    [SerializeField] public Sprite abilitySprite;
    [SerializeField] public Color materialColor = Color.white;
    [SerializeField] private Material _materialForTurret;
    public Material MaterialForTurret => _materialForTurret;


    [Header("ABILITY INFO")]
    [Header("Name")]
    [SerializeField] private string _abilityName;
    [Header("Description")]
    [SerializeField, TextArea(3, 5)] private string _abilityDescription;
    [SerializeField] private CardAbilityKeyword[] _descriptionKeywords;

    public string AbilityName => _abilityName;

    public TurretAbilityDescription MakeAbilityDescription()
    {
        TurretAbilityDescription abilityDescription = 
            new TurretAbilityDescription(_abilityName, _abilityDescription);

        foreach (CardAbilityKeyword descriptionKeyword in _descriptionKeywords)
        {
            descriptionKeyword.ApplyDescriptionModifications(abilityDescription);
        }
        
        return abilityDescription;
    }


    // Operator Overloads
    public static bool operator== (TurretPartProjectileDataModel obj1, TurretPartProjectileDataModel obj2)
    {
        if (!obj1 || !obj2 ) return false;
        return obj1._projectilePrefab == obj2._projectilePrefab;
    }

    public static bool operator !=(TurretPartProjectileDataModel obj1, TurretPartProjectileDataModel obj2)
    {
        return !(obj1 == obj2);
    }

}
