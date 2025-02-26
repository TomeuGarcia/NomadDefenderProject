using System;
using UnityEngine;

public class ResultsScreenDebugger : MonoBehaviour
{ 
    [SerializeField] private bool _enabled;
    [SerializeField] private bool _victory;
    [SerializeField] private RunState _runState;
    [SerializeField] private CardDeckAsset _starterDeck;
    [SerializeField] private EnemyTypeConfig _mostDamagingEnemy;

    [SerializeField] private bool _unlockDifficulty = false;
    [SerializeField] private bool _unlockStarterDeck = false;

    private void Awake()
    {
        if (!_enabled) return;
        
        _runState.Init(_starterDeck, null);
        _runState.DebugOverwriteWithRandomData(_victory, _starterDeck, _starterDeck.MakeDeckContent(), _mostDamagingEnemy);
        _runState.Finish(_victory, _unlockDifficulty, _unlockStarterDeck);
    }
}