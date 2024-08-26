using System.Collections;
using System.Collections.Generic;
using UnityEditor.ProBuilder;
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
    [SerializeField] private CardAbilityDescriptionModel _descriptionModel;
    public string AbilityName => _descriptionModel.AbilityName;
    

    public EditableCardAbilityDescription MakeAbilityDescription()
    {
        return _descriptionModel.MakeEditableDescription();
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
