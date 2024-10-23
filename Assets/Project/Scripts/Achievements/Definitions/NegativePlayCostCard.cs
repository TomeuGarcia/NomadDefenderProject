public static partial class AchievementDefinitions
{
    public static class NegativePlayCostCard
    {
        public static void Check(int cardPlayCost)
        {
            if (cardPlayCost < 0)
            {
                AchievementsManager.UnlockAchievement(AchievementType.NegativePlayCostCard);
            }
        }
    }
}