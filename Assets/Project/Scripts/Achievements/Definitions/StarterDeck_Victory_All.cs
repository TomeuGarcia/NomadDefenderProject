public static partial class AchievementDefinitions
{
    public static class StarterDeck_Victory_All
    {
        public static void Check()
        {
            IAchievementsManager achievementsManager = AchievementsManager;
            bool wonWithAllDecks = 
                achievementsManager.IsAchievementUnlocked(AchievementType.StarterDeck_Victory_Frost) &&
                achievementsManager.IsAchievementUnlocked(AchievementType.StarterDeck_Victory_Repeater) &&
                achievementsManager.IsAchievementUnlocked(AchievementType.StarterDeck_Victory_Currency) &&
                achievementsManager.IsAchievementUnlocked(AchievementType.StarterDeck_Victory_Berserker);
            
            if (wonWithAllDecks)
            {
                achievementsManager.UnlockAchievement(AchievementType.StarterDeck_Victory_All);
            }
        }
    }
}