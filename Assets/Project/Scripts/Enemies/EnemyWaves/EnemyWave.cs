using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class EnemyWave
{
    public float delayBetweenSpawns = 1.0f;
    public Enemy.EnemyType[] enemies;
    public EnemyInWave[] enemiesInWave;


    [System.Serializable]
    public struct EnemyInWave
    {
        [SerializeField] private Enemy.EnemyType enemyType;
        [SerializeField] private float delayBeforeSpawn;

        public Enemy.EnemyType EnemyType => enemyType;
        public float DelayBeforeSpawn => delayBeforeSpawn;

        public EnemyInWave(Enemy.EnemyType enemyType, float delayBeforeSpawn)
        {
            this.enemyType = enemyType;
            this.delayBeforeSpawn = delayBeforeSpawn;
        }
    }


    public int GetEnemyCount()
    {
        return enemies.Length;
    }

}
