
using UnityEngine;

[CreateAssetMenu(fileName = "TurretViewAddOnConfig_NAME", 
    menuName = SOAssetPaths.VFX_ABILITIES + "TurretViewAddOn")]
public class TurretViewAddOnConfig : ScriptableObject
{
    [SerializeField] private ObjectPoolData<ATurretViewAddOn> _objectPoolData;
    public ObjectPoolData<ATurretViewAddOn> ObjectPoolData => _objectPoolData;
}