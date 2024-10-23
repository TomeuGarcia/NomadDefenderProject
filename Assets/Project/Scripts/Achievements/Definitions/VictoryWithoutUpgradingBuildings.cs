public static partial class AchievementDefinitions
{
    public static class VictoryWithoutUpgradingBuildings
    {
        public static void Check(int totalBuildingUpgrades)
        {
            if (totalBuildingUpgrades < 0)
            {
                AchievementsManager.UnlockAchievement(AchievementType.VictoryWithoutUpgradingBuildings);
            }
        }
    }
}