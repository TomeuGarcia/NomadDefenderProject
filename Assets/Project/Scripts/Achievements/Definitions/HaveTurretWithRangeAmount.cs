public static partial class AchievementDefinitions
{
    public static class HaveTurretWithRangeAmount
    {
        public static void Check(TurretStatsSnapshot turretStats)
        {
            const float amountToEqualOrSurpass = 5f;
            if (turretStats.RadiusRange >= amountToEqualOrSurpass)
            {
                AchievementsManager.UnlockAchievement(AchievementType.HaveTurretWithRangeAmount);
            }
        }
    }
}