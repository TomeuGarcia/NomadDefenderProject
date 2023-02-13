using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;


[CreateAssetMenu(fileName = "EnemyWaveSpawner", menuName = "Enemies/EnemyWaveSpawner")]
public class EnemyWaveSpawner : ScriptableObject
{
    [SerializeField, Tooltip("Enemy stats will increase each round ddepending on this coef")]
    private float waveMultiplierCoef = 0.0f;
    [SerializeField] public float delayWaveStart = 1f;
    [SerializeField] public float delayBetweenWaves = 5f;
    [SerializeField] private EnemyWave[] enemyWaves;

    private PathNode startNode;

    public int currentWave { get; private set; }
    public int activeEnemies { get; private set; }

    public int numWaves => enemyWaves.Length;


    public delegate void EnemyWaveSpawnerAction(EnemyWaveSpawner enemyWaveSpawner);
    public event EnemyWaveSpawnerAction OnWaveFinished;
    public event EnemyWaveSpawnerAction OnLastWaveFinished;

    bool stopForced;


    public void Init(PathNode startNode)
    {
        currentWave = 0;
        activeEnemies = 0;
        this.startNode = startNode;
        stopForced = false;
    }


    public IEnumerator SpawnCurrentWaveEnemies(Transform spawnTransform)
    {
        yield return new WaitForSeconds(delayWaveStart);

        activeEnemies = enemyWaves[currentWave].GetEnemyCount();

        if (activeEnemies == 0) // If empty wave, end
        {
            EndWave();
        }
        else
        {
            foreach (Enemy.EnemyType enemyType in enemyWaves[currentWave].enemies)
            {
                if (stopForced) break;

                SpawnEnemy(enemyType, spawnTransform);

                yield return new WaitForSeconds(enemyWaves[currentWave].delayBetweenSpawns);
            }
        }
        
    }


    private void SpawnEnemy(Enemy.EnemyType enemyType, Transform spawnTransform)
    {
        GameObject enemyGameObject = EnemyFactory.GetInstance()
            .GetEnemyGameObject(enemyType, startNode.Position, Quaternion.identity, spawnTransform);
        enemyGameObject.SetActive(true);

        /////////////
        float totalDistance = 0f;
        PathNode itNode = startNode;
        while (!itNode.IsLastNode)
        {
            totalDistance += itNode.GetDistanceToNextNode();
            itNode = itNode.GetNextNode();
        }

        Enemy spawnedEnemy = enemyGameObject.GetComponent<Enemy>();
        spawnedEnemy.ApplyWaveStatMultiplier(CalcWaveMultiplier());
        Vector3 randomOffset = (spawnedEnemy.transformToMove.right * Random.Range(-0.3f, 0.3f)) + 
                               (spawnedEnemy.transformToMove.forward * Random.Range(-0.3f, 0.3f));
        spawnedEnemy.pathFollower.Init(startNode.GetNextNode(), startNode.GetDirectionToNextNode(), randomOffset, totalDistance, spawnedEnemy.transformToMove);
        /////////////

        spawnedEnemy.OnEnemyDeactivated += SubtractActiveEnemy;
    }

    private float CalcWaveMultiplier()
    {
        return (1.0f + (currentWave * waveMultiplierCoef));
    }

    private void SubtractActiveEnemy(Enemy enemy)
    {
        enemy.OnEnemyDeactivated -= SubtractActiveEnemy;

        if (--activeEnemies == 0)
        {
            EndWave();
        }
    }

    private void EndWave()
    {
        if (++currentWave < enemyWaves.Length)
        {
            if (OnWaveFinished != null) OnWaveFinished(this);
        }
        else
        {
            if (OnLastWaveFinished != null) OnLastWaveFinished(this);
        }
    }

    public void ForceStopWave()
    {

    }

}
