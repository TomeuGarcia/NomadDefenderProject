public static partial class AchievementDefinitions
{
    
    public static class DiscoverAllProjectilesAndAbilities
    {
        public static void Check(CardCollectionDataStorage cardCollectionDataStorage)
        {
            if (cardCollectionDataStorage.AllProjectilesDiscovered() &&
                cardCollectionDataStorage.AllPassiveAbilitiesDiscovered())
            {
                AchievementsManager.UnlockAchievement(AchievementType.DiscoverAllProjectilesAndAbilities);
            }
        }
    }
}