using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableCannonsShootSequenceWithEnemySpawns : MonoBehaviour
{
    [System.Serializable]
    private class Beat
    {
        [SerializeField, Min(0)] private int _waveIndex;
        [SerializeField, Min(1)] private int _enemyCount;
        [SerializeField, Min(0)] private float _delay;
        [Space(5)] 
        [SerializeField] private DisableCannonsShootLogic.ShootingFunctions _shootingFunction;
        [SerializeField, Min(1)] private int _numberOfShots;
        
        public int WaveIndex => _waveIndex;
        public int EnemyCount => _enemyCount;
        public float Delay => _delay;
        public DisableCannonsShootLogic.ShootingFunctions ShootingFunction => _shootingFunction;
        public int NumberOfShots => _numberOfShots;
    }


    [Header("REFERENCES")] 
    [SerializeField] private EnemyWaveManager _enemyWaveManager;
    [SerializeField] private DisableCannonsShootLogic _shootLogic;

    [Header("SEQUENCE")] 
    [SerializeField] private Beat[] _beats;
    private List<Beat> _remainingBeats;

    private EnemyWaveTracker EnemyWaveTracker => _enemyWaveManager.EnemyWaveTracker;


    private void Awake()
    {
        _remainingBeats = new List<Beat>(_beats);
    }

    private void OnEnable()
    {
        EnemyWaveTracker.OnEnemyCountUpdate += OnEnemyCountUpdate;
    }
    private void OnDisable()
    {
        EnemyWaveTracker.OnEnemyCountUpdate -= OnEnemyCountUpdate;
    }


    private void OnEnemyCountUpdate()
    {
        int currentWaveIndex = EnemyWaveTracker.CurrentWaveIndex;
        int enemyCountInCurrentWave = EnemyWaveTracker.EnemyCountInCurrentWave;
    
        for (int i = _remainingBeats.Count - 1; i >= 0; --i)
        {
            Beat beat = _remainingBeats[i];
            bool beatMatches = beat.WaveIndex == currentWaveIndex && beat.EnemyCount == enemyCountInCurrentWave;
            
            if (!beatMatches)
            {
                continue;
            }

            StartCoroutine(BeatTrigger(beat));
            
            _remainingBeats.RemoveAt(i);            
        }
    }

    private IEnumerator BeatTrigger(Beat beat)
    {
        yield return StartCoroutine(GameTime.WaitForSeconds(beat.Delay));
        
        _shootLogic.Shoot(beat.ShootingFunction, beat.NumberOfShots);
    }
}