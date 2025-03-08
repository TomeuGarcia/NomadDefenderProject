
public static partial class AchievementDefinitions
{
    public static class VictoryHardRunNoDamage
    {
        public static void Check(int totalDamageTaken, GameDifficultyType currentDifficulty)
        {
            const int damageThreshold = 0;
            if (totalDamageTaken <= damageThreshold && currentDifficulty == GameDifficultyType.Hard)
            {
                AchievementsManager.UnlockAchievement(AchievementType.VictoryHardRunNoDamage);
            }
        }
    }
}