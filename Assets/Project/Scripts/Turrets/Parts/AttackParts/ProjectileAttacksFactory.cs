using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ProjectileParticleFactory;

public class ProjectileAttacksFactory : MonoBehaviour
{
    [System.Serializable]
    private struct ProjectileTypeAndPool
    {
        public TurretPartAttack_Prefab.AttackType attackType;
        public Pool pool;
    }


    private static ProjectileAttacksFactory instance;

    [SerializeField] private ProjectileTypeAndPool[] projectileAndPoolList;
    private Dictionary<TurretPartAttack_Prefab.AttackType, Pool> projectileTypeToPool;



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

    private void Init()
    {
        projectileTypeToPool = new Dictionary<TurretPartAttack_Prefab.AttackType, Pool>();

        foreach (ProjectileTypeAndPool projectileTypeAndPool in projectileAndPoolList)
        {
            projectileTypeToPool.Add(projectileTypeAndPool.attackType, projectileTypeAndPool.pool);
        }
    }

    public static ProjectileAttacksFactory GetInstance()
    {
        return instance;
    }

    private void ResetPools()
    {
        foreach (ProjectileTypeAndPool attackTypeParticleToPool in projectileAndPoolList)
        {
            attackTypeParticleToPool.pool.ResetObjectsList();
        }
    }

    public GameObject GetAttackGameObject(TurretPartAttack_Prefab.AttackType attackType, Vector3 position, Quaternion rotation)
    {
        return projectileTypeToPool[attackType].GetObject(position, rotation);
    }

}
