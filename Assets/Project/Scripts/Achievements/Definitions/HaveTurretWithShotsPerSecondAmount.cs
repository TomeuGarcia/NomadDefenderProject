public static partial class AchievementDefinitions
{
    public static class HaveTurretWithShotsPerSecondAmount
    {
        public static void Check(TurretStatsSnapshot turretStats)
        {
            const float amountToEqualOrSurpass = 7f;
            if (1f / turretStats.ShotsPerSecondInverted >= amountToEqualOrSurpass)
            {
                AchievementsManager.UnlockAchievement(AchievementType.HaveTurretWithShotsPerSecondAmount);
            }
        }
    }
}