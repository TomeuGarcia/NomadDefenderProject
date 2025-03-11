using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private GameDifficultyConfig _gameDifficultyConfig;
    
    [Header("AUDIO")]
    [SerializeField] private AudioMixersController audioMixersController;
    [SerializeField] private Slider _masterSoundSlider;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _sfxSlider;

    [Header("SCREEN")]
    [SerializeField] private ScreenOptionsController _screenOptionsController;

    [Header("NEW GAME")]
    [SerializeField] private Button _newGameButton;
    [SerializeField] private NewGameManager _newGameManager;



    private string PathToFile_StreamingAssets => Application.streamingAssetsPath + "/JSONfiles/Settings/";
    private string PathToFile_Persistent => Application.persistentDataPath + "/Settings/";
    private const string FILE_NAME = "Options.json";
    

    public void Init()
    {
        _masterSoundSlider.onValueChanged.AddListener(SetMasterMixerVolume);
        _musicSlider.onValueChanged.AddListener(SetMusicMixerVolume);
        _sfxSlider.onValueChanged.AddListener(SetSFXMixerVolume);

        _gameDifficultyConfig.StartupInit();

        LoadOptions(
            out float masterSoundSliderValue,
            out float musicSoundSliderValue,
            out float sfxSoundSliderValue,
            out bool fullScreen,
            out GameDifficultyType gameDifficultyType,
            out GameDifficultyType[] unlockedGameDifficultyType
            );

        
        _masterSoundSlider.value = masterSoundSliderValue;
        _musicSlider.value = musicSoundSliderValue;
        _sfxSlider.value = sfxSoundSliderValue;
        
        _screenOptionsController.Init(fullScreen);
        
        _gameDifficultyConfig.SetDifficulty(gameDifficultyType);
        _gameDifficultyConfig.Init(unlockedGameDifficultyType);

        SetMasterMixerVolume(_masterSoundSlider.value);
        SetMusicMixerVolume(_musicSlider.value);
        SetSFXMixerVolume(_sfxSlider.value);
        
        _newGameManager.Init();
        _newGameButton.onClick.AddListener(_newGameManager.Show);
    }

    private void OnDestroy()
    {
        SaveOptions(true);
    }

    public void Show()
    {
        SetNewGameNotAvailable();
    }
    public void Hide()
    {
    }

    public void SetNewGameAvailable()
    {
        _newGameButton.gameObject.SetActive(true);
    }
    private void SetNewGameNotAvailable()
    {
        _newGameButton.gameObject.SetActive(false);
    }


    private void SetMasterMixerVolume(float sliderValue)
    {
        audioMixersController.SetMasterMixerVolume(sliderValue);
    }
    private void SetMusicMixerVolume(float sliderValue)
    {
        audioMixersController.SetMusicMixerVolume(sliderValue);
    }
    private void SetSFXMixerVolume(float sliderValue)
    {
        audioMixersController.SetSFXMixerVolume(sliderValue);
    }


    
    
    
    
    [System.Serializable]
    private class SaveDataWrapper
    {
        [SerializeField] public float MasterSoundVolume = 1.0f;
        [SerializeField] public float MusicSoundVolume = 1.0f;
        [SerializeField] public float SFXSoundVolume = 1.0f;
        [SerializeField] public bool FullScreen = true;
        [SerializeField] public GameDifficultyType GameDifficultyType = GameDifficultyType.Normal;
        [SerializeField] public GameDifficultyType[] UnlockedGameDifficulties =
            { GameDifficultyType.Easy, GameDifficultyType.Normal };

        public SaveDataWrapper(float masterSoundVolume, float musicSoundVolume, float sfxSoundVolume, bool fullScreen,
            GameDifficultyType gameDifficultyType, GameDifficultyType[] unlockedGameDifficulties)
        {
            MasterSoundVolume = masterSoundVolume;
            MusicSoundVolume = musicSoundVolume;
            SFXSoundVolume = sfxSoundVolume;
            FullScreen = fullScreen;
            GameDifficultyType = gameDifficultyType;
            UnlockedGameDifficulties = unlockedGameDifficulties;
        }

        public void CheckFixes()
        {
            if (UnlockedGameDifficulties == null)
            {
                UnlockedGameDifficulties = new[]
                    { GameDifficultyType.Easy, GameDifficultyType.Normal };
            }
        }
    }
    

    private void LoadOptions(
        out float masterSoundSliderValue, 
        out float musicSoundSliderValue, 
        out float sfxSoundSliderValue, 
        out bool fullScreen, 
        out GameDifficultyType gameDifficultyType,
        out GameDifficultyType[] unlockedGameDifficulties
        )
    {
        CheckFile();

        string storedContent = File.ReadAllText(PathToFile_Persistent + FILE_NAME);
        SaveDataWrapper storedData = JsonUtility.FromJson<SaveDataWrapper>(storedContent);
        storedData.CheckFixes();

        masterSoundSliderValue = storedData.MasterSoundVolume;
        musicSoundSliderValue = storedData.MusicSoundVolume;
        sfxSoundSliderValue = storedData.SFXSoundVolume;
        fullScreen = storedData.FullScreen;
        gameDifficultyType = storedData.GameDifficultyType;
        unlockedGameDifficulties = storedData.UnlockedGameDifficulties;
    }

    private void SaveOptions(bool toPersistent)
    {
        SaveDataWrapper dataToStore = new SaveDataWrapper(
            _masterSoundSlider.value,
            _musicSlider.value,
            _sfxSlider.value,
            _screenOptionsController.IsCurrentlyFullscreen,
            _gameDifficultyConfig.CurrentGameDifficulty,
            _gameDifficultyConfig.UnlockedGameDifficulties
            );

        string path = (toPersistent ? PathToFile_Persistent : PathToFile_StreamingAssets) + FILE_NAME;
        string contentToStore = JsonUtility.ToJson(dataToStore);
        File.WriteAllText(path, contentToStore);
    }
    
    private void CheckFile()
    {
        if (Directory.Exists(PathToFile_Persistent) && File.Exists(PathToFile_Persistent + FILE_NAME))
        {
            return;
        }
        
        if (!Directory.Exists(PathToFile_StreamingAssets) || !File.Exists(PathToFile_StreamingAssets + FILE_NAME))
        {
            Directory.CreateDirectory(PathToFile_StreamingAssets);
            SaveOptions(false);
        }
        
        Directory.CreateDirectory(PathToFile_Persistent);
        File.Copy(PathToFile_StreamingAssets + FILE_NAME, PathToFile_Persistent + FILE_NAME);
        SaveOptions(true);
    }

}
