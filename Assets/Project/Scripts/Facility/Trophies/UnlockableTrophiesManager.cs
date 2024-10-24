
using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "UnlockableTrophiesManager", 
    menuName = SOAssetPaths.UNLOCKABLES + "UnlockableTrophiesManager")]
public class UnlockableTrophiesManager : ScriptableObject
{
    [SerializeField] private UnlockableTrophyModel[] _unlockableTrophies = Array.Empty<UnlockableTrophyModel>();

    public const string UNLOCKABLE_TROPHIES_KEY = "UnlockableTrophies";
    private const char IDS_SEPARATOR = '_';

    private void OnValidate()
    {
        ValidateIds();
    }

    private void OnEnable()
    {
        CreateEntryIfNeeded();
        ValidateIds();;
    }

    private void CreateEntryIfNeeded()
    {
        if (PlayerPrefs.HasKey(UNLOCKABLE_TROPHIES_KEY))
        {
            SetAllTrophiesLocked();
        }
    }

    [Button("DEBUG_SetAllTrophies_LOCKED")]
    public void SetAllTrophiesLocked()
    {
        PlayerPrefs.SetString(UNLOCKABLE_TROPHIES_KEY, "");
    }
    
    [Button("DEBUG_SetAllTrophies_UNLOCKED")]
    public void SetAllTrophiesUnlocked()
    {
        PlayerPrefs.SetString(UNLOCKABLE_TROPHIES_KEY, GetAppendedTrophiesIds());
    }

    public void Unlock(UnlockableTrophyModel model)
    {
        if (!_unlockableTrophies.Contains(model))
        {
            Debug.LogError("Trying to unlock untracked TrophyModel: " + model.name);
        }

        string addedIds = PlayerPrefs.GetString(UNLOCKABLE_TROPHIES_KEY);
        addedIds += IDS_SEPARATOR + model.Id;
        PlayerPrefs.SetString(UNLOCKABLE_TROPHIES_KEY, addedIds);
    }
    
    
    private string GetAppendedTrophiesIds()
    {
        string appendedIds = "";
        foreach (UnlockableTrophyModel unlockableTrophy in _unlockableTrophies)
        {
            appendedIds += unlockableTrophy.Id + IDS_SEPARATOR;
        }

        return appendedIds;
    }

    [Button("DEBUG_ValidateIds")]
    private void ValidateIds()
    {
        Dictionary<string, UnlockableTrophyModel> idToModel = new (_unlockableTrophies.Length);

        foreach (UnlockableTrophyModel unlockableTrophy in _unlockableTrophies)
        {
            string id = unlockableTrophy.Id;
            if (idToModel.TryGetValue(id, out var alreadyAddedModel))
            {
                Debug.LogError("Repeated ID *"+id+"*, found at:     " +
                               alreadyAddedModel.name + "     &     " +
                               unlockableTrophy.name);
                break;
            }
            
            idToModel.Add(id, unlockableTrophy);
        }
    }
    
}