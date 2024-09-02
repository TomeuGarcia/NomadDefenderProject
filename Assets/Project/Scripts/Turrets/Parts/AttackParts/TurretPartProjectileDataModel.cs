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
    [SerializeField] protected ProjectileParticleType _hitParticlesType;
    [SerializeField] private ATurretProjectileBehaviour _projectilePrefab;
    public ATurretProjectileBehaviour.Type ProjectileType => _projectileType;
    public ProjectileParticleType HitParticlesType => _hitParticlesType;
    public ATurretProjectileBehaviour ProjectilePrefab => _projectilePrefab;


    [Header("SHOOTING CONFIG")] 
    [SerializeField] private AProjectileShootingControllerCreator _shootingControllerCreator;
    public AProjectileShootingControllerCreator ShootingControllerCreator => _shootingControllerCreator;
    
    
    [Header("VISUALS")]
    [SerializeField] public Texture texture;
    [SerializeField] public Sprite abilitySprite;
    [SerializeField] public Color materialColor = Color.white;
    [SerializeField] private Material _materialForTurret;
    [SerializeField] private Material _materialForTurretPreview;
    public Material MaterialForTurret => _materialForTurret;
    public Material MaterialForTurretPreview => _materialForTurretPreview;

    
    [Header("STATS CONFIG")] 
    [SerializeField, Min(0f)] private float _movementSpeed = 2f;
    public float MovementSpeed => _movementSpeed;
    

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
