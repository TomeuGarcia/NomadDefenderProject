using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileParticleFactory : MonoBehaviour
{
    [System.Serializable]
    private struct AttackTypeParticleToPool
    {
        public TurretPartAttack_Prefab.AttackType type;
        public Pool pool;
    }



    private static ProjectileParticleFactory instance;

    [SerializeField] private AttackTypeParticleToPool[] particleToPool;
    private Dictionary<TurretPartAttack_Prefab.AttackType, Pool> sortedAttacks;



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
        sortedAttacks = new Dictionary<TurretPartAttack_Prefab.AttackType, Pool>();
        foreach (AttackTypeParticleToPool attackTypeToPool in particleToPool)
        {
            sortedAttacks[attackTypeToPool.type] = attackTypeToPool.pool;
        }
    }

    public GameObject GetAttackParticlesGameObject(TurretPartAttack_Prefab.AttackType attackType, Vector3 position, Quaternion rotation)
    {
        return sortedAttacks[attackType].GetObject(position, rotation);
    }

    private void ResetPools()
    {
        foreach (AttackTypeParticleToPool attackTypeParticleToPool in particleToPool)
        {
            attackTypeParticleToPool.pool.ResetObjectsList();
        }
    }
}
