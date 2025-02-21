using Scripts.ObjectPooling;
using System.Collections.Generic;
using UnityEngine;

public class BuildingDisableBombFactory : MonoBehaviour
{
    [System.Serializable]
    private class SpeedBoosterConfigToPool
    {
        [SerializeField] private BuildingDisableBombConfig _config;
        [SerializeField] private ObjectPoolData<BuildingDisableBomb> _objectPoolData;

        public BuildingDisableBombConfig Config => _config;
        public ObjectPoolData<BuildingDisableBomb> ObjectPoolData => _objectPoolData;
    }

    [SerializeField] private SpeedBoosterConfigToPool[] _configsToPoolData;
    private Dictionary<BuildingDisableBombConfig, ObjectPool> _configsToPool;
    public static BuildingDisableBombFactory Instance { get; private set; }


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
        _configsToPool = new Dictionary<BuildingDisableBombConfig, ObjectPool>(_configsToPoolData.Length);

        foreach (SpeedBoosterConfigToPool configToPoolData in _configsToPoolData)
        {
            _configsToPool.Add(configToPoolData.Config, configToPoolData.ObjectPoolData.ToObjectPool(transform));
        }
    }

    public BuildingDisableBomb Create(BuildingDisableBombConfig config, Vector3 position, Quaternion rotation)
    {
        BuildingDisableBomb buildingDisableWave = _configsToPool[config].Spawn<BuildingDisableBomb>(position, rotation);
        buildingDisableWave.Init(config);
        return buildingDisableWave;
    }
}
