public static partial class AchievementDefinitions
{
    
    public static class UnlockAllStarterDecks
    {
        public static void Check(int numberOfUnlockedStarterDecks)
        {
            const int amountToEqualOrSurpass = 4;
            if (numberOfUnlockedStarterDecks >= amountToEqualOrSurpass)
            {
                AchievementsManager.UnlockAchievement(AchievementType.UnlockAllStarterDecks);
            }
        }
    }
}