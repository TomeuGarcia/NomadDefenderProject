using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Scripts.ObjectPooling;
using UnityEngine;
using UnityEngine.UI;

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
                EnemyDisplayUI.Hide();
                EnemyDisplayUI.Deactivate();
                UpdateText();
                EnemyDisplayUI.PlayResetAnimation();
            }

            public void Cleanup()
            {
                Destroy(EnemyDisplayUI.gameObject);
            }
            
            public void DecrementCurrentCount()
            {
                --_currentCount;
                UpdateText();
                EnemyDisplayUI.PlayTextUpdateAnimation();
            }

            private void UpdateText()
            {
                EnemyDisplayUI.SetText('x' + _currentCount.ToString());
            }
        }

        public Entry[] Entries { get; private set; }
        
        

        public DisplayData()
        {
            Entries = Array.Empty<Entry>();
        }

        public void Reset(Entry[] entries)
        {
            Cleanup();
            Entries = entries;
        }

        private void Cleanup()
        {
            foreach (Entry entry in Entries)
            {
                entry.Cleanup();
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

        public void ActivateViews()
        {
            foreach (Entry entry in Entries)
            {
                entry.EnemyDisplayUI.Activate();
            }
        }
        public void DeactivateViews()
        {
            foreach (Entry entry in Entries)
            {
                entry.EnemyDisplayUI.Deactivate();
            }
        }
    }

    
    [Header("LOGIC")]
    [SerializeField] private EnemyDisplayUI _enemyDisplayPrefab;
    [SerializeField] private Transform _entriesHolder;
    
    [Header("ANIMATION")]
    [SerializeField] private CanvasGroup _canvasGroup;
    
    
    private DisplayData _currentDisplayData;
    private bool _isShowing;


    public static EnemiesInWaveDisplayUI Instance;
    
    private enum AnimationState { None, Showing, Hiding }

    private AnimationState _animationState;
    
    
    private void Awake()
    {
        _isShowing = false;
        Instance = this;
        _canvasGroup.alpha = 0;
        _animationState = 0;
    }

    private void OnDestroy()
    {
        Instance = null;
    }


    public void Show(DisplayData displayData)
    {
        _currentDisplayData = displayData;
        _isShowing = true;
        StopAllCoroutines();
        StartCoroutine(PlayShowAnimation());
    }

    private IEnumerator PlayShowAnimation()
    {
        _currentDisplayData.ActivateViews();
        _animationState = AnimationState.Showing;
        
        for (int i = 0; i < 2; ++i)
        {
            _canvasGroup.alpha = 0;
            yield return new WaitForSeconds(0.05f);
            GameAudioManager.GetInstance().PlayCardInfoMoveShown();
            _canvasGroup.alpha = 1;
            yield return new WaitForSeconds(0.05f);
        }
        
        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < _currentDisplayData.Entries.Length; ++i)
        {
            GameAudioManager.GetInstance().PlayCardInfoMoveShown();
            _currentDisplayData.Entries[i].EnemyDisplayUI.Show();
            yield return new WaitForSeconds(0.1f);
        }
        
        _animationState = AnimationState.None;
    }
    
    public void Hide()
    {
        _isShowing = false;
        StopAllCoroutines();
        StartCoroutine(PlayHideAnimation());
    }
    
    private IEnumerator PlayHideAnimation()
    {
        _animationState = AnimationState.Hiding;
        
        for (int i = _currentDisplayData.Entries.Length - 1; i >= 0; --i)
        {
            GameAudioManager.GetInstance().PlayCardInfoMoveHidden();
            _currentDisplayData.Entries[i].EnemyDisplayUI.Hide();
            yield return new WaitForSeconds(0.1f);
        }
        
        for (int i = 0; i < 2; ++i)
        {
            _canvasGroup.alpha = 1;
            yield return new WaitForSeconds(0.05f);
            GameAudioManager.GetInstance().PlayCardInfoMoveHidden();
            _canvasGroup.alpha = 0;
            yield return new WaitForSeconds(0.05f);
        }
        
        _currentDisplayData.DeactivateViews();
        _animationState = AnimationState.None;
    }


    public EnemyDisplayUI ProvideEnemyDisplay()
    {
        return Instantiate(_enemyDisplayPrefab, _entriesHolder);
    }

    public bool IsShowingDisplayData(DisplayData displayData)
    {
        return _isShowing && _currentDisplayData == displayData;
    }
    
}
