using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyType_SpeedBoosterSpawner", 
    menuName = SOAssetPaths.ENEMY_TYPES + "EnemyTypeConfig_SpeedBoosterSpawner")]
public class EnemyTypeConfig_BuildingDisabler : EnemyTypeConfig
{
    [Space(30)]
    [Header("BUILDING DISABLER")]
    [Expandable] [SerializeField] private BuildingDisableWaveConfig _buildingDisableWaveConfig;
    
    public BuildingDisableWaveConfig BuildingDisableWaveConfig => _buildingDisableWaveConfig;
}