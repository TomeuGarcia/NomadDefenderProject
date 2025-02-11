public interface IGameDifficultySettingsSource
{
    GameDifficultyType CurrentGameDifficulty { get; }
    GameDifficultySettings GetDifficultySettings();
}