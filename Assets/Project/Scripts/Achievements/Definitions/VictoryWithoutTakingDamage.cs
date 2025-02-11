public static partial class AchievementDefinitions
{
    public static class VictoryWithLessThanDamage
    {
        public static void Check(int totalDamageTaken)
        {
            const int damageThreshold = 5;
            if (totalDamageTaken <= damageThreshold)
            {
                AchievementsManager.UnlockAchievement(AchievementType.VictoryWithLessThanDamage);
            }
        }
    }
}