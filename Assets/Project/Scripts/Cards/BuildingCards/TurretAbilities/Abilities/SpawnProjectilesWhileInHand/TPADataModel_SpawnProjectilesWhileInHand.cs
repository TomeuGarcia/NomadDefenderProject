using UnityEngine;

[CreateAssetMenu(fileName = "TurretAbility_SpawnProjectilesWhileInHand", 
    menuName = SOAssetPaths.CARDS_ABILITIES + "SpawnProjectilesWhileInHand")]
public class TPADataModel_SpawnProjectilesWhileInHand : ATurretPassiveAbilityDataModel
{
    [Header("ABILITY CONFIG")] 
    [SerializeField] private TurretPartProjectileDataModel _projectileDataModel;
    [SerializeField, Min(0)] private float _delayBetweenSpawns = 0.5f; 
    public TurretPartProjectileDataModel ProjectileDataModel => _projectileDataModel;
    public float DelayBetweenSpawns => _delayBetweenSpawns;


    public override ATurretPassiveAbility MakePassiveAbility()
    {
        return new TurretPassiveAbility_SpawnProjectilesWhileInHand(this);
    }
}