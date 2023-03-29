using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[System.Serializable]
public class EnemyInWave
{
    [SerializeField, SerializeReference] public Enemy.EnemyType enemyType;
    [SerializeField, SerializeReference] public float delayBeforeSpawn;

    public Enemy.EnemyType EnemyType => enemyType;
    public float DelayBeforeSpawn => delayBeforeSpawn;


    public EnemyInWave()
    {
        this.enemyType = Enemy.EnemyType.BASIC;
        this.delayBeforeSpawn = 1.0f;
    }
    public EnemyInWave(Enemy.EnemyType enemyType, float delayBeforeSpawn)
    {
        this.enemyType = enemyType;
        this.delayBeforeSpawn = delayBeforeSpawn;
    }
}

[System.Serializable]
public class EnemyWave
{
    public float delayBetweenSpawns;// = 1.0f;
    public Enemy.EnemyType[] enemies;
    [SerializeField] public EnemyInWave[] enemiesInWave;


    public EnemyWave()
    {
        delayBetweenSpawns = 1.0f;
        enemiesInWave = new EnemyInWave[2];
        for (int i = 0; i < enemiesInWave.Length; ++i)
        {
            enemiesInWave[i] = new EnemyInWave();
        }
    }

    public EnemyWave(EnemyWave other)
    {
        delayBetweenSpawns = other.delayBetweenSpawns;
        enemies = other.enemies;
        enemiesInWave = other.enemiesInWave;
    }

    public int GetEnemyCount()
    {
        return enemies.Length;
    }

}
