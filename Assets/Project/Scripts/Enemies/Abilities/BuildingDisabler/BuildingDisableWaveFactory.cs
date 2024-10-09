using System.Collections.Generic;
using UnityEngine;
using Scripts.ObjectPooling;


public class BuildingDisableWaveFactory : MonoBehaviour
{
    [System.Serializable]
    private class SpeedBoosterConfigToPool
    {
        [SerializeField] private BuildingDisableWaveConfig _config;
        [SerializeField] private ObjectPoolData<SpeedBooster> _objectPoolData;
        
        public BuildingDisableWaveConfig Config => _config;
        public ObjectPoolData<SpeedBooster> ObjectPoolData => _objectPoolData;
    }

    [SerializeField] private SpeedBoosterConfigToPool[] _configsToPoolData;
    private Dictionary<BuildingDisableWaveConfig, ObjectPool> _configsToPool;
    public static BuildingDisableWaveFactory Instance { get; private set; }
    
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
        _configsToPool = new Dictionary<BuildingDisableWaveConfig, ObjectPool>(_configsToPoolData.Length);

        foreach (SpeedBoosterConfigToPool configToPoolData in _configsToPoolData)
        {
            _configsToPool.Add(configToPoolData.Config, configToPoolData.ObjectPoolData.ToObjectPool(transform));
        }
    }

    public BuildingDisableWave Create(BuildingDisableWaveConfig config, Vector3 position, Quaternion rotation)
    {
        BuildingDisableWave buildingDisableWave = _configsToPool[config].Spawn<BuildingDisableWave>(position, rotation);
        buildingDisableWave.Init(config);
        return buildingDisableWave;
    }
}