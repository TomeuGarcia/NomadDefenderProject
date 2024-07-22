using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "AllEnemyWaveSpawnersCollection", 
    menuName = SOAssetPaths.ENEMY_WAVES + "AllEnemyWaveSpawnersCollection")]
public class AllEnemyWaveSpawnersCollection : ScriptableObject
{
    [Expandable] [SerializeField] private EnemyWaveSpawner[] _allEnemyWaveSpawners;
    [Expandable] [SerializeField] public AllEnemyTypeConfigsCollection enemyTypesColl;

    public EnemyWaveSpawner[] AllEnemyWaveSpawners => _allEnemyWaveSpawners;

    public void UpdateEnemyWaveSpawners(EnemyWaveSpawner[] allEnemyWaveSpawners)
    {
        _allEnemyWaveSpawners = allEnemyWaveSpawners;
    }

}

