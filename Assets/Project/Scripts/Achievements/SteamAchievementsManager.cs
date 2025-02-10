

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Steamworks;

public class SteamAchievementsManager : IAchievementsManager
{
    private readonly DemoManagerConfig _demoManagerConfig;
    private readonly Dictionary<AchievementType, string> _achievementsMap;

    public SteamAchievementsManager(AchievementsManagerConfig config, DemoManagerConfig demoManagerConfig)
    {
        _demoManagerConfig = demoManagerConfig;
        _achievementsMap = config.MakeAchievementsMap();
    }

    public void UnlockAchievement(AchievementType achievementType)
    {
        if (!SteamManager.Initialized || _demoManagerConfig.DemoEnabled)
        {
            return;
        }
        
        SteamUserStats.SetAchievement(_achievementsMap[achievementType]);
        SteamUserStats.StoreStats();
    }

    public bool IsAchievementUnlocked(AchievementType achievementType)
    {
        if (!SteamManager.Initialized)
        {
            return false;
        }

        if (SteamUserStats.GetAchievement(_achievementsMap[achievementType], out bool achieved))
        {
            return achieved;
        }

        return false;
    }

    public void LockAllAchievements()
    {
        foreach (AchievementType achievementType in Enum.GetValues(typeof(AchievementType)))
        {
            SteamUserStats.ClearAchievement(_achievementsMap[achievementType]);
            SteamUserStats.StoreStats();
        }
    }
    
}