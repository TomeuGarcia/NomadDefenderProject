using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "EnemyWaveSpawner", menuName = "Enemies/EnemyWaveSpawner")]
public class EnemyWaveSpawner : ScriptableObject
{
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



    public void Init(PathNode startNode)
    {
        currentWave = 0;
        activeEnemies = 0;
        this.startNode = startNode;
    }


    public IEnumerator SpawnCurrentWaveEnemies()
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
                SpawnEnemy(enemyType);

                yield return new WaitForSeconds(enemyWaves[currentWave].delayBetweenSpawns);
            }
        }
        
    }


    private void SpawnEnemy(Enemy.EnemyType enemyType)
    {
        GameObject enemyGameObject = EnemyFactory.GetInstance().GetEnemyGameObject(enemyType, startNode.Position, Quaternion.identity);
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
        Vector3 randomOffset = (spawnedEnemy.transformToMove.right * Random.Range(-0.2f, 0.2f)) + 
                               (spawnedEnemy.transformToMove.forward * Random.Range(-0.2f, 0.2f));
        spawnedEnemy.pathFollower.Init(startNode.GetNextNode(), startNode.GetDirectionToNextNode(), randomOffset, totalDistance, spawnedEnemy.transformToMove);
        /////////////

        spawnedEnemy.OnEnemyDeath += SubtractActiveEnemy;
    }

    private void SubtractActiveEnemy(Enemy enemy)
    {
        enemy.OnEnemyDeath -= SubtractActiveEnemy;

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

}
