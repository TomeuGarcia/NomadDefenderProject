using DG.Tweening;
using System;
using System.Linq;
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

    [Header("VIEW")]
    [SerializeField] private TextDecoder _textDecoder;
    [SerializeField] private Button _leftArrow;
    [SerializeField] private Button _rightArrow;
    [SerializeField] private GameObject _watcher;
    [SerializeField] private GameObject _lockedObject;
    [SerializeField] private Light _bottomLight;
    [SerializeField] private GameObject _runButton;
    [SerializeField] private MouseOverNotifier _lockNotifier;
    [SerializeField] private Difficulty[] _difficulties;

    private GameDifficultyType _selectedGameDifficultyType;


    private void OnEnable()
    {
        _lockNotifier.OnMousePressed += PressedLock; 
        _selectedGameDifficultyType = _gameDifficultyConfig.CurrentGameDifficulty;
        UpdateDifficulty();
    }

    private void OnDisable()
    {
        _lockNotifier.OnMousePressed -= PressedLock;
    }

    private void PressedLock()
    {
        _lockedObject.transform.DOComplete();
        _lockedObject.transform.DOShakePosition(0.15f, 0.05f, 100, 90, false, false, ShakeRandomnessMode.Full);
        GameAudioManager.GetInstance().PlayError();
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

            if (_demoManagerConfig.DemoEnabled || 
                _gameDifficultyConfig.UnlockedGameDifficulties.Contains(_selectedGameDifficultyType))
            {
                _lockedObject.SetActive(true);
                _runButton.SetActive(false);
            }

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

    public void ButtonHover()
    {
        GameAudioManager.GetInstance().PlayCardHovered();
    }

    public void ButtonUnhover()
    {
        GameAudioManager.GetInstance().PlayCardHoverExit();
    }
}
