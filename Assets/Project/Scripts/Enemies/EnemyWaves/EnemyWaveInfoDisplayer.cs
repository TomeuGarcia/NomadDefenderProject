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

    private List<EnemyWaveSpawner> _overlappingEnemyWaveSpawners;
    int numberOfEnemiesToSpawn = 0;

    private EnemiesInWaveDisplayUI _enemiesInWaveDisplayUI;
    private EnemiesInWaveDisplayUI.DisplayData _currentEnemiesDisplayData;

    private MouseOverlapNotifier _mouseOverNotifier;

    private GameObject _mouseHoverViewToggle;
    
    
    private void OnDestroy()
    {
        foreach (EnemyWaveSpawner overlappedEnemyWaveSpawner in _overlappingEnemyWaveSpawners)
        {
            overlappedEnemyWaveSpawner.OnWaveStartSpawning -= OnWaveStartSpawning;
            overlappedEnemyWaveSpawner.OnEnemyFromWaveSpawned -= OnWaveSpawnsEnemy;
        }


        _mouseOverNotifier.OnMouseEntered -= ShowDisplayUI;
        _mouseOverNotifier.OnMouseExited -= HideDisplayUI;
        
        canvasHolder.DOComplete();
    }

    public void Init(PathNode pathNode, EnemyWaveSpawner enemyWaveSpawner, EnemiesInWaveDisplayUI enemiesInWaveDisplayUI,
        MouseOverlapNotifier mouseOverNotifier)
    {
        _enemiesInWaveDisplayUI = enemiesInWaveDisplayUI;
        transform.position = pathNode.Position + Vector3.up * 1.2f;

        _overlappingEnemyWaveSpawners = new List<EnemyWaveSpawner>() { enemyWaveSpawner };
        SetupForNewEnemyWave();

        canvasHolder.DOBlendableMoveBy(Vector3.up * 0.3f, 3f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);

        enemyWaveSpawner.OnWaveStartSpawning += OnWaveStartSpawning;
        enemyWaveSpawner.OnEnemyFromWaveSpawned += OnWaveSpawnsEnemy;
        
        _mouseOverNotifier = mouseOverNotifier;
        _mouseOverNotifier.OnMouseEntered += ShowDisplayUI;
        _mouseOverNotifier.OnMouseExited += HideDisplayUI;

        _currentEnemiesDisplayData = new EnemiesInWaveDisplayUI.DisplayData();
        InitEnemiesDisplayDataUI();

        _mouseHoverViewToggle = pathNode.transform.GetChild(3).gameObject;
        _mouseHoverViewToggle.SetActive(false);
    }

    public void Merge(EnemyWaveSpawner overlappingEnemyWaveSpawner)
    {
        _overlappingEnemyWaveSpawners.Add(overlappingEnemyWaveSpawner);
        
        overlappingEnemyWaveSpawner.OnWaveStartSpawning += OnWaveStartSpawning;
        overlappingEnemyWaveSpawner.OnEnemyFromWaveSpawned += OnWaveSpawnsEnemy;
    }


    private void SetNumberOfEnemiesText(int numberOfEnemies)
    {
        numberOfEnemiesText.text = numberOfEnemies.ToString();
        numberOfEnemiesText.color = NoEnemiesLeft() ? allEnemiesSpawnedColor : normalColor;
    }
    
    private async void SetupForNewEnemyWave()
    {
        numberOfEnemiesToSpawn = 0;
        foreach (EnemyWaveSpawner overlappingEnemyWaveSpawner in _overlappingEnemyWaveSpawners)
        {
            numberOfEnemiesToSpawn += overlappingEnemyWaveSpawner.CurrentEnemyWave.GetEnemyCount();
        }
        
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
        if (_enemiesInWaveDisplayUI.IsShowingDisplayData(_currentEnemiesDisplayData))
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
        List<EnemyInWave> enemiesInWave = new List<EnemyInWave>(numberOfEnemiesToSpawn);
        
        foreach (EnemyWaveSpawner overlappingEnemyWaveSpawner in _overlappingEnemyWaveSpawners)
        {
            enemiesInWave.AddRange(overlappingEnemyWaveSpawner.CurrentEnemyWave.enemiesInWave);
        }
        
        
        Dictionary<EnemyTypeConfig, (int, bool)> groupedEnemiesInWave = new();
        foreach (EnemyInWave enemyInWave in enemiesInWave)
        {
            EnemyTypeConfig realEnemyType = enemyInWave.EnemyType;
            EnemyTypeConfig enemyType = realEnemyType.NonArmored;
            bool hasArmor = realEnemyType.BaseStats.Armor > 0;
            
            if (groupedEnemiesInWave.ContainsKey(enemyType))
            {
                (int, bool) existingEntry = groupedEnemiesInWave[enemyType];
                groupedEnemiesInWave[enemyType] = (
                    existingEntry.Item1 + enemyInWave.NumberOfSpawns, 
                    existingEntry.Item2 || hasArmor);
            }
            else
            {
                groupedEnemiesInWave.Add(enemyType, (enemyInWave.NumberOfSpawns, hasArmor));
            }
        }
        
        List<EnemiesInWaveDisplayUI.DisplayData.Entry> currentEnemyEntries = new();
        foreach (var groupedEnemyInWave in groupedEnemiesInWave)
        {
            currentEnemyEntries.Add(new EnemiesInWaveDisplayUI.DisplayData.Entry(
                groupedEnemyInWave.Key,
                groupedEnemyInWave.Value.Item1,
                groupedEnemyInWave.Value.Item2,
                _enemiesInWaveDisplayUI.ProvideEnemyDisplay()
            ));
        }
        
        _currentEnemiesDisplayData.Reset(currentEnemyEntries.ToArray());
    }
    
    
    private void ShowDisplayUI()
    {
        _enemiesInWaveDisplayUI.Show(_currentEnemiesDisplayData);
        _mouseHoverViewToggle.SetActive(true);
    }

    private void HideDisplayUI()
    {
        if (_enemiesInWaveDisplayUI.IsShowingDisplayData(_currentEnemiesDisplayData))
        {
            _enemiesInWaveDisplayUI.Hide();
            _mouseHoverViewToggle.SetActive(false);
        }
    }
}
