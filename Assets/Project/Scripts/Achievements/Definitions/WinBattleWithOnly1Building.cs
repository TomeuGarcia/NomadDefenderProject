public static partial class AchievementDefinitions
{
    public static class WinBattleWithOnly1Building
    {
        public static void Check(int totalBuildingsPlaced)
        {
            if (totalBuildingsPlaced == 1)
            {
                AchievementsManager.UnlockAchievement(AchievementType.WinBattleWithOnly1Building);
            }
        }
    }
}