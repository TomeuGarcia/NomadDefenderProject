using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "AchievementsManagerConfig", 
    menuName = SOAssetPaths.ACHIEVEMENTS + "AchievementsManagerConfig")]
public class AchievementsManagerConfig : ScriptableObject
{
    [System.Serializable]
    private class AchievementAndId
    {
        [ReadOnly, AllowNesting] [SerializeField] private AchievementType _type;
        [SerializeField] private string _id;
        
        public AchievementType Type => _type;
        public string Id => _id;

        public AchievementAndId(AchievementType type)
        {
            _type = type;
            _id = "";
        }

        public void OverwriteType(AchievementType type)
        {
            _type = type;
        }
        public void ClearId()
        {
            _id = "";
        }
    }


    [SerializeField] private List<AchievementAndId> _achievements = new ();

    private void OnEnable()
    {
        ValidateIds();
    }

    private void OnValidate()
    {
        ValidateIds();
    }


    [Button()]
    private void ValidateIds()
    {
        CreateMissingAchievements();
        
        int i = 0;
        foreach (AchievementType achievementType in Enum.GetValues(typeof(AchievementType)))
        {
            AchievementAndId currentAchievement = _achievements[i];
            CheckRepeatedIds(currentAchievement, i);
            if (currentAchievement.Type != achievementType)
            {
                currentAchievement.OverwriteType(achievementType);
            }

            ++i;
        }
    }

    private void CreateMissingAchievements()
    {
        int totalCount = Enum.GetValues(typeof(AchievementType)).Length;
        for (int i = _achievements.Count; i < totalCount; ++i)
        {
            AddAchievement((AchievementType)i);
        }

        _achievements = _achievements.GetRange(0, totalCount);
    }

    private void AddAchievement(AchievementType achievementType)
    {
        _achievements.Add(new AchievementAndId(achievementType));
    }

    
    private void CheckRepeatedIds(AchievementAndId achievement, int achievementIndexToIgnore)
    {
        for (int i = 0; i < achievementIndexToIgnore; ++i)
        {
            CheckRepeatedIds(i, achievement.Id);
        }
        
        for (int i = achievementIndexToIgnore + 1; i < _achievements.Count; ++i)
        {
            CheckRepeatedIds(i, achievement.Id);
        }
    }
    private void CheckRepeatedIds(int indexToCompare, string id)
    {
        if (_achievements[indexToCompare].Id == id)
        {
            _achievements[indexToCompare].ClearId();
        }
    }
    

    public Dictionary<AchievementType, string> MakeAchievementsMap()
    {
        Dictionary<AchievementType, string> achievementsMap = new(_achievements.Count);
        foreach (AchievementAndId achievementAndId in _achievements)
        {
            achievementsMap.Add(achievementAndId.Type, achievementAndId.Id);
        }

        return achievementsMap;
    }
}
