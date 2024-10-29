using System;
using UnityEngine;

public class ResultsScreenDebugger : MonoBehaviour
{ 
    [SerializeField] private RunState _runState;
    [SerializeField] private CardDeckAsset _starterDeck;
    [SerializeField] private EnemyTypeConfig _mostDamagingEnemy;

    private void Awake()
    {
        _runState.Init(_starterDeck, null);
        _runState.DebugOverwriteWithRandomData(_starterDeck, _starterDeck.MakeDeckContent(), _mostDamagingEnemy);
    }
}