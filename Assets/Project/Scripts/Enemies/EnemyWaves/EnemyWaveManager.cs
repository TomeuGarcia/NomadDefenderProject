using System.Collections;
using UnityEngine;
using TMPro;
using DG.Tweening;
using System;

public class EnemyWaveManager : MonoBehaviour
{
    [SerializeField] private Transform enemySpawnTransform;
    [SerializeField] private GameObject canvas;

    [SerializeField] private TextMeshProUGUI debugText; // works for 1 waveSpawner

    [SerializeField] private EnemyWaveSpawner[] enemyWaveSpawners;
    [SerializeField] private PathNode[] startPathNodes;

    [SerializeField] ConsoleDialogSystem consoleDialog;

    [SerializeField] bool isTutorial;
    private int currentWaves = 0;
    private int activeWaves = 0;
    private bool started = false;

    private IEnumerator[] waveCoroutines;
    private Vector3 lastEnemyPos;


    public delegate void EnemyWaveManagerAction();
    public static event EnemyWaveManagerAction OnAllWavesFinished;
    public static event EnemyWaveManagerAction OnWaveFinished;
    public static event EnemyWaveManagerAction OnStartNewWaves;

    

    [SerializeField] TextLine textLine;

    [Header("FEEDBACK")]
    [SerializeField] LastEnemyKIllAnimation lastEnemyKIllAnimation;


    private void Awake()
    {
        //canvas.SetActive(false);
        activeWaves = enemyWaveSpawners.Length;
        for (int i = 0; i< enemyWaveSpawners.Length; i++)
        {
            enemyWaveSpawners[i].Init(startPathNodes[i]);
            enemyWaveSpawners[i].OnWaveFinished += FinishWave;
            enemyWaveSpawners[i].OnLastWaveFinished += FinishLastWave;
        }
        waveCoroutines = new IEnumerator[enemyWaveSpawners.Length];


        StartCoroutine(WaitForStart());

        HandBuildingCards.OnCardPlayed += StartAfterFirstCardPlayed;
        Enemy.OnEnemyDeathDropCurrency += OnEnemyDeath;
    }

    private void OnEnable()
    {
        CardDrawer.OnStartSetupBattleCanvases += ActivateCanvas;
        TDGameManager.OnGameOverStart += ForceStopWaves;
        SceneLoader.OnSceneForceQuit += ForceStopWaves;
    }
    private void OnDisable()
    {
        CardDrawer.OnStartSetupBattleCanvases -= ActivateCanvas;
        TDGameManager.OnGameOverStart -= ForceStopWaves;
        SceneLoader.OnSceneForceQuit -= ForceStopWaves;
    }

    private void ActivateCanvas()
    {
        //canvas.SetActive(true);
        PrintConsoleLine(TextTypes.INSTRUCTION, "Play a card to start Enemy Waves",true);
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
            StartWave(enemyWaveSpawners[i], enemySpawnTransform, i);
        }
    }


    private void StartWave(EnemyWaveSpawner enemyWaveSpawner, Transform enemySpawnTransform, int index)
    {
        ++currentWaves;
        waveCoroutines[index] = enemyWaveSpawner.SpawnCurrentWaveEnemies(enemySpawnTransform);
        StartCoroutine(waveCoroutines[index]);


        //set the textline.text to the needed string and call dialog system.printLine
        PrintConsoleLine(TextTypes.SYSTEM, "Wave " + (enemyWaveSpawner.currentWave + 1) + "/" + enemyWaveSpawner.numWaves,true);


       /* debugText.text = "Wave " + (enemyWaveSpawner.currentWave+1) + "/" + enemyWaveSpawner.numWaves;/* + 
            " (Enemies: " + enemyWaveSpawner.activeEnemies + ")";*/
    }

    void PrintConsoleLine(TextTypes type, string text)
    {
        if (!isTutorial){ 
        textLine.textType = type;
        textLine.text = text;
        consoleDialog.PrintLine(textLine);
        }
    }
    void PrintConsoleLine(TextTypes type, string text, bool clearBeforeWritting)
    {
        if (!isTutorial)
        {
            if (clearBeforeWritting)
                consoleDialog.Clear();

            textLine.textType = type;
            textLine.text = text;
            consoleDialog.PrintLine(textLine);
        }
    }
    private IEnumerator StartNextWave(EnemyWaveSpawner enemyWaveSpawner, int index)
    {
        if(OnWaveFinished != null) OnWaveFinished();

        yield return new WaitForSeconds(enemyWaveSpawner.delayBetweenWaves);

        StartWave(enemyWaveSpawner, enemySpawnTransform, index);
    }

    private void FinishWave(EnemyWaveSpawner enemyWaveSpawner)
    {        
        if (--currentWaves == 0)
        {
            ////////
            /// Invoke event  Start new waves here 
            if (OnStartNewWaves != null) OnStartNewWaves();
            ////////


            for (int i = 0; i < enemyWaveSpawners.Length; i++)
            {
                StartCoroutine(StartNextWave(enemyWaveSpawners[i], i));
            }
            PrintConsoleLine(TextTypes.SYSTEM, "Waiting for new wave...");
            
        }
    }

    private void FinishLastWave(EnemyWaveSpawner enemyWaveSpawner)
    {
        // Unsubscribe events
        enemyWaveSpawner.OnWaveFinished -= FinishWave;
        enemyWaveSpawner.OnLastWaveFinished -= FinishLastWave;

        if (--activeWaves == 0)
        {
            StartCoroutine(LastWaveAnimation());
        }
    }

    private IEnumerator LastWaveAnimation()
    {
        if (OnAllWavesFinished != null) OnAllWavesFinished();

        yield return StartCoroutine(lastEnemyKIllAnimation.StartAnimation(lastEnemyPos));

        if (OnAllWavesFinished != null) OnAllWavesFinished();
        PrintConsoleLine(TextTypes.SYSTEM, "All waves finished", true);
    }


    private void ForceStopWaves()
    {
        for (int i = 0; i < enemyWaveSpawners.Length; i++)
        {
            enemyWaveSpawners[i].ForceStopWave();
        }
    }

    private void OnEnemyDeath(Enemy enemy)
    {
        lastEnemyPos = enemy.Position;
    }

    private void Update()
    {
        // TO DELETE
        if(Input.GetKeyDown(KeyCode.L))
        {
            for (int i = 0; i < enemyWaveSpawners.Length; i++)
            {
                StopCoroutine(waveCoroutines[i]);
                currentWaves = enemyWaveSpawners[i].SetToLastWave();
                StartNextWave(enemyWaveSpawners[i], i);
            }
        }
    }
}
