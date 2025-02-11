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

        public string DisplayText => _displayText;
        public DecodingParameters DecodingParameters => _decodingParameters;
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
    [SerializeField] private MeshRenderer _crossMesh;
    private Material _crossMat;
    [SerializeField] private Difficulty[] _difficulties;

    private GameDifficultyType _selectedGameDifficultyType;


    private void Awake()
    {
        _crossMat = _crossMesh.material;

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

            _watcher.SetActive(false);
        }
        else if(_selectedGameDifficultyType == GameDifficultyType.Hard)
        {
            _leftArrow.interactable = true;
            _rightArrow.interactable = false;

            _watcher.SetActive(true);
        }
        else
        {
            _leftArrow.interactable = true;
            _rightArrow.interactable = true;

            _watcher.SetActive(false);
        }

        DecodeDifficultyDisplay(_difficulties[(int)_selectedGameDifficultyType]);
    }

    private void DecodeDifficultyDisplay(Difficulty difficulty)
    {
        _textDecoder.StopAllCoroutines();

        _textDecoder.SetDecodingParameters(difficulty.DecodingParameters);
        _textDecoder.ResetDecoder();
        _textDecoder.SetTextStrings(difficulty.DisplayText);
        _textDecoder.Activate();
    }
}
