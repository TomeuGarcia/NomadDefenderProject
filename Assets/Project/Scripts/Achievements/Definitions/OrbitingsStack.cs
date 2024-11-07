public static partial class AchievementDefinitions
{
    public static class OrbitingsStack
    {
        public static void Check(int currentlyActiveOrbitings)
        {
            const int amountToEqualOrSurpass = 60;
            if (currentlyActiveOrbitings >= amountToEqualOrSurpass)
            {
                AchievementsManager.UnlockAchievement(AchievementType.OrbitingsStack);
            }
        }
    }
}