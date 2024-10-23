public static partial class AchievementDefinitions
{
    public static class HaveAmountOfBuildingsSimultaneously
    {
        public static void Check(int currentAmountOfPlacedBuildings)
        {
            const int amountToEqualOrSurpass = 8;
            if (currentAmountOfPlacedBuildings >= amountToEqualOrSurpass)
            {
                AchievementsManager.UnlockAchievement(AchievementType.HaveAmountOfBuildingsSimultaneously);
            }
        }
    }
}