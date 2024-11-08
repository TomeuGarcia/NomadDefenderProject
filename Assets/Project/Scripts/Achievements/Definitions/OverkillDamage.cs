public static partial class AchievementDefinitions
{
    public static class OverkillDamage
    {
        public static void Check(int damageDealt)
        {
            const int amountToEqualOrSurpass = 500;
            if (damageDealt >= amountToEqualOrSurpass)
            {
                AchievementsManager.UnlockAchievement(AchievementType.OverkillDamage);
            }
        }
    }
}