using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[System.Serializable]
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct EnemyInWave
{
    [SerializeField, SerializeReference] private Enemy.EnemyType enemyType;
    [SerializeField, SerializeReference] private float delayBeforeSpawn;

    public Enemy.EnemyType EnemyType => enemyType;
    public float DelayBeforeSpawn => delayBeforeSpawn;

    public EnemyInWave(Enemy.EnemyType enemyType, float delayBeforeSpawn)
    {
        this.enemyType = enemyType;
        this.delayBeforeSpawn = delayBeforeSpawn;
    }
}

[System.Serializable]
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct EnemyWave
{
    public float delayBetweenSpawns;// = 1.0f;
    public Enemy.EnemyType[] enemies;
    [SerializeField] public EnemyInWave[] enemiesInWave;


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
