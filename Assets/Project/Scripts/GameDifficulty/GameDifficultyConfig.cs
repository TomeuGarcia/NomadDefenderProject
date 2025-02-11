using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameDifficultyConfig", 
    menuName = SOAssetPaths.GAMEDIFFICULTY + "GameDifficultyConfig")]
public class GameDifficultyConfig : ScriptableObject, IGameDifficultySettingsSource
{
    [Header("DIFFICULTY SELECTION")]
    [SerializeField] private GameDifficultyType _gameDifficulty = GameDifficultyType.Easy;
    
    [Header("SETTINGS")]
    [SerializeField] private GameDifficultySettings _easy;
    [Space(5)]
    [SerializeField] private GameDifficultySettings _normal;
    [Space(5)]
    [SerializeField] private GameDifficultySettings _hard;

    public GameDifficultyType CurrentGameDifficulty => _gameDifficulty;
    

    public void SetDifficulty(GameDifficultyType gameDifficulty)
    {
        _gameDifficulty = gameDifficulty;
    }

    public GameDifficultySettings GetDifficultySettings()
    {
        switch (_gameDifficulty)
        {
            case GameDifficultyType.Easy:
                return _easy;
            case GameDifficultyType.Normal:
                return _normal;
            case GameDifficultyType.Hard:
                return _hard;
        }

        return null;
    }
    
    
}
