using System;
using Project.Scripts.Enemies.BossLevels;
using UnityEngine;

public class TileChangingBossManager : MonoBehaviour
{
    [SerializeField] private ScreenGlitcher _screenGlitcher;
    [SerializeField] private LevelTileChange[] _levelTileChanges;
    private LevelTileChange _currentLevelTileChange;
    private int _currentWaveIndex = 0;


    private void Awake()
    {
        _currentLevelTileChange = null;
        _currentWaveIndex = 0;
        
        foreach (LevelTileChange levelTileChange in _levelTileChanges)
        {
            levelTileChange.Init();
        }
    }

    private void OnEnable()
    {
        EnemyWaveManager.OnStartNewWaves += OnStartNewWaves;
    }
    private void OnDisable()
    {
        EnemyWaveManager.OnStartNewWaves -= OnStartNewWaves;
    }


    private void OnStartNewWaves()
    {
        ++_currentWaveIndex;
        
        
        if (!GetNextLevelTileChange(out LevelTileChange nextLevelTileChange))
        {
            return;
        }
        
        _currentLevelTileChange?.Hide();
        _currentLevelTileChange = nextLevelTileChange;
        _currentLevelTileChange.Show();

        StartCoroutine(
            _screenGlitcher.PlayGlitch(0f, 0.2f, 0.7f, _currentWaveIndex)
        );
    }

    private bool GetNextLevelTileChange(out LevelTileChange nextLevelTileChange)
    {
        foreach (LevelTileChange levelTileChange in _levelTileChanges)
        {
            if (levelTileChange.WaveIndex == _currentWaveIndex)
            {
                nextLevelTileChange = levelTileChange;
                return true;
            }
        }

        nextLevelTileChange = null;
        return false;
    }
}