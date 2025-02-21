using System;
using UnityEngine;

public class EnemyBuildingDisabler : MonoBehaviour
{
    [SerializeField] private Enemy _enemy;
    [SerializeField] private EnemyTypeConfig_BuildingDisabler _buildingDisablerConfig;
    

    private void OnEnable()
    {
        _enemy.OnEnemyDeath += SpawnBuildingDisableWave;
    }
    private void OnDisable()
    {
        _enemy.OnEnemyDeath -= SpawnBuildingDisableWave;
    }

    private void SpawnBuildingDisableWave(Enemy enemy)
    {
        BuildingDisableBomb disabler = BuildingDisableBombFactory.Instance.Create(_buildingDisablerConfig.BuildingDisableBombConfig,
            transform.position, Quaternion.identity);
        disabler.transform.position = transform.position;
        disabler.transform.rotation = transform.rotation;
    }
}