using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileViewAddOnConfig_NAME", 
    menuName = SOAssetPaths.TURRET_PARTS_ATTACKS + "ProjectileViewAddOn")]
public class ProjectileViewAddOnConfig : ScriptableObject
{
    [SerializeField] private ObjectPoolData<AProjectileViewAddOn> _objectPoolData;
    public ObjectPoolData<AProjectileViewAddOn> ObjectPoolData => _objectPoolData;
}