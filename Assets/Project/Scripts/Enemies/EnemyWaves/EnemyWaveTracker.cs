

using System;

public class EnemyWaveTracker
{
    private EnemyWaveSpawner[] _spawners;
    public int CurrentWaveIndex { get; private set; }
    public int EnemyCountInCurrentWave { get; private set; }

    public Action OnEnemyCountUpdate;
    
    
    
    public void Init(EnemyWaveSpawner[] spawners)
    {
        _spawners = spawners;
        
        CurrentWaveIndex = 0;
        EnemyCountInCurrentWave = 0;

        foreach (EnemyWaveSpawner spawner in _spawners)
        {
            spawner.OnEnemyFromWaveSpawned += OnEnemyFromWaveSpawned;
        }
    }

    public void Cleanup()
    {
        foreach (EnemyWaveSpawner spawner in _spawners)
        {
            spawner.OnEnemyFromWaveSpawned -= OnEnemyFromWaveSpawned;
        }
    }


    private void OnEnemyFromWaveSpawned(EnemyTypeConfig enemyTypeConfig)
    {
        ++EnemyCountInCurrentWave;
        OnEnemyCountUpdate?.Invoke();
    }
    
    public void OnWaveFinished()
    {
        ++CurrentWaveIndex;
        EnemyCountInCurrentWave = 0;
    }
}