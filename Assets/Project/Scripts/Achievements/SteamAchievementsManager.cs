

using System.Collections.Generic;
using Steamworks;

public class SteamAchievementsManager : IAchievementsManager
{
    private readonly Dictionary<AchievementType, string> _achievementsMap;

    public SteamAchievementsManager(AchievementsManagerConfig config)
    {
        _achievementsMap = config.MakeAchievementsMap();
    }
    
    public void UnlockAchievement(AchievementType achievementType)
    {
        SteamUserStats.SetAchievement(_achievementsMap[achievementType]);
        SteamUserStats.StoreStats();
    }
}