using System;
using NaughtyAttributes;
using UnityEngine;

public class LevelTileChangesTester : MonoBehaviour
{
    [SerializeField] private LevelTileChange[] _levelTileChanges;

    [SerializeField, Min(-1)] private int _changesIndex;


    private void OnValidate()
    {
        _changesIndex = Mathf.Clamp(_changesIndex, -1, _levelTileChanges.Length - 1);
    }


    [Button()]
    private void TestChanges()
    {
        for (int i = 0; i < _levelTileChanges.Length; ++i)
        {
            _levelTileChanges[i].Init(false);
        }
        
        for (int i = 0; i <= _changesIndex; ++i)
        {
            int previousI = i - 1;
            if (previousI > -1)
            {
                _levelTileChanges[previousI].Hide();
            }
            _levelTileChanges[i].Show();
        }
    }
}