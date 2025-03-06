public static partial class AchievementDefinitions
{
    public static class HaveTurretWithRangeAmount
    {
        public static void Check(TurretStatsSnapshot turretStats)
        {
            const float amountToEqualOrSurpass = 7f;
            if (turretStats.RadiusRange >= amountToEqualOrSurpass)
            {
                AchievementsManager.UnlockAchievement(AchievementType.HaveTurretWithRangeAmount);
            }
        }
    }
}