using System;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class BuildingSellingConfig
{
    [SerializeField, Min(0)] private float _playValueMultiplier = 0.5f;
    [SerializeField, Min(0)] private float _upgradesValueMultiplier = 0.5f;
    
    public float PlayValueMultiplier => _playValueMultiplier;
    public float UpgradesValueMultiplier => _upgradesValueMultiplier;
    
    public int ExtraAddAmount = 0;
    

    public Action OnExtraAddAmountUpdated;
    

    public BuildingSellingConfig(BuildingSellingConfig other)
    {
        _playValueMultiplier = other._playValueMultiplier;
        _upgradesValueMultiplier = other._upgradesValueMultiplier;
        ExtraAddAmount = 0;
    }

    public void OverwriteExtraAddAmount(int newExtraAddAmount)
    {
        ExtraAddAmount = newExtraAddAmount;
        OnExtraAddAmountUpdated?.Invoke();
    }
}