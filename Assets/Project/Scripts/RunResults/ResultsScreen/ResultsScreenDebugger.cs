using System;
using UnityEngine;

public class ResultsScreenDebugger : MonoBehaviour
{ 
    [SerializeField] private bool _enabled;
    [SerializeField] private bool _victory;
    [SerializeField] private RunState _runState;
    [SerializeField] private CardDeckAsset _starterDeck;
    [SerializeField] private EnemyTypeConfig _mostDamagingEnemy;

    private void Awake()
    {
        if (!_enabled) return;
        
        _runState.Init(_starterDeck, null);
        _runState.DebugOverwriteWithRandomData(_victory, _starterDeck, _starterDeck.MakeDeckContent(), _mostDamagingEnemy);
    }
}