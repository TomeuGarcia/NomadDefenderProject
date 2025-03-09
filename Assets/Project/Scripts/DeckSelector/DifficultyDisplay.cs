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

        private bool _isLeftmost;
        private bool _isRightmost;
        private bool _isUnlocked;

        public void Init(bool isLeftmost, bool isRightmost, bool isUnlocked)
        {
            _isLeftmost = isLeftmost;
            _isRightmost = isRightmost;
            _isUnlocked = isUnlocked;
        }

        public void ApplyState(Button leftArrow, Button rightArrow, 
            GameObject lockedObject, GameObject runButton, GameObject watcher)
        {
            leftArrow.interactable = !_isLeftmost;
            rightArrow.interactable = !_isRightmost;

            lockedObject.SetActive(!_isUnlocked);
            runButton.SetActive(_isUnlocked);
            
            watcher.SetActive(_isRightmost);
        }
    }


    [Header("DIFFICULTY CONFIG")]
    [SerializeField] private GameDifficultyConfig _gameDifficultyConfig;

    [Header("HIGHSCORE")] 
    [SerializeField] private DeckSelectorHighscoreDisplayer _highscoreDisplayer;

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
    public Difficulty CurrentDifficultyDisplay { get; private set; }

    private GameDifficultyType _selectedGameDifficultyType;


    public void Init()
    {
        _selectedGameDifficultyType = _gameDifficultyConfig.CurrentGameDifficulty;
        
        for (int i = 0; i < _difficulties.Length; ++i)
        {
            GameDifficultyType difficultyType = (GameDifficultyType)i;
            bool isLeftmost = i == 0;
            bool isRightmost = i == _difficulties.Length - 1;
            bool isUnlocked = _gameDifficultyConfig.UnlockedGameDifficulties.Contains(difficultyType);

            if (_demoManagerConfig.DemoEnabled && difficultyType == GameDifficultyType.Hard)
            {
                isUnlocked = false;
            }

            _difficulties[i].Init(isLeftmost, isRightmost, isUnlocked);
        }
        
        gameObject.SetActive(false);
        _textDecoder.gameObject.SetActive(false);
    }

    public void FirstTimeShow()
    {
        gameObject.SetActive(true);
        _textDecoder.gameObject.SetActive(true);
        
        SilentUpdateDifficulty();
    }

    
    private void OnEnable()
    {
        _lockNotifier.OnMousePressed += PressedLock; 
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

    private void SilentUpdateDifficulty()
    {
        _gameDifficultyConfig.SetDifficulty(_selectedGameDifficultyType);

        int difficultyIndex = (int)_selectedGameDifficultyType;
        CurrentDifficultyDisplay = _difficulties[difficultyIndex];
        CurrentDifficultyDisplay.ApplyState(_leftArrow, _rightArrow, _lockedObject, _runButton, _watcher);
        DecodeDifficultyDisplay(_difficulties[difficultyIndex]);
    }
    private void UpdateDifficulty()
    {
        SilentUpdateDifficulty();        
        _highscoreDisplayer.Show();
        GameAudioManager.GetInstance().PlayCardSelected();
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
        //GameAudioManager.GetInstance().PlayCardInfoMoveShown();
    }

    public void ButtonUnhover()
    {
        //GameAudioManager.GetInstance().PlayCardInfoMoveHidden();
    }
}
