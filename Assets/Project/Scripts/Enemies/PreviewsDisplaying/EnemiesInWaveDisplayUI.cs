using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.ObjectPooling;
using UnityEngine;

public class EnemiesInWaveDisplayUI : MonoBehaviour
{
    public class DisplayData
    {
        public class Entry
        {
            public readonly EnemyTypeConfig EnemyType;
            private int _currentCount;
            public readonly EnemyDisplayUI EnemyDisplayUI;

            public Entry(EnemyTypeConfig enemyType, int currentCount, EnemyDisplayUI enemyDisplayUI)
            {
                EnemyType = enemyType;
                _currentCount = currentCount;
                EnemyDisplayUI = enemyDisplayUI;
                EnemyDisplayUI.SetEnemyType(EnemyType);
                UpdateText();
            }

            public void Cleanup()
            {
                EnemyDisplayUI.Recycle();
            }
            
            public void DecrementCurrentCount()
            {
                --_currentCount;
                UpdateText();
            }

            private void UpdateText()
            {
                EnemyDisplayUI.SetText('x' + _currentCount.ToString());
            }
        }

        public Entry[] Entries { get; private set; }

        public DisplayData(Entry[] entries)
        {
            Entries = entries;
        }

        public void Cleanup()
        {
            foreach (Entry entry in Entries)
            {
                entry.Cleanup();
            }
        }

        public void Show()
        {
            foreach (Entry entry in Entries)
            {
                entry.EnemyDisplayUI.Show();
            }
        }
        public void Hide()
        {
            foreach (Entry entry in Entries)
            {
                entry.EnemyDisplayUI.Hide();
            }
        }

        public void DecrementEntry(EnemyTypeConfig enemyType)
        {
            foreach (Entry entry in Entries)
            {
                if (entry.EnemyType == enemyType)
                {
                    entry.DecrementCurrentCount();
                    return;
                }
            }
        }
    }

    
    [SerializeField] private EnemyDisplayUI _enemyDisplayPrefab;
    [SerializeField] private Transform _entriesHolder;
    private DisplayData _currentDisplayData;

    private ObjectPool _enemyDisplayPool;

    public bool IsShowing { get; private set; }


    public static EnemiesInWaveDisplayUI Instance;
    
    private void Awake()
    {
        _enemyDisplayPool = new ObjectPool(_enemyDisplayPrefab, _entriesHolder);
        _enemyDisplayPool.Init(10);
        gameObject.SetActive(false);
        IsShowing = false;
        Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }


    public void Show(DisplayData displayData)
    {
        gameObject.SetActive(true);
        _currentDisplayData = displayData;
        _currentDisplayData.Show();
        IsShowing = true;
    }
    
    public void Hide()
    {
        _currentDisplayData.Hide();
        gameObject.SetActive(false);
        IsShowing = false;
    }


    public EnemyDisplayUI ProvideEnemyDisplay()
    {
        return _enemyDisplayPool.Spawn<EnemyDisplayUI>(_entriesHolder.position, Quaternion.identity);
    }
    
}
