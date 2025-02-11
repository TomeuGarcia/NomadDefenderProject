public static partial class AchievementDefinitions
{
    public static class HardDifficultyVictory
    {
        public static void Check(GameDifficultyType currentDifficulty)
        {
            if (currentDifficulty == GameDifficultyType.Hard)
            {
                AchievementsManager.UnlockAchievement(AchievementType.HardDifficulty_Victory);
            }
        }
    }
}