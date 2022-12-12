using System.Collections;
using UnityEngine;
using TMPro;

public class EnemyWaveManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI debugText; // works for 1 waveSpawner

    [SerializeField] private EnemyWaveSpawner[] enemyWaveSpawners;
    [SerializeField] private PathNode[] startPathNodes;


    private int currentWaves = 0;
    private int activeWaves = 0;
    private bool started = false;


    public delegate void EnemyWaveManagerAction();
    public static event EnemyWaveManagerAction OnAllWavesFinished;
    public static event EnemyWaveManagerAction OnWaveFinished;



    private void Awake()
    {
        activeWaves = enemyWaveSpawners.Length;
        for (int i = 0; i< enemyWaveSpawners.Length; i++)
        {
            enemyWaveSpawners[i].Init(startPathNodes[i]);
            enemyWaveSpawners[i].OnWaveFinished += FinishWave;
            enemyWaveSpawners[i].OnLastWaveFinished += FinishLastWave;
        }

        debugText.text = "Play a card to start Enemy Waves";
        StartCoroutine(WaitForStart());

        HandBuildingCards.OnCardPlayed += StartAfterFirstCardPlayed;
    }
    private void OnEnable()
    {
        TDGameManager.OnGameOverStart += ForceStopWaves;
        SceneLoader.OnSceneForceQuit += ForceStopWaves;
    }
    private void OnDisable()
    {
        TDGameManager.OnGameOverStart -= ForceStopWaves;
        SceneLoader.OnSceneForceQuit -= ForceStopWaves;
    }



    private void StartAfterFirstCardPlayed()
    {
        HandBuildingCards.OnCardPlayed -= StartAfterFirstCardPlayed;
        started = true;
    }


    private IEnumerator WaitForStart()
    {
        yield return new WaitUntil(() => started);

        // Start all parallel waves at once
        for (int i = 0; i < enemyWaveSpawners.Length; i++)
        {
            StartWave(enemyWaveSpawners[i]);
        }
    }


    private void StartWave(EnemyWaveSpawner enemyWaveSpawner)
    {
        ++currentWaves;
        StartCoroutine(enemyWaveSpawner.SpawnCurrentWaveEnemies());
        
        debugText.text = "Wave " + (enemyWaveSpawner.currentWave+1) + "/" + enemyWaveSpawner.numWaves;/* + 
            " (Enemies: " + enemyWaveSpawner.activeEnemies + ")";*/
    }


    private IEnumerator StartNextWave(EnemyWaveSpawner enemyWaveSpawner)
    {
        debugText.text = "Waiting for new wave...";
        if(OnWaveFinished != null) OnWaveFinished();

        yield return new WaitForSeconds(enemyWaveSpawner.delayBetweenWaves);

        StartWave(enemyWaveSpawner);
    }

    private void FinishWave(EnemyWaveSpawner enemyWaveSpawner)
    {        
        if (--currentWaves == 0)
        {
            ////////
            /// Invoke event  Start new waves here 
            ////////

            foreach (EnemyWaveSpawner enemyWaveSpawnerI in enemyWaveSpawners)
            {
                StartCoroutine(StartNextWave(enemyWaveSpawnerI));
            }            
        }
    }

    private void FinishLastWave(EnemyWaveSpawner enemyWaveSpawner)
    {
        // Unsubscribe events
        enemyWaveSpawner.OnWaveFinished -= FinishWave;
        enemyWaveSpawner.OnLastWaveFinished -= FinishLastWave;

        if (--activeWaves == 0)
        {
            if (OnAllWavesFinished != null) OnAllWavesFinished();
            debugText.text = "All waves finished";
        }
    }


    private void ForceStopWaves()
    {
        for (int i = 0; i < enemyWaveSpawners.Length; i++)
        {
            enemyWaveSpawners[i].ForceStopWave();
        }
    }
}
