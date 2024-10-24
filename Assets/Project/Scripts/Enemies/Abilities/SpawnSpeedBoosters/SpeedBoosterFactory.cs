using System.Collections.Generic;
using Scripts.ObjectPooling;
using UnityEngine;

public class SpeedBoosterFactory : MonoBehaviour
{
    [System.Serializable]
    private class SpeedBoosterConfigToPool
    {
        [SerializeField] private SpeedBoosterConfig _config;
        [SerializeField] private ObjectPoolData<SpeedBooster> _objectPoolData;
        
        public SpeedBoosterConfig Config => _config;
        public ObjectPoolData<SpeedBooster> ObjectPoolData => _objectPoolData;
    }

    [SerializeField] private SpeedBoosterConfigToPool[] _configsToPoolData;
    private Dictionary<SpeedBoosterConfig, ObjectPool> _configsToPool;
    public static SpeedBoosterFactory Instance { get; private set; }
    
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
        _configsToPool = new Dictionary<SpeedBoosterConfig, ObjectPool>(_configsToPoolData.Length);

        foreach (SpeedBoosterConfigToPool configToPoolData in _configsToPoolData)
        {
            _configsToPool.Add(configToPoolData.Config, configToPoolData.ObjectPoolData.ToObjectPool(transform));
        }
    }

    public SpeedBooster Create(SpeedBoosterConfig config, Vector3 position, Quaternion rotation)
    {
        SpeedBooster speedBooster = _configsToPool[config].Spawn<SpeedBooster>(position, rotation);
        speedBooster.Init(config);
        return speedBooster;
    }
    
}