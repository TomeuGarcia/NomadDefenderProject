using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    //[SerializeField] private EnemyWaveWorkaround[] enemyWavesWorkaround;
    public EnemyWave[] EnemyWaves => enemyWaves;

    private PathNode startNode;

    public int currentWave { get; private set; }
    public int activeEnemies { get; private set; }

    public int numWaves => enemyWaves.Length;

    // JSON 
    private string nameLevel;
    private string nameJSON;
    private NodeEnums.ProgressionState progressionState;
    private int numNodes = 1;
    private bool isTutorial;
    private bool IS_INCORRECT;

    public static string[] PROGRESSION_PATHS = { "EarlyLevels/", "MidLevels/", "LateLevels/" };
    public static string[] NUM_NODES_PATHS = { "1Node/", "2Nodes/" };
    public const string PATH_TO_JSON = "/JSONfiles/EnemyWaves/";

    public string NameLevel => nameLevel;
    public string NameJSON => nameJSON;
    public NodeEnums.ProgressionState ProgressionState => progressionState;
    public int NumNodes => numNodes;
    public bool IsTutorial => isTutorial;
    public bool IsIncorrect => IS_INCORRECT;
    //


    public delegate void EnemyWaveSpawnerAction(EnemyWaveSpawner enemyWaveSpawner);
    public event EnemyWaveSpawnerAction OnWaveFinished;
    public event EnemyWaveSpawnerAction OnLastWaveFinished;

    bool stopForced;


    private void OnValidate()
    {        
        // UNCOMMENT AFTER FIXING
        //EnemyWaveJSONManager.SaveEnemyWave(this, true);

        return;
        /*
        for (int enemyWaveI = 0; enemyWaveI < enemyWaves.Length; ++enemyWaveI)
        {
            enemyWaves[enemyWaveI].enemiesInWave = new EnemyInWave[enemyWaves[enemyWaveI].enemies.Length];

            for (int i = 0; i < enemyWaves[enemyWaveI].enemies.Length; i++)
            {
                enemyWaves[enemyWaveI].enemiesInWave[i] = new EnemyInWave(enemyWaves[enemyWaveI].enemies[i], enemyWaves[enemyWaveI].delayBetweenSpawns);
            }
        }
        */
    }

    public void ValidateJSONFormat()
    {
        int lastUnderscore = name.LastIndexOf('_');
        nameLevel = name.Substring(0, lastUnderscore != -1 ? lastUnderscore : name.Length);
        nameJSON = name;
        IS_INCORRECT = false;

        if (nameLevel == null) return;
        if (nameLevel.Length < 1) return;

        char progressionChar = nameLevel[0];
        if (progressionChar == 'E')
        {
            progressionState = NodeEnums.ProgressionState.EARLY;
        }
        else if (progressionChar == 'M')
        {
            progressionState = NodeEnums.ProgressionState.MID;
        }
        else if (progressionChar == 'L')
        {
            progressionState = NodeEnums.ProgressionState.LATE;
        }
        else
        {
            IS_INCORRECT = true;
        }

        char numNodesChar = nameLevel[1];
        if (numNodesChar == '1')
        {
            numNodes = 1;
        }
        else if (numNodesChar == '2')
        {
            numNodes = 2;
        }
        else
        {
            IS_INCORRECT = true;
        }

        isTutorial = name[3] == 'T';
    }

    //public void SaveToWorkaround()
    //{
    //    enemyWavesWorkaround = new EnemyWaveWorkaround[enemyWaves.Length];
    //    for (int i = 0; i < enemyWaves.Length; i++)
    //    {
    //        enemyWavesWorkaround[i] = new EnemyWaveWorkaround(enemyWaves[i]);
    //    }
    //}
    //public void LoadFromWorkaround()
    //{
    //    enemyWaves = new EnemyWave[enemyWavesWorkaround.Length];
    //    for (int i = 0; i < enemyWaves.Length; i++)
    //    {
    //        enemyWaves[i] = new EnemyWave(enemyWavesWorkaround[i]);
    //    }
    //}


    public void SetEnemyWaves(EnemyWave[] enemyWaves)
    {
        this.enemyWaves = enemyWaves;
    }

    public void Init(PathNode startNode)
    {
        currentWave = 0;
        activeEnemies = 0;
        this.startNode = startNode;
        stopForced = false;

        // UNCOMMENT AFTER FIXING
        ValidateJSONFormat();
        EnemyWaveJSONManager.LoadEnemyWave(this, false);
    }


    public IEnumerator SpawnCurrentWaveEnemies(Transform spawnTransform)
    {
        yield return new WaitForSeconds(delayWaveStart * GameTime.TimeScale);

        activeEnemies = enemyWaves[currentWave].GetEnemyCount();

        if (activeEnemies == 0) // If empty wave, end
        {
            EndWave();
        }
        else
        {
            foreach (EnemyInWave enemyInWave in enemyWaves[currentWave].enemiesInWave)
            {
                if (stopForced) break;

                for (int i = 0; i < enemyInWave.NumberOfSpawns; ++i)
                {
                    if (stopForced) break;

                    SpawnEnemy(enemyInWave.EnemyType, spawnTransform);

                    yield return new WaitForSeconds(enemyInWave.DelayBeforeSpawn * GameTime.TimeScale);
                }                
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
