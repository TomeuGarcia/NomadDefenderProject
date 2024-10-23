public static partial class AchievementDefinitions
{
    public static class StarterDeck_Victory_Berserker
    {
        public static void Check(bool isUsingBerserkerDeck)
        {
            if (isUsingBerserkerDeck)
            {
                AchievementsManager.UnlockAchievement(AchievementType.StarterDeck_Victory_Berserker);
            }
        }
    }
}