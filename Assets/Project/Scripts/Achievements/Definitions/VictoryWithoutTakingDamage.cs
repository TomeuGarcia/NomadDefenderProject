public static partial class AchievementDefinitions
{
    public static class VictoryWithoutTakingDamage
    {
        public static void Check(int totalDamageTaken)
        {
            if (totalDamageTaken < 0)
            {
                AchievementsManager.UnlockAchievement(AchievementType.VictoryWithoutTakingDamage);
            }
        }
    }
}