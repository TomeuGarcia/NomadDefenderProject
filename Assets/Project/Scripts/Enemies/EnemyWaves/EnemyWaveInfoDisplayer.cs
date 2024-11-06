using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class EnemyWaveInfoDisplayer : MonoBehaviour
{
    [SerializeField] private Transform canvasHolder;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI numberOfEnemiesText;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color allEnemiesSpawnedColor = Color.cyan;

    private float showHideDuration = 0.5f;

    private EnemyWaveSpawner enemyWaveSpawner;
    int numberOfEnemiesToSpawn = 0;

    private EnemiesInWaveDisplayUI _enemiesInWaveDisplayUI;
    private EnemiesInWaveDisplayUI.DisplayData _currentEnemiesDisplayData;

    private MouseOverNotifier _mouseOverNotifier;
    
    private void OnDestroy()
    {
        enemyWaveSpawner.OnWaveStartSpawning -= OnWaveStartSpawning;
        enemyWaveSpawner.OnEnemyFromWaveSpawned -= OnWaveSpawnsEnemy;

        _mouseOverNotifier.OnMouseEntered -= ShowDisplayUI;
        _mouseOverNotifier.OnMouseExited -= HideDisplayUI;
        
        canvasHolder.DOComplete();
    }

    public void Init(PathNode pathNode, EnemyWaveSpawner enemyWaveSpawner, EnemiesInWaveDisplayUI enemiesInWaveDisplayUI,
        MouseOverNotifier mouseOverNotifier)
    {
        _enemiesInWaveDisplayUI = enemiesInWaveDisplayUI;
        transform.position = pathNode.Position + Vector3.up * 1.2f;

        this.enemyWaveSpawner = enemyWaveSpawner;
        SetupForNewEnemyWave();

        canvasHolder.DOBlendableMoveBy(Vector3.up * 0.3f, 3f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);

        enemyWaveSpawner.OnWaveStartSpawning += OnWaveStartSpawning;
        enemyWaveSpawner.OnEnemyFromWaveSpawned += OnWaveSpawnsEnemy;
        
        _mouseOverNotifier = mouseOverNotifier;
        _mouseOverNotifier.OnMouseEntered += ShowDisplayUI;
        _mouseOverNotifier.OnMouseExited += HideDisplayUI;

        InitEnemiesDisplayDataUI();
    }


    private void SetNumberOfEnemiesText(int numberOfEnemies)
    {
        numberOfEnemiesText.text = numberOfEnemies.ToString();
        numberOfEnemiesText.color = NoEnemiesLeft() ? allEnemiesSpawnedColor : normalColor;
    }
    
    private async void SetupForNewEnemyWave()
    {
        numberOfEnemiesToSpawn = enemyWaveSpawner.EnemyWaves[enemyWaveSpawner.currentWave].GetEnemyCount();

        int total = numberOfEnemiesToSpawn;


        if (NoEnemiesLeft())
        {
            Hide();
        }
        else
        {
            await Show();
        }

        int n = 0;
        DOTween.To(
            () => n,
            (value) => { n = value; SetNumberOfEnemiesText(n); },
            total,
            0.5f
            )
            .OnComplete(() => SetNumberOfEnemiesText(numberOfEnemiesToSpawn));
    }


    private void OnWaveStartSpawning(EnemyWaveSpawner enemyWaveSpawner)
    {
        SetupForNewEnemyWave();

        InitEnemiesDisplayDataUI();
        if (_enemiesInWaveDisplayUI.IsShowing)
        {
            ShowDisplayUI();
        }
    }
    
    private void OnWaveSpawnsEnemy(EnemyTypeConfig enemyType)
    {
        --numberOfEnemiesToSpawn;
        SetNumberOfEnemiesText(numberOfEnemiesToSpawn);

        if (NoEnemiesLeft())
        {
            Hide();
        }

        _currentEnemiesDisplayData.DecrementEntry(enemyType);
    }


    private bool NoEnemiesLeft()
    {
        return numberOfEnemiesToSpawn == 0;
    }

    private async Task Show()
    {
        canvasGroup.DOFade(1.0f, showHideDuration);
        await Task.Delay((int)(showHideDuration * 1000));
    }
    public void Hide()
    {
        canvasGroup.DOFade(0.0f, showHideDuration);
    }





    private void InitEnemiesDisplayDataUI()
    {
        _currentEnemiesDisplayData?.Cleanup();
        EnemyInWave[] enemiesInWave = enemyWaveSpawner.CurrentEnemyWave.enemiesInWave;
        Dictionary<EnemyTypeConfig, int> groupedEnemiesInWave = new();
        foreach (EnemyInWave enemyInWave in enemiesInWave)
        {
            EnemyTypeConfig enemyType = enemyInWave.EnemyType;
            if (groupedEnemiesInWave.ContainsKey(enemyType))
            {
                groupedEnemiesInWave[enemyType] += 1;
            }
            else
            {
                groupedEnemiesInWave.Add(enemyType, 1);
            }
        }
        
        List<EnemiesInWaveDisplayUI.DisplayData.Entry> currentEnemyEntries = new();
        foreach (var groupedEnemyInWave in groupedEnemiesInWave)
        {
            currentEnemyEntries.Add(new EnemiesInWaveDisplayUI.DisplayData.Entry(
                groupedEnemyInWave.Key,
                groupedEnemyInWave.Value,
                _enemiesInWaveDisplayUI.ProvideEnemyDisplay()
            ));
        }
        _currentEnemiesDisplayData = new EnemiesInWaveDisplayUI.DisplayData(currentEnemyEntries.ToArray());
    }
    
    private void ShowDisplayUI()
    {
        _enemiesInWaveDisplayUI.Show(_currentEnemiesDisplayData);
    }

    private void HideDisplayUI()
    {
        _enemiesInWaveDisplayUI.Hide();
    }
}
