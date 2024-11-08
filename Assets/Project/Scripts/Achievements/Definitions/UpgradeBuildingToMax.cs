public static partial class AchievementDefinitions
{
    public static class UpgradeBuildingToMax
    {
        public static void Check(bool buildingUpgradedToMax)
        {
            if (buildingUpgradedToMax)
            {
                AchievementsManager.UnlockAchievement(AchievementType.UpgradeBuildingToMax);
            }
        }
    }
}