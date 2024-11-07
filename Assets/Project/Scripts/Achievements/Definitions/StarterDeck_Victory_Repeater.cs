public static partial class AchievementDefinitions
{
    public static class StarterDeck_Victory_Repeater
    {
        public static void Check(bool isUsingRepeaterDeck)
        {
            if (isUsingRepeaterDeck)
            {
                AchievementsManager.UnlockAchievement(AchievementType.StarterDeck_Victory_Repeater);
            }
        }
    }
}