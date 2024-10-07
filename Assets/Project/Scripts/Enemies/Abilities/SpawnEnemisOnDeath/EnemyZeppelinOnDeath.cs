using System;
using System.Collections;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyZeppelinOnDeath : MonoBehaviour
{
    [SerializeField] private Enemy _enemy;
    [SerializeField] private EnemyTypeConfig_Zeppelin _zeppelinConfig;
    

    private void OnEnable()
    {
        _enemy.OnEnemyDeath += SpawnEnemies;
    }
    private void OnDisable()
    {
        _enemy.OnEnemyDeath -= SpawnEnemies;
    }


    private void SpawnEnemies(Enemy deadEnemy)
    {
        SpawnEnemiesOverTime(
            deadEnemy.SpawnerOwner,
            deadEnemy.transform.parent,
            deadEnemy.AttackDestination,
            deadEnemy.PathFollower.CurrentNode,
            deadEnemy.PathFollower.ToNextNodeT
        );
    }

    private async void SpawnEnemiesOverTime(
        EnemyWaveSpawner spawner, Transform enemiesParent,
        EnemyAttackDestination attackDestination,
        PathNode ownerCurrentNode, float ownerToNextNodeT)
    {
        for (int i = 0; i < _zeppelinConfig.NumberOfSpawns; ++i)
        {
            spawner.SpawnEnemyNotIncludedInWave(
                _zeppelinConfig.SpawnEnemyType, 
                enemiesParent,
                attackDestination, 
                _zeppelinConfig.RandomSpawnOffset(),
                ownerCurrentNode,
                ownerToNextNodeT
            );

            await Task.Delay(TimeSpan.FromSeconds(_zeppelinConfig.DelayBetweenSpawns));
        }
    }
    
}