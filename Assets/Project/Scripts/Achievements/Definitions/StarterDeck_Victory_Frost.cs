public static partial class AchievementDefinitions
{
    public static class StarterDeck_Victory_Frost
    {
        public static void Check(bool isUsingFrostDeck)
        {
            if (isUsingFrostDeck)
            {
                AchievementsManager.UnlockAchievement(AchievementType.StarterDeck_Victory_Frost);
            }
        }
    }
}