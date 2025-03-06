using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
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


    private HashSet<GameDifficultyType> _unlockedGameDifficulties;
    public GameDifficultyType[] UnlockedGameDifficulties => _unlockedGameDifficulties.ToArray();

    public void Init(GameDifficultyType[] unlockedGameDifficulties)
    {
        _unlockedGameDifficulties = new HashSet<GameDifficultyType>(unlockedGameDifficulties);
    }

    public void ResetState()
    {
        _unlockedGameDifficulties = new HashSet<GameDifficultyType>
            { GameDifficultyType.Easy, GameDifficultyType.Normal };

        SetDifficulty(GameDifficultyType.Normal);
    }

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
    
    
    
    public void UnlockDifficulty(GameDifficultyType gameDifficulty)
    {
        if (!_unlockedGameDifficulties.Contains(gameDifficulty))
        {
            _unlockedGameDifficulties.Add(gameDifficulty);
        }
    }


    [Button()]
    private void DebugUnlockAllDifficulties()
    {
        int difficultiesCount = Enum.GetValues(typeof(GameDifficultyType)).Length;

        for (int i = 0; i < difficultiesCount; ++i)
        {
            UnlockDifficulty((GameDifficultyType)i);
        }
    }
    
    [Button()]
    private void DebugRevertUnlockDifficulties()
    {
        ResetState();
    }
    
}
