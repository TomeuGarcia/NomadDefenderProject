public static partial class AchievementDefinitions
{
    public static class VictoryWithLessThanDamage
    {
        public static void Check(int totalDamageTaken, GameDifficultyType currentDifficulty)
        {
            const int damageThreshold = 5;
            if (totalDamageTaken <= damageThreshold && currentDifficulty == GameDifficultyType.Hard)
            {
                AchievementsManager.UnlockAchievement(AchievementType.VictoryWithLessThanDamage);
            }
        }
    }
}