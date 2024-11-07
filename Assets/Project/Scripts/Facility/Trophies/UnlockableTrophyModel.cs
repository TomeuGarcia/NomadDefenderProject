using UnityEngine;


[CreateAssetMenu(fileName = "UnlockableTrophyModel_NAME", 
    menuName = SOAssetPaths.UNLOCKABLES + "UnlockableTrophyModel")]
public class UnlockableTrophyModel : ScriptableObject
{
    [SerializeField] private string _id;
    public string Id => _id;

    public bool IsUnlocked()
    {
         string allIds = PlayerPrefs.GetString(UnlockableTrophiesManager.UNLOCKABLE_TROPHIES_KEY);
         return allIds.Contains(_id);
    }

}