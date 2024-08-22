using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileParticleFactory : MonoBehaviour
{
    public enum ProjectileParticleType { BASIC, TESLA, TESLA_WAVE, LONG_RANGE, CLOSE_RANGE, PIERCING }

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

    public GameObject GetAttackParticlesGameObject(ATurretProjectileBehaviour.Type attackType, Vector3 position, Quaternion rotation)
    {
        return GetAttackParticlesGameObject(AttackTypeToProjectileParticleType(attackType), position, rotation);
    }

    public GameObject GetAttackParticlesGameObject(ProjectileParticleType projectileParticleType, Vector3 position, Quaternion rotation)
    {
        return sortedAttacks[projectileParticleType].GetObject(position, rotation);
    }

    public void ResetPools()
    {
        foreach (AttackTypeParticleToPool attackTypeParticleToPool in particleToPool)
        {
            attackTypeParticleToPool.pool.ResetObjectsList();
        }
    }

    private ProjectileParticleType AttackTypeToProjectileParticleType(ATurretProjectileBehaviour.Type attackType)
    {
        ProjectileParticleType projectileParticleType = ProjectileParticleType.BASIC;

        if (attackType == ATurretProjectileBehaviour.Type.Homing)
        {
            projectileParticleType = ProjectileParticleType.BASIC;
        }
        else if (attackType == ATurretProjectileBehaviour.Type.HomingChaining)
        {
            projectileParticleType = ProjectileParticleType.TESLA;
        }
        /*
        else if (attackType == ATurretProjectileBehaviour.Type.LONG_RANGE)
        {
            projectileParticleType = ProjectileParticleType.LONG_RANGE;
        }
        else if (attackType == ATurretProjectileBehaviour.Type.CLOSE_RANGE)
        {
            projectileParticleType = ProjectileParticleType.CLOSE_RANGE;
        }
        */
        else if (attackType == ATurretProjectileBehaviour.Type.Piercing)
        {
            projectileParticleType = ProjectileParticleType.PIERCING;
        }

        return projectileParticleType;
    }

}
