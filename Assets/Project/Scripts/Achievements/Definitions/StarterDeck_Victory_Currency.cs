public static partial class AchievementDefinitions
{
    public static class StarterDeck_Victory_Currency
    {
        public static void Check(bool isUsingCurrencyDeck)
        {
            if (isUsingCurrencyDeck)
            {
                AchievementsManager.UnlockAchievement(AchievementType.StarterDeck_Victory_Currency);
            }
        }
    }
}