using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TDGameManagerConfig", 
    menuName = SOAssetPaths.TD_GAME + "TDGameManagerConfig")]
public class TDGameManagerConfig : ScriptableObject
{
    [Header("LOCATIONS MAX HEALTH")]
    [SerializeField, Min(0)] private int _oneNodeLocationsMaxHealth = 5;
    [SerializeField, Min(0)] private int _twoNodesLocationsMaxHealth = 3;


    public int GetLocationsHealth(int numberOfLocations)
    {
        return numberOfLocations > 1
            ? _twoNodesLocationsMaxHealth
            : _oneNodeLocationsMaxHealth;
    }
}
