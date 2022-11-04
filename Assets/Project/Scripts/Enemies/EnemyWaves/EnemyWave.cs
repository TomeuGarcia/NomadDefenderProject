using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class EnemyWave
{
    public float delayBetweenSpawns = 1.0f;
    public Enemy.EnemyType[] enemies;


    public int GetEnemyCount()
    {
        return enemies.Length;
    }

}
