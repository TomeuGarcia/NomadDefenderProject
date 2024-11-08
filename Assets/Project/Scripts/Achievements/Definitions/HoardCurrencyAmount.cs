public static partial class AchievementDefinitions
{
    public static class HoardCurrencyAmount
    {
        public static void Check(int currentCurrencyAmount)
        {
            const int amountToEqualOrSurpass = 1000;
            if (currentCurrencyAmount >= amountToEqualOrSurpass)
            {
                AchievementsManager.UnlockAchievement(AchievementType.HoardCurrencyAmount);
            }
        }
    }
}