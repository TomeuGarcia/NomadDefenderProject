using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
using DG.Tweening;
public class EnemyWaveManager : MonoBehaviour
{
    [SerializeField] private Transform enemySpawnTransform;
    [SerializeField] private GameObject canvas;
    [SerializeField] private EnemyWaveInfoDisplayer enemyWaveInfoPrefab;

    [SerializeField] private TextMeshProUGUI debugText; // works for 1 waveSpawner


    [System.Serializable]
    private class PathStartData
    {
        [SerializeField] private EnemyWaveSpawner _enemyWaveSpawner;
        [SerializeField] private PathNode _startNode;

        public PathNode StartNode => _startNode;
        public EnemyWaveSpawner EnemyWaveSpawner => _enemyWaveSpawner;
        public Coroutine ActiveWaveCoroutine { get; private set; }
        public EnemyWaveInfoDisplayer WaveDisplayer { get; private set; }
        
        private NodePathViewer _pathViewer;
        private MouseOverlapNotifier _mouseOverNotifier;
        private bool _showingPathPermanently = false;

        public void InitWaveDisplayer(EnemyWaveInfoDisplayer waveDisplayer, EnemiesInWaveDisplayUI enemiesInWaveDisplayUI)
        {
            WaveDisplayer = waveDisplayer;
            WaveDisplayer.Init(_startNode, _enemyWaveSpawner, enemiesInWaveDisplayUI, _mouseOverNotifier);
        }

        public void MergeWaveDisplayer(EnemyWaveSpawner overlappingEnemyWaveSpawner)
        {
            WaveDisplayer.Merge(overlappingEnemyWaveSpawner);
        }
        
        public void SetActiveWaveCoroutine(Coroutine activeWaveCoroutine)
        {
            ActiveWaveCoroutine = activeWaveCoroutine;
        }

        public void InitNodePathViewer(NodePathViewer pathViewer, MouseOverlapNotifier mouseOverNotifier)
        {
            _pathViewer = pathViewer;
            _mouseOverNotifier = mouseOverNotifier;
            _mouseOverNotifier.OnMouseEntered += ShowPathViewer;
            _mouseOverNotifier.OnMouseExited += HidePathViewer;
        }

        ~PathStartData()
        {
            _mouseOverNotifier.OnMouseEntered -= ShowPathViewer;
            _mouseOverNotifier.OnMouseExited -= HidePathViewer;
        }

        private void ShowPathViewer()
        {
            if (_showingPathPermanently) return;
            _pathViewer.Show(_startNode);
        }
        private void HidePathViewer()
        {
            if (_showingPathPermanently) return;
            _pathViewer.Hide();
        }
        public void StartShowingPathViewerPermanently()
        {
            ShowPathViewer();
            _showingPathPermanently = true;
        }
        public void StopShowingPathViewerPermanently()
        {
            _showingPathPermanently = false;
            HidePathViewer();
        }
    }

    [System.Serializable]
    public class PathEndData
    {
        [SerializeField] private PathNode _endNode;
        [SerializeField] private PathLocation _endLocation;
        public PathNode EndNode => _endNode;
        public PathLocation EndLocation => _endLocation;
    }

    [SerializeField] private PathStartData[] _pathsStartData;
    [SerializeField] private PathEndData[] _pathsEndData;


    [SerializeField] ConsoleDialogSystem consoleDialog;

    [SerializeField] bool isTutorial;
    private int currentWaves = 0;
    private int activeWaves = 0;
    private ITDGameState _tdGameState;

    private Vector3 lastEnemyPos;

    [SerializeField] TextLine textLine;

    [Header("FEEDBACK")]
    [SerializeField] LastEnemyKIllAnimation lastEnemyKIllAnimation;

    
    [Header("ENEMY PATH TRAIL")]
    [SerializeField] private GameObject enemyPathTrailPrefab;
    [SerializeField] private NodePathViewer _pathViewerPrefab;
    [SerializeField] private MouseOverlapNotifier _pathViewerMouseNotifierPrefab;
    private PathFollower[] enemyPathFollowerTrails;
    private bool enemyPathFollowerTrailsEnabled;
    private static Vector3 enemyPathFollowerTrailsPositionOffset = Vector3.zero;// Vector3.up * 0.5f;

    public bool WaveStartPaused { get; set; } = false;
    

    public delegate void EnemyWaveManagerAction();
    public static event EnemyWaveManagerAction OnAllWavesFinished;
    public static event EnemyWaveManagerAction OnAllWavesFinishedEnd;
    public static event EnemyWaveManagerAction OnWaveFinished;
    public static event EnemyWaveManagerAction OnStartNewWaves;
    public static event EnemyWaveManagerAction OnStartFirstWaves;

    private EnemyAttackDestination _enemiesAttackDestination;

    private bool _gameOverAlreadyPlayed = false;
    
    
    private void Init()
    {
        //canvas.SetActive(false);
        _enemiesAttackDestination = new EnemyAttackDestination(_pathsEndData);
        activeWaves = _pathsStartData.Length;

        Dictionary<PathNode, PathStartData> repeatedStartPathNodes = new(_pathsStartData.Length);

        for (int i = 0; i< _pathsStartData.Length; i++)
        {
            PathStartData pathStartData = _pathsStartData[i];
            PathNode startPathNode = pathStartData.StartNode;
            pathStartData.EnemyWaveSpawner.Init(startPathNode);
            
            if (repeatedStartPathNodes.TryGetValue(startPathNode, out PathStartData firstPathStartData))
            {
                firstPathStartData.MergeWaveDisplayer(pathStartData.EnemyWaveSpawner);
                continue;
            }

            MouseOverlapNotifier enemySpawnMouseOverNotifier =
                Instantiate(_pathViewerMouseNotifierPrefab, pathStartData.StartNode.transform);

            pathStartData.InitNodePathViewer(
                Instantiate(_pathViewerPrefab, pathStartData.StartNode.transform), enemySpawnMouseOverNotifier);
                
            pathStartData.InitWaveDisplayer(
                Instantiate(enemyWaveInfoPrefab, startPathNode.transform), EnemiesInWaveDisplayUI.Instance);
                
            repeatedStartPathNodes.Add(startPathNode, pathStartData);
        }

        
        StartCoroutine(SetupEnemyPathFollowerTrails());

        
        Enemy.OnEnemyDeathGlobal += OnEnemyDeath;
        Enemy.OnEnemySuicide += OnEnemyDeath;
    }

    private void Start()
    {
        Init();
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
        
        for (int i = 0; i < _pathsStartData.Length; i++)
        {
            EnemyWaveSpawner enemyWaveSpawner = _pathsStartData[i].EnemyWaveSpawner;
            enemyWaveSpawner.OnWaveFinished += FinishWave;
            enemyWaveSpawner.OnLastWaveFinished += FinishLastWave;
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

        for (int i = 0; i < _pathsStartData.Length; i++)
        {
            EnemyWaveSpawner enemyWaveSpawner = _pathsStartData[i].EnemyWaveSpawner;
            enemyWaveSpawner.OnWaveFinished -= FinishWave;
            enemyWaveSpawner.OnLastWaveFinished -= FinishLastWave;
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
            for (int waveCoroutineI = 0; waveCoroutineI < _pathsStartData.Length; ++waveCoroutineI)
            {
                if (_pathsStartData[waveCoroutineI].ActiveWaveCoroutine != null)
                {
                    StopCoroutine(_pathsStartData[waveCoroutineI].ActiveWaveCoroutine);
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
        for (int i = 0; i < _pathsStartData.Length; i++)
        {
            StartWave(_pathsStartData[i].EnemyWaveSpawner, enemySpawnTransform, i);
        }
        OnStartFirstWaves?.Invoke();

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
        _pathsStartData[index].SetActiveWaveCoroutine(
            StartCoroutine(enemyWaveSpawner.SpawnCurrentWaveEnemies(enemySpawnTransform, this, _enemiesAttackDestination)));


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

        yield return new WaitUntil(() => !WaveStartPaused);

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
            OnStartNewWaves?.Invoke();
            ////////


            for (int i = 0; i < _pathsStartData.Length; i++)
            {
                StartCoroutine(StartNextWave(_pathsStartData[i].EnemyWaveSpawner, i));
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

        //if (!_gameOverAlreadyPlayed)
        {
            yield return StartCoroutine(lastEnemyKIllAnimation.StartAnimation(lastEnemyPos));
        }

        yield return new WaitUntil(() => !WaveStartPaused);
        
        if (OnAllWavesFinished != null) OnAllWavesFinished();

        yield return null;
        OnAllWavesFinishedEnd?.Invoke();
        yield return new WaitForSeconds(2.5f);

        PrintConsoleLine(TextTypes.SYSTEM, "All waves finished", true);
    }


    private void ForceStopWaves()
    {
        _gameOverAlreadyPlayed = true;
        
        for (int i = 0; i < _pathsStartData.Length; i++)
        {
            _pathsStartData[i].EnemyWaveSpawner.ForceStopWave();
        }
    }

    private void OnEnemyDeath(Enemy enemy)
    {
        lastEnemyPos = enemy.Position;
    }


    


    private IEnumerator SetupEnemyPathFollowerTrails()
    {
        /**/
        yield return new WaitForSeconds(2f);
        
        enemyPathFollowerTrails = new PathFollower[_pathsStartData.Length];
        for (int i = 0; i < _pathsStartData.Length; ++i)
        {
            enemyPathFollowerTrails[i] = Instantiate(enemyPathTrailPrefab, 
                _pathsStartData[i].StartNode.Position + enemyPathFollowerTrailsPositionOffset, Quaternion.identity,
                enemySpawnTransform).GetComponent<PathFollower>();

            enemyPathFollowerTrails[i].Init(_pathsStartData[i].StartNode, enemyPathFollowerTrailsPositionOffset, 0f);
            enemyPathFollowerTrails[i].SetMoveSpeedMultiplier(0f);
            enemyPathFollowerTrails[i].OnPathEndReached2 += ResetEnemyPathFollowerTrailToStart;
        }
        /**/

        yield return null;
        StartEnemyPathFollowerTrails();
    }

    private void StartEnemyPathFollowerTrails()
    {
        /**/
        for (int i = 0; i < enemyPathFollowerTrails.Length; ++i) 
        {
            enemyPathFollowerTrails[i].SetMoveSpeedMultiplier(1f);
        }
        enemyPathFollowerTrailsEnabled = true;
        /**/
        
        /*
        foreach (PathStartData pathStartData in _pathsStartData)
        {
            pathStartData.StartShowingPathViewerPermanently();
        }
        */
    }

    private void StopEnemyPathFollowerTrails()
    {
        /**/
        for (int i = 0; i < enemyPathFollowerTrails.Length; ++i)
        {
            enemyPathFollowerTrails[i].SetMoveSpeedMultiplier(0f);
        }
        enemyPathFollowerTrailsEnabled = false; 
        /**/
        
        /*      
        foreach (PathStartData pathStartData in _pathsStartData)
        {
          pathStartData.StopShowingPathViewerPermanently();
        }
        */
    }

    private void ResetEnemyPathFollowerTrailToStart(PathFollower enemyPathFollowerTrail)
    {
        for (int i = 0; i < enemyPathFollowerTrails.Length; ++i)
        {
            if (enemyPathFollowerTrails[i] == enemyPathFollowerTrail)
            {
                StartCoroutine(DoResetEnemyPathFollowerTrailToStart(enemyPathFollowerTrail, _pathsStartData[i].StartNode));
                return;
            }
        }
    }

    private IEnumerator DoResetEnemyPathFollowerTrailToStart(PathFollower enemyPathFollowerTrail, PathNode startNode)
    {
        enemyPathFollowerTrail.SetMoveSpeedMultiplier(0f);
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

        enemyPathFollowerTrail.SetMoveSpeedMultiplier(1f);
        enemyPathFollowerTrail.Init(startNode, enemyPathFollowerTrailsPositionOffset, 0f);
    }


    public void HideWaveSpawnersInfoDisplay()
    {
        for (int i = 0; i < _pathsStartData.Length; ++i)
        {
            _pathsStartData[i].WaveDisplayer.Hide();
        }
    }

}
