using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileParticleFactory : MonoBehaviour
{
    

    [System.Serializable]
    private struct AttackTypeParticleToPool
    {
        public ProjectileParticleType type;
        public Pool pool;
    }



    private static ProjectileParticleFactory instance;

    [SerializeField] private AttackTypeParticleToPool[] particleToPool;
    private Dictionary<ProjectileParticleType, Pool> sortedAttacks;



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

  

}
