using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "RunState", 
    menuName = SOAssetPaths.GAME_RUN + "RunState")]
public class RunState : ScriptableObject, 
    IRunStateInitialization, IRunStateData, IRunStateUpdate
{
    public int TotalDamageTaken { get; private set; }
    public int TotalBuildingsUpgraded { get; private set; }

    
    public void Init()
    {
        TotalDamageTaken = 0;
        TotalBuildingsUpgraded = 0;
    }


    public void AddDamageTaken(int damageTaken)
    {
        TotalDamageTaken += damageTaken;
    }

    public void IncrementUpgradedBuildings()
    {
        ++TotalBuildingsUpgraded;
    }


}
