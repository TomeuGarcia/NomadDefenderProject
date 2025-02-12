using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyDisplay : MonoBehaviour
{
    [Serializable]
    public class Difficulty
    {
        [SerializeField] private string _displayText;
        [SerializeField] private DecodingParameters _decodingParameters;
        [SerializeField] private Color _lightColor;

        public string DisplayText => _displayText;
        public DecodingParameters DecodingParameters => _decodingParameters;
        public Color LightColor => _lightColor;
    }


    [Header("DIFFICULTY CONFIG")]
    [SerializeField] private GameDifficultyConfig _gameDifficultyConfig;

    [Header("DEMO")]
    [SerializeField] private DemoManagerConfig _demoManagerConfig;
    [SerializeField] private bool _debugAlwaysHideButtons;

    [Header("VIEW")]
    [SerializeField] private TextDecoder _textDecoder;
    [SerializeField] private Button _leftArrow;
    [SerializeField] private Button _rightArrow;
    [SerializeField] private GameObject _watcher;
    [SerializeField] private GameObject _lockedObject;
    [SerializeField] private Light _bottomLight;
    [SerializeField] private GameObject _runButton;
    [SerializeField] private Difficulty[] _difficulties;

    private GameDifficultyType _selectedGameDifficultyType;


    private void Awake()
    {
        _selectedGameDifficultyType = _gameDifficultyConfig.CurrentGameDifficulty;
        UpdateDifficulty();

        //TODO - DELETE
        return;
        if (_demoManagerConfig.DemoEnabled || _debugAlwaysHideButtons)
        {
            gameObject.SetActive(false);
        }

        if (_demoManagerConfig.DemoEnabled) // DEMO only on Normal difficulty
        {
            _gameDifficultyConfig.SetDifficulty(GameDifficultyType.Normal);
        }
    }

    public void IncreaseDifficulty()
    {
        _selectedGameDifficultyType += 1;

        UpdateDifficulty();

    }

    public void DecreaseDifficulty()
    {
        _selectedGameDifficultyType -= 1;

        UpdateDifficulty();
    }

    private void UpdateDifficulty()
    {
        _gameDifficultyConfig.SetDifficulty(_selectedGameDifficultyType);

        GameAudioManager.GetInstance().PlayCardSelected();

        if(_selectedGameDifficultyType == GameDifficultyType.Easy)
        {
            _leftArrow.interactable = false;
            _rightArrow.interactable = true;

            _lockedObject.SetActive(false);
            _runButton.SetActive(true);

            _watcher.SetActive(false);
        }
        else if(_selectedGameDifficultyType == GameDifficultyType.Hard)
        {
            _leftArrow.interactable = true;
            _rightArrow.interactable = false;

            /*if (_demoManagerConfig.DemoEnabled || ) //TODO - 
            {
                _lockedObject.SetActive(true);
                _runButton.SetActive(false);
            }*/

            _watcher.SetActive(true);
        }
        else
        {
            _leftArrow.interactable = true;
            _rightArrow.interactable = true;

            _lockedObject.SetActive(false);
            _runButton.SetActive(true);

            _watcher.SetActive(false);
        }

        DecodeDifficultyDisplay(_difficulties[(int)_selectedGameDifficultyType]);
    }

    private void DecodeDifficultyDisplay(Difficulty difficulty)
    {
        _bottomLight.DOKill();
        _bottomLight.DOColor(difficulty.LightColor, 0.25f);

        _textDecoder.StopAllCoroutines();

        _textDecoder.SetDecodingParameters(difficulty.DecodingParameters);
        _textDecoder.ResetDecoder();
        _textDecoder.SetTextStrings(difficulty.DisplayText);
        _textDecoder.Activate();
    }
}
