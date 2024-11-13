using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "AchievementsTester", 
    menuName = SOAssetPaths.ACHIEVEMENTS + "AchievementsTester")]
public class AchievementsTester : ScriptableObject
{
    [SerializeField] private AchievementType _achievementToUnlock;


    [Button()]
    private void UnlockAchievement()
    {
        ServiceLocator.GetInstance().AchievementsManager.UnlockAchievement(_achievementToUnlock);

        if (ServiceLocator.GetInstance().AchievementsManager.IsAchievementUnlocked(_achievementToUnlock))
        {
            Debug.Log("Achievement " + _achievementToUnlock + " unlocked!" );
        }
        else
        {
            Debug.Log("Achievement " + _achievementToUnlock + " could NOT BE unlocked :(" );
        }
    }
    
    [Button()]
    private void ClearAllAchievements()
    {
        ServiceLocator.GetInstance().AchievementsManager.LockAllAchievements();
    }
    
    
}
