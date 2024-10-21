

using System.Collections.Generic;
using System.Threading.Tasks;
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

    public bool IsAchievementUnlocked(AchievementType achievementType)
    {
        if (SteamUserStats.GetAchievement(_achievementsMap[achievementType], out bool achieved))
        {
            return achieved;
        }

        return false;
    }

}