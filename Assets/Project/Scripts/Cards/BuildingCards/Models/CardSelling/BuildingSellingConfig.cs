using UnityEngine;

[System.Serializable]
public class BuildingSellingConfig
{
    [SerializeField, Min(0)] private float _playValueMultiplier = 0.5f;
    [SerializeField, Min(0)] private float _upgradesValueMultiplier = 0.5f;
    
    public float PlayValueMultiplier => _playValueMultiplier;
    public float UpgradesValueMultiplier => _upgradesValueMultiplier;

    public BuildingSellingConfig(BuildingSellingConfig other)
    {
        _playValueMultiplier = other._playValueMultiplier;
        _upgradesValueMultiplier = other._upgradesValueMultiplier;
    }
}