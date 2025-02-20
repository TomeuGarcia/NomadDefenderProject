using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableCannonsAdderWithEnemyWaves : MonoBehaviour
{
    [System.Serializable]
    private class Change
    {
        [SerializeField, Min(0)] private int _waveIndex;
        [SerializeField, Min(0)] private float _delay;
        
        public int WaveIndex => _waveIndex;
        public float Delay => _delay;
    }
    
    [Header("REFERENCES")]
    [SerializeField] private EnemyWaveManager _enemyWaveManager;
    [SerializeField] private DisableCannonsAdder _disableCannonsAdder;

    [Header("CONFIGURATION")] 
    [SerializeField] private Change[] _activationChanges;
    [SerializeField] private Change[] _deactivationChanges;

    private EnemyWaveTracker EnemyWaveTracker => _enemyWaveManager.EnemyWaveTracker;
    
    
    private void OnEnable()
    {
        EnemyWaveTracker.OnBeforeEnemyWaveFinished += OnBeforeEnemyWaveFinished;
    }
    private void OnDisable()
    {
        EnemyWaveTracker.OnBeforeEnemyWaveFinished -= OnBeforeEnemyWaveFinished;
    }


    private void OnBeforeEnemyWaveFinished()
    {
        for (int i = 0; i < _activationChanges.Length; ++i)
        {
            Change activationChange = _activationChanges[i];
            if (activationChange.WaveIndex == EnemyWaveTracker.CurrentWaveIndex)
            {
                StartCoroutine(NextActivation(activationChange.Delay));
                return;
            }
        }
        
        for (int i = 0; i < _deactivationChanges.Length; ++i)
        {
            Change deactivationChanges = _deactivationChanges[i];
            if (deactivationChanges.WaveIndex == EnemyWaveTracker.CurrentWaveIndex)
            {
                StartCoroutine(DeactivateAll(deactivationChanges.Delay));
                return;
            }
        }
    }


    private IEnumerator NextActivation(float delay)
    {
        yield return new WaitForSeconds(delay);
        _disableCannonsAdder.AddNext();
    }
    
    private IEnumerator DeactivateAll(float delay)
    {
        yield return new WaitForSeconds(delay);
        _disableCannonsAdder.RemoveAllAndReset();
    }
}
