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



    private void OnDestroy()
    {
        enemyWaveSpawner.OnWaveStartSpawning -= OnWaveStartSpawning;
        enemyWaveSpawner.OnEnemySpawn -= OnWaveSpawnsEnemy;

        canvasHolder.DOComplete();
    }

    public void Init(PathNode pathNode, EnemyWaveSpawner enemyWaveSpawner)
    {
        transform.position = pathNode.Position + Vector3.up * 1.2f;

        this.enemyWaveSpawner = enemyWaveSpawner;
        SetupForNewEnemyWave();

        canvasHolder.DOBlendableMoveBy(Vector3.up * 0.3f, 3f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);

        enemyWaveSpawner.OnWaveStartSpawning += OnWaveStartSpawning;
        enemyWaveSpawner.OnEnemySpawn += OnWaveSpawnsEnemy;
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
    }
    
    private void OnWaveSpawnsEnemy(EnemyWaveSpawner enemyWaveSpawner)
    {
        --numberOfEnemiesToSpawn;
        SetNumberOfEnemiesText(numberOfEnemiesToSpawn);

        if (NoEnemiesLeft())
        {
            Hide();
        }
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

}
