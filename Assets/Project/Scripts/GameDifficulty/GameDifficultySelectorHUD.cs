using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameDifficultySelectorHUD : MonoBehaviour
{
    [System.Serializable]
    private class SelectionButton
    {
        [SerializeField] private GameDifficultyType _difficulty;
        [SerializeField] private Button _button;

        private GameDifficultyConfig _gameDifficultyConfig;
        

        public void Init(GameDifficultyConfig gameDifficultyConfig)
        {
            _gameDifficultyConfig = gameDifficultyConfig;
            _button.onClick.AddListener(OnButtonClicked);

            if (gameDifficultyConfig.CurrentGameDifficulty == _difficulty)
            {
                InitSelected();
            }
        }

        public void Cleanup()
        {
            _button.onClick.RemoveAllListeners();
        }

        private void OnButtonClicked()
        {
            _gameDifficultyConfig.SetDifficulty(_difficulty);
            GameAudioManager.GetInstance().PlayCardSelected();
            _button.transform.DOComplete();
            _button.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f, 4, 0.2f);
        }

        private void InitSelected()
        {
            _button.Select();
        }
    }
    
    
    [Header("DIFFICULTY CONFIG")] 
    [SerializeField] private GameDifficultyConfig _gameDifficultyConfig;

    [Header("HUD")] 
    [SerializeField] private SelectionButton[] _buttons;

    
    private void Awake()
    {
        foreach (SelectionButton button in _buttons)
        {
            button.Init(_gameDifficultyConfig);
        }
    }
    
    private void OnDestroy()
    {
        foreach (SelectionButton button in _buttons)
        {
            button.Cleanup();
        }
    }
    
    
}
