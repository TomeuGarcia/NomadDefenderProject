using System;
using UnityEngine;

public class EnemySpawnOtherEnemiesOnDeath : MonoBehaviour
{
    [SerializeField] private Enemy _enemy;


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
        
    }
    
}