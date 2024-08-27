using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ProjectileParticleFactory;

public class ProjectileAttacksFactory : MonoBehaviour
{
    [System.Serializable]
    private struct ProjectileTypeAndPool : Pool.IListener
    {
        public TurretPartProjectileDataModel projectileDataModel;
        public Pool pool;

        public void Init()
        {
            pool.SetPooledObject(projectileDataModel.ProjectilePrefab.gameObject);
            pool._listener = this;
        }

        public void OnObjectInstantiated(GameObject projectileGameObject)
        {
            projectileGameObject.GetComponent<ATurretProjectileBehaviour>().InstantiatedInit(projectileDataModel);
        }
    }


    private static ProjectileAttacksFactory instance;

    [SerializeField] private ProjectileTypeAndPool[] projectileAndPoolList;
    private Dictionary<ATurretProjectileBehaviour.Type, Pool> projectileTypeToPool;



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
        projectileTypeToPool = new Dictionary<ATurretProjectileBehaviour.Type, Pool>();

        foreach (ProjectileTypeAndPool projectileTypeAndPool in projectileAndPoolList)
        {
            projectileTypeAndPool.Init();
            projectileTypeToPool.Add(projectileTypeAndPool.projectileDataModel.ProjectileType, projectileTypeAndPool.pool);
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

    public GameObject GetAttackGameObject(ATurretProjectileBehaviour.Type attackType, Vector3 position, Quaternion rotation)
    {
        return projectileTypeToPool[attackType].GetObject(position, rotation);
    }

}
