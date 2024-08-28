using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.ObjectPooling;


public class ProjectileParticleFactory : MonoBehaviour
{
    [System.Serializable]
    private struct AttackTypeParticleToPool
    {
        public ProjectileParticleType type;
        public Pool pool;
    }



    private static ProjectileParticleFactory instance;

    
    [Header("PARTICLES")]
    [SerializeField] private AttackTypeParticleToPool[] particleToPool;
    private Dictionary<ProjectileParticleType, Pool> sortedAttacks;


    [Header("VIEW ADD-ONS")]
    [Header("Projectiles")]
    [SerializeField] private ProjectileViewAddOnConfig[] _projectileAddOnConfigs;
    private Dictionary<ProjectileViewAddOnConfig, ObjectPool> _projectileAddOnConfigToPool;

    [Header("Turrets")]
    [SerializeField] private TurretViewAddOnConfig[] _turretAddOnConfigs;
    private Dictionary<TurretViewAddOnConfig, ObjectPool> _turretAddOnConfigToPool;
    
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
            Init();
        }
        else
        {
            Destroy(this);
        }
    }

    private void OnEnable()
    {
        TDGameManager.OnEndGameResetPools += ResetPools;
    }
    private void OnDisable()
    {
        TDGameManager.OnEndGameResetPools -= ResetPools;
    }

    
    public static ProjectileParticleFactory GetInstance()
    {
        return instance;
    }

    
    private void Init()
    {
        sortedAttacks = new Dictionary<ProjectileParticleType, Pool>();
        foreach (AttackTypeParticleToPool attackTypeToPool in particleToPool)
        {
            sortedAttacks[attackTypeToPool.type] = attackTypeToPool.pool;
        }
        
        _projectileAddOnConfigToPool = new Dictionary<ProjectileViewAddOnConfig, ObjectPool>(_projectileAddOnConfigs.Length);
        foreach (ProjectileViewAddOnConfig addOnConfig in _projectileAddOnConfigs)
        {
            _projectileAddOnConfigToPool.Add(addOnConfig, addOnConfig.ObjectPoolData.ToObjectPool(transform));
        }
        
        _turretAddOnConfigToPool = new Dictionary<TurretViewAddOnConfig, ObjectPool>(_turretAddOnConfigs.Length);
        foreach (TurretViewAddOnConfig addOnConfig in _turretAddOnConfigs)
        {
            _turretAddOnConfigToPool.Add(addOnConfig, addOnConfig.ObjectPoolData.ToObjectPool(transform));
        }
    }
    
    public GameObject CreateParticlesGameObject(ProjectileParticleType projectileParticleType, Vector3 position, Quaternion rotation)
    {
        GameObject hitParticles = sortedAttacks[projectileParticleType].GetObject(position, rotation);
        hitParticles.SetActive(true);
        return hitParticles;
    }

    public void ResetPools()
    {
        foreach (AttackTypeParticleToPool attackTypeParticleToPool in particleToPool)
        {
            attackTypeParticleToPool.pool.ResetObjectsList();
        }
    }
  
    public AProjectileViewAddOn CreateProjectileAddOn(ProjectileViewAddOnConfig addOnConfig)
    {
        return _projectileAddOnConfigToPool[addOnConfig].Spawn<AProjectileViewAddOn>(Vector3.zero, Quaternion.identity);
    }
    
    public ATurretViewAddOn CreateTurretAddOn(TurretViewAddOnConfig addOnConfig)
    {
        return _turretAddOnConfigToPool[addOnConfig].Spawn<ATurretViewAddOn>(Vector3.zero, Quaternion.identity);
    }

}
