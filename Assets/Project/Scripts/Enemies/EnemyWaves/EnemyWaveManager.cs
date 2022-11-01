using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using TMPro;

public class EnemyWaveManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI debugText; // works for 1 waveSpawner

    [SerializeField] private EnemyWaveSpawner[] enemyWaveSpawners;
    [SerializeField] private PathNode[] startPathNodes;


    private int activeWaves = 0;
    private bool started = false;


    public delegate void EnemyWaveManagerAction();
    public static event EnemyWaveManagerAction OnAllWavesFinished;


    private void Awake()
    {
        activeWaves = enemyWaveSpawners.Length;
        for (int i = 0; i< enemyWaveSpawners.Length; i++)
        {
            enemyWaveSpawners[i].Init(startPathNodes[i]);
            enemyWaveSpawners[i].OnWaveFinished += FinishWave;
            enemyWaveSpawners[i].OnLastWaveFinished += FinishLastWave;
        }

        debugText.text = "Press Q to start Enemy Wave";
        StartCoroutine(WaitForStart());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
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
        StartCoroutine(enemyWaveSpawner.SpawnCurrentWaveEnemies());
        
        debugText.text = "Wave " + (enemyWaveSpawner.currentWave+1) + "/" + enemyWaveSpawner.numWaves + 
            " (Enemies: " + enemyWaveSpawner.activeEnemies + ")";
    }


    private IEnumerator StartNextWave(EnemyWaveSpawner enemyWaveSpawner)
    {
        debugText.text = "Waiting for new wave...";

        yield return new WaitForSeconds(enemyWaveSpawner.delayBetweenWaves);

        StartWave(enemyWaveSpawner);
    }

    private void FinishWave(EnemyWaveSpawner enemyWaveSpawner)
    {        
        StartCoroutine(StartNextWave(enemyWaveSpawner));
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

}
