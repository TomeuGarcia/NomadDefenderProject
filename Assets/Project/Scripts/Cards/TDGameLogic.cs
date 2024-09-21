using System.Collections;
using UnityEngine;

public class TDGameLogic : MonoBehaviour
{
    [SerializeField] private CardDrawer _cardDrawer;
    [SerializeField] private TDBattleTutorialsManager _tdBattleTutorialsManager;
    [SerializeField] private bool _checkTutorials = true;
    
    private int _waveCounter = 0;

    
    private void OnEnable()
    {
        EnemyWaveManager.OnStartFirstWaves += OnFirstWaveStarted;
        EnemyWaveManager.OnStartNewWaves += OnNewWaveStarted;
    }

    private void OnDisable()
    {
        EnemyWaveManager.OnStartFirstWaves -= OnFirstWaveStarted;
        EnemyWaveManager.OnStartNewWaves -= OnNewWaveStarted;
    }

    private void OnFirstWaveStarted()
    {
        StartCoroutine(DoOnFirstWaveStarted());
    }
    private IEnumerator DoOnFirstWaveStarted()
    {
        ++_waveCounter;
        if (_checkTutorials)
        {
            yield return StartCoroutine(_tdBattleTutorialsManager.PlayWaveStartTutorials(_waveCounter));
        }
    }
    
    private void OnNewWaveStarted()
    {
        StartCoroutine(DoOnNewWaveStarted());
    }
    private IEnumerator DoOnNewWaveStarted()
    {
        ++_waveCounter;
        yield return StartCoroutine(_cardDrawer.DoDrawCardAfterWave());
        if (_checkTutorials)
        {
            yield return StartCoroutine(_tdBattleTutorialsManager.PlayWaveStartTutorials(_waveCounter));
        }
    }

}