using System.Collections;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class EnemyWaveManager : MonoBehaviour
{
    [SerializeField] private Transform enemySpawnTransform;
    [SerializeField] private GameObject canvas;
    [SerializeField] private EnemyWaveInfoDisplayer enemyWaveInfoPrefab;

    [SerializeField] private TextMeshProUGUI debugText; // works for 1 waveSpawner

    [SerializeField] private EnemyWaveSpawner[] enemyWaveSpawners;
    [SerializeField] private PathNode[] startPathNodes;
    private EnemyWaveInfoDisplayer[] enemyWaveInfoDisplayers;

    [SerializeField] ConsoleDialogSystem consoleDialog;

    [SerializeField] bool isTutorial;
    private int currentWaves = 0;
    private int activeWaves = 0;
    private ITDGameState _tdGameState;

    private IEnumerator[] waveCoroutines;
    private Vector3 lastEnemyPos;

    [SerializeField] TextLine textLine;

    [Header("FEEDBACK")]
    [SerializeField] LastEnemyKIllAnimation lastEnemyKIllAnimation;


    [Header("ENEMY PATH TRAIL")]
    [SerializeField] private GameObject enemyPathTrailPrefab;
    private PathFollower[] enemyPathFollowerTrails;
    private bool enemyPathFollowerTrailsEnabled;
    private static Vector3 enemyPathFollowerTrailsPositionOffset = Vector3.up * 0.5f;


    public delegate void EnemyWaveManagerAction();
    public static event EnemyWaveManagerAction OnAllWavesFinished;
    public static event EnemyWaveManagerAction OnWaveFinished;
    public static event EnemyWaveManagerAction OnStartNewWaves;

    



    private void Awake()
    {
        //canvas.SetActive(false);
        activeWaves = enemyWaveSpawners.Length;

        enemyWaveInfoDisplayers = new EnemyWaveInfoDisplayer[enemyWaveSpawners.Length];

        for (int i = 0; i< enemyWaveSpawners.Length; i++)
        {
            PathNode startPathNode = startPathNodes[i];
            enemyWaveSpawners[i].Init(startPathNode);

            enemyWaveInfoDisplayers[i] = Instantiate(enemyWaveInfoPrefab, startPathNode.transform);
            enemyWaveInfoDisplayers[i].Init(startPathNode, enemyWaveSpawners[i]);
        }
        waveCoroutines = new IEnumerator[enemyWaveSpawners.Length];


        StartCoroutine(SetupEnemyPathFollowerTrails());

        
        Enemy.OnEnemyDeathGlobal += OnEnemyDeath;
        Enemy.OnEnemySuicide += OnEnemyDeath;
    }

    private void Start()
    {
        _tdGameState = ServiceLocator.GetInstance().TDGameState;
        StartCoroutine(WaitForStart());
    }

    private void OnDestroy()
    {
        Enemy.OnEnemyDeathGlobal -= OnEnemyDeath;
        Enemy.OnEnemySuicide -= OnEnemyDeath;
    }

    private void OnEnable()
    {
        CardDrawer.OnStartSetupBattleCanvases += ActivateCanvas;
        TDGameManager.OnGameOverStart += ForceStopWaves;
        TDGameManager.OnGameOverStart += PrintConsoleGameOver;
        SceneLoader.OnSceneForceQuit += ForceStopWaves;

        for (int i = 0; i < enemyWaveSpawners.Length; i++)
        {
            enemyWaveSpawners[i].OnWaveFinished += FinishWave;
            enemyWaveSpawners[i].OnLastWaveFinished += FinishLastWave;
        }

        if (enemyPathFollowerTrails != null)
        {
            for (int i = 0; i < enemyPathFollowerTrails.Length; ++i)
            {
                enemyPathFollowerTrails[i].OnPathEndReached2 += ResetEnemyPathFollowerTrailToStart;
            }
        }        
    }
    private void OnDisable()
    {
        CardDrawer.OnStartSetupBattleCanvases -= ActivateCanvas;
        TDGameManager.OnGameOverStart -= ForceStopWaves;
        TDGameManager.OnGameOverStart -= PrintConsoleGameOver;
        SceneLoader.OnSceneForceQuit -= ForceStopWaves;

        for (int i = 0; i < enemyWaveSpawners.Length; i++)
        {
            enemyWaveSpawners[i].OnWaveFinished -= FinishWave;
            enemyWaveSpawners[i].OnLastWaveFinished -= FinishLastWave;
        }

        if (enemyPathFollowerTrails != null)
        {
            for (int i = 0; i < enemyPathFollowerTrails.Length; ++i)
            {
                enemyPathFollowerTrails[i].OnPathEndReached2 -= ResetEnemyPathFollowerTrailToStart;
            }
        }

        if (activeWaves > 0)
        {
            for (int waveCoroutineI = 0; waveCoroutineI < waveCoroutines.Length; ++waveCoroutineI)
            {
                if (waveCoroutines[waveCoroutineI] != null)
                {
                    StopCoroutine(waveCoroutines[waveCoroutineI]);
                }
            }
            // TODO fix enemy Factory
        }       
        
    }

    private void ActivateCanvas()
    {
        //canvas.SetActive(true);
        PrintConsoleLine(TextTypes.INSTRUCTION, "Play a card to start Enemy Waves",true);
    }




    private IEnumerator WaitForStart()
    {
        yield return new WaitUntil(() => _tdGameState.FirstCardWasPlayed);

        // Start all parallel waves at once
        for (int i = 0; i < enemyWaveSpawners.Length; i++)
        {
            StartWave(enemyWaveSpawners[i], enemySpawnTransform, i);
        }

        StopEnemyPathFollowerTrails();
    }


    private void StartWave(EnemyWaveSpawner enemyWaveSpawner, Transform enemySpawnTransform, int index)
    {
        
        //for (int i = 0; i < enemyWaveSpawner.EnemyWaves.Length; ++i)
        //{
        //    Debug.Log("Wave " + i);
        //    for (int j = 0; j < enemyWaveSpawner.EnemyWaves[i].enemiesInWave.Length; ++j)
        //    {
        //        Debug.Log(enemyWaveSpawner.EnemyWaves[i].enemiesInWave[j].enemyType + " x" + enemyWaveSpawner.EnemyWaves[i].enemiesInWave[j].NumberOfSpawns);
        //    }
        //}

        ++currentWaves;
        waveCoroutines[index] = enemyWaveSpawner.SpawnCurrentWaveEnemies(enemySpawnTransform, this);
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
    private void PrintConsoleGameOver()
    {
        StartCoroutine(DoPrintConsoleGameOver());
    }
    private IEnumerator DoPrintConsoleGameOver()
    {
        consoleDialog.SetMaxLinesOnScreen(10);
        string[] texts = { "STAGE OVER", "StAge OVeR", "StaGE oVER", "sTAgE OVER", "STAGE oVer" };
        PrintConsoleLine(TextTypes.SYSTEM, texts[0], true);
        for (int i = 0; i < 10; ++i)
        {
            yield return new WaitForSeconds(0.15f);  
            PrintConsoleLine(TextTypes.SYSTEM, texts[Random.Range(0, texts.Length)]);
        }
    }


    private IEnumerator StartNextWave(EnemyWaveSpawner enemyWaveSpawner, int index)
    {
        if(OnWaveFinished != null) OnWaveFinished();

        enemyWaveSpawner.ReadyToStartNextWave();

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

        yield return new WaitForSeconds(2.5f);

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





    private IEnumerator SetupEnemyPathFollowerTrails()
    {
        yield return new WaitForSeconds(2f);
        
        enemyPathFollowerTrails = new PathFollower[startPathNodes.Length];
        for (int i = 0; i < startPathNodes.Length; ++i)
        {
            enemyPathFollowerTrails[i] = Instantiate(enemyPathTrailPrefab, startPathNodes[i].Position + enemyPathFollowerTrailsPositionOffset, Quaternion.identity,
                enemySpawnTransform).GetComponent<PathFollower>();

            enemyPathFollowerTrails[i].Init(startPathNodes[i].GetNextNode(), startPathNodes[i].GetDirectionToNextNode(), enemyPathFollowerTrailsPositionOffset, 0f);
            enemyPathFollowerTrails[i].SetMoveSpeed(0f);
            enemyPathFollowerTrails[i].OnPathEndReached2 += ResetEnemyPathFollowerTrailToStart;
        }

        StartEnemyPathFollowerTrails();
    }

    private void StartEnemyPathFollowerTrails()
    {
        for (int i = 0; i < enemyPathFollowerTrails.Length; ++i) 
        {
            enemyPathFollowerTrails[i].SetMoveSpeed(1f);
        }
        enemyPathFollowerTrailsEnabled = true;
    }

    private void StopEnemyPathFollowerTrails()
    {
        for (int i = 0; i < enemyPathFollowerTrails.Length; ++i)
        {
            enemyPathFollowerTrails[i].SetMoveSpeed(0f);
        }
        enemyPathFollowerTrailsEnabled = false;
    }

    private void ResetEnemyPathFollowerTrailToStart(PathFollower enemyPathFollowerTrail)
    {
        for (int i = 0; i < enemyPathFollowerTrails.Length; ++i)
        {
            if (enemyPathFollowerTrails[i] == enemyPathFollowerTrail)
            {
                StartCoroutine(DoResetEnemyPathFollowerTrailToStart(enemyPathFollowerTrail, startPathNodes[i]));
                return;
            }
        }
    }

    private IEnumerator DoResetEnemyPathFollowerTrailToStart(PathFollower enemyPathFollowerTrail, PathNode startNode)
    {
        enemyPathFollowerTrail.SetMoveSpeed(0f);
        yield return new WaitForSeconds(0.5f);
        enemyPathFollowerTrail.gameObject.SetActive(false);

        if (!enemyPathFollowerTrailsEnabled)
        {
            yield break;
        }

        enemyPathFollowerTrail.transform.position = startNode.Position + enemyPathFollowerTrailsPositionOffset;
        enemyPathFollowerTrail.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);

        if (!enemyPathFollowerTrailsEnabled)
        {            
            yield break;
        }

        enemyPathFollowerTrail.SetMoveSpeed(1f);
        enemyPathFollowerTrail.Init(startNode.GetNextNode(), startNode.GetDirectionToNextNode(), enemyPathFollowerTrailsPositionOffset, 0f);
    }


    public void HideWaveSpawnersInfoDisplay()
    {
        foreach (var enemyWaveInfoDisplayer in enemyWaveInfoDisplayers)
        {
            enemyWaveInfoDisplayer.Hide();
        }
    }

}
