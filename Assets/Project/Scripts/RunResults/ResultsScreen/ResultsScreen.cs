using System;
using System.Collections;
using System.Collections.Generic;
using AYellowpaper;
using UnityEngine;

public class ResultsScreen : MonoBehaviour
{
    [SerializeField] private InterfaceReference<IRunStateData, ScriptableObject> _runStateData;
    private IRunStateData RunStateData => _runStateData.Value;


    private void Awake()
    {
        Init();
    }


    private void Init()
    {
        CheckAchievements();
    }


    private void CheckAchievements()
    {
        AchievementDefinitions.VictoryWithoutTakingDamage.Check(RunStateData.TotalDamageTaken);
        AchievementDefinitions.VictoryWithoutUpgradingBuildings.Check(RunStateData.TotalBuildingsUpgraded);
    }
}
