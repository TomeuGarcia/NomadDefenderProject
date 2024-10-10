using System;
using System.Collections.Generic;
using UnityEngine;

public class BuildingDisableManager : MonoBehaviour
{
    private class BuildingDisableState
    {
        public IDisableableBuilding DisableableBuilding { get; }
        public Timer DisableableTimer { get; }

        public BuildingDisableState(IDisableableBuilding disableableBuilding, float duration)
        {
            DisableableBuilding = disableableBuilding;
            DisableableTimer = new Timer(duration);
        }
    }
    
    public static BuildingDisableManager Instance { get; private set; }
    private Dictionary<IDisableableBuilding, BuildingDisableState> _currentDisabledBuildings;
    private List<IDisableableBuilding> _disableableBuildingsToRemove;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
            Init();
        }
        else
        {
            Destroy(this);
        }
    }

    private void Init()
    {
        _currentDisabledBuildings = new Dictionary<IDisableableBuilding, BuildingDisableState>(5);
        _disableableBuildingsToRemove = new List<IDisableableBuilding>(5);
    }

    public void HandleNewBuilding(IDisableableBuilding disableableBuilding, float disableDuration)
    {
        if (_currentDisabledBuildings.TryGetValue(disableableBuilding, out BuildingDisableState buildingDisableState))
        {
            buildingDisableState.DisableableTimer.Reset();
            buildingDisableState.DisableableTimer.Duration = disableDuration;
            disableableBuilding.RestartDisabled();
        }
        else
        {
            _currentDisabledBuildings.Add(disableableBuilding, new BuildingDisableState(disableableBuilding, disableDuration));
            disableableBuilding.StartDisabled();
        }
    }

    private void Update()
    {
        if (_currentDisabledBuildings.Count < 1)
        {
            return;
        }
        
        foreach (var currentDisabledBuildingPair in _currentDisabledBuildings)
        {
            Timer disableTimer = currentDisabledBuildingPair.Value.DisableableTimer;
            disableTimer.Update(GameTime.DeltaTime);

            IDisableableBuilding disableableBuilding = currentDisabledBuildingPair.Key;
            disableableBuilding.UpdateDisabled(disableTimer.Ratio01);
            
            if (disableTimer.HasFinished())
            {
                _disableableBuildingsToRemove.Add(disableableBuilding);
            }
        }

        foreach (IDisableableBuilding disableableBuilding in _disableableBuildingsToRemove)
        {
            _currentDisabledBuildings.Remove(disableableBuilding);
            disableableBuilding.FinishDisabled();
        }
        _disableableBuildingsToRemove.Clear();
    }
    
    
}