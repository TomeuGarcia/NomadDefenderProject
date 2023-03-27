using System.Collections;
using System.Collections.Generic;
using System.IO;
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

    [Header("JSON")]
    [SerializeField] private string enemyWavesJSONname;
    [SerializeField] private NodeEnums.ProgressionState progressionState;
    private static string[] PROGRESSION_PATHS = { "EarlyLevels/", "MidLevels/", "LateLevels/" };
    [SerializeField, Range(1, 2)] private int numNodes = 1;
    private static string[] NUM_NODES_PATHS = { "1Node/", "2Nodes/" };
    private const string PATH_TO_JSON = "/JSONfiles/EnemyWaves/";

    [System.Serializable]
    private class EnemyWavesWrapper
    {
        [SerializeField] public EnemyWave[] enemyWaves;

        public EnemyWavesWrapper(EnemyWave[] enemyWaves)
        {
            this.enemyWaves = new EnemyWave[enemyWaves.Length];
            enemyWaves.CopyTo(this.enemyWaves, 0);
        }
    }



    public delegate void EnemyWaveSpawnerAction(EnemyWaveSpawner enemyWaveSpawner);
    public event EnemyWaveSpawnerAction OnWaveFinished;
    public event EnemyWaveSpawnerAction OnLastWaveFinished;

    bool stopForced;

    private void SaveToJSON(string path)
    {
        Debug.Log(path);

        EnemyWavesWrapper enemyWavesWrapper = new EnemyWavesWrapper(enemyWaves);
        string content = JsonUtility.ToJson(enemyWavesWrapper);

        File.WriteAllText(path, content);
    }
    

    private void LoadFromJSON(string path)
    {
        string content = File.ReadAllText(path);
        
        enemyWaves = JsonUtility.FromJson<EnemyWave[]>(content);
    }
    private string GetPathJSON()
    {        
        string progressionStatePath = PROGRESSION_PATHS[(int)progressionState];
        string numNodesPath = NUM_NODES_PATHS[numNodes - 1];
        string targetFilePath = enemyWavesJSONname + ".json";

        string directory = Application.persistentDataPath + PATH_TO_JSON + progressionStatePath + numNodesPath;
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        return directory + targetFilePath;
    }

    private void OnValidate()
    {
        
        if (enemyWavesJSONname == null) return;
        if (enemyWavesJSONname.Length < 1) return;
        SaveToJSON(GetPathJSON());


        return;
        for (int enemyWaveI = 0; enemyWaveI < enemyWaves.Length; ++enemyWaveI)
        {
            enemyWaves[enemyWaveI].enemiesInWave = new EnemyInWave[enemyWaves[enemyWaveI].enemies.Length];

            for (int i = 0; i < enemyWaves[enemyWaveI].enemies.Length; i++)
            {
                enemyWaves[enemyWaveI].enemiesInWave[i] = new EnemyInWave(enemyWaves[enemyWaveI].enemies[i], enemyWaves[enemyWaveI].delayBetweenSpawns);
            }
        }
    }
    
    
    

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
            /*
            foreach (Enemy.EnemyType enemyType in enemyWaves[currentWave].enemies)
            {
                if (stopForced) break;

                SpawnEnemy(enemyType, spawnTransform);

                yield return new WaitForSeconds(enemyWaves[currentWave].delayBetweenSpawns);
            }
            */
            foreach (EnemyInWave enemyInWave in enemyWaves[currentWave].enemiesInWave)
            {
                if (stopForced) break;

                SpawnEnemy(enemyInWave.EnemyType, spawnTransform);

                yield return new WaitForSeconds(enemyInWave.DelayBeforeSpawn);
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
