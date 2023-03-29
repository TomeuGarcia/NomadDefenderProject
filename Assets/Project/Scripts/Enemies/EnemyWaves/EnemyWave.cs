using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[System.Serializable]
public class EnemyInWave
{
    [SerializeField, SerializeReference] public Enemy.EnemyType enemyType;
    [SerializeField, SerializeReference] public float delayBeforeSpawn;
    [SerializeField, SerializeReference, Min(1)] public int numberOfSpawns = 1;

    public Enemy.EnemyType EnemyType => enemyType;
    public float DelayBeforeSpawn => delayBeforeSpawn;
    public float NumberOfSpawns => numberOfSpawns;


    public EnemyInWave()
    {
        this.enemyType = Enemy.EnemyType.BASIC;
        this.delayBeforeSpawn = 1.0f;
        this.numberOfSpawns = 1;
    }
    public EnemyInWave(Enemy.EnemyType enemyType, float delayBeforeSpawn)
    {
        this.enemyType = enemyType;
        this.delayBeforeSpawn = delayBeforeSpawn;
        this.numberOfSpawns = 1;
    }
}

[System.Serializable]
public class EnemyWave
{
    [SerializeField] public EnemyInWave[] enemiesInWave;


    public EnemyWave()
    {
        enemiesInWave = new EnemyInWave[2];
        for (int i = 0; i < enemiesInWave.Length; ++i)
        {
            enemiesInWave[i] = new EnemyInWave();
        }
    }

    public EnemyWave(EnemyWave other)
    {
        enemiesInWave = other.enemiesInWave;
    }

    public EnemyWave(EnemyWaveWorkaround enemyWavesWorkaround)
    {
        enemiesInWave = new EnemyInWave[enemyWavesWorkaround.enemiesInWave.Length];
        enemyWavesWorkaround.enemiesInWave.CopyTo(enemiesInWave, 0);
    }


    public int GetEnemyCount()
    {
        int count = 0;

        foreach (EnemyInWave enemyInWave in enemiesInWave)
        {
            count += enemyInWave.numberOfSpawns;
        }

        return count;
    }

}

[System.Serializable]
public class EnemyWaveWorkaround
{
    [SerializeField] public EnemyInWave[] enemiesInWave;



    public EnemyWaveWorkaround(EnemyWave enemyWave)
    {
        enemiesInWave = new EnemyInWave[enemyWave.enemiesInWave.Length];
        enemyWave.enemiesInWave.CopyTo(enemiesInWave, 0);
    }


    public int GetEnemyCount()
    {
        return enemiesInWave.Length;
    }

}
