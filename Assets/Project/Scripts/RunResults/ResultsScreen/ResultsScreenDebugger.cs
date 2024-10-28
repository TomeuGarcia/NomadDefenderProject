using System;
using UnityEngine;

public class ResultsScreenDebugger : MonoBehaviour
{ 
    [SerializeField] private RunState _runState;
    [SerializeField] private CardDeckAsset _starterDeck;

    private void Awake()
    {
        _runState.Init(_starterDeck, null);
        _runState.DebugOverwriteWithRandomData();
    }
}