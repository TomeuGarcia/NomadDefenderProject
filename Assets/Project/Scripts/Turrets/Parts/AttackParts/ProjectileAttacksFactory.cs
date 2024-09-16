using System.Collections;
using System.Collections.Generic;
using Scripts.ObjectPooling;
using UnityEngine;


public class ProjectileAttacksFactory : MonoBehaviour
{
    [System.Serializable]
    private struct ProjectileTypeAndPool : ObjectPool.IListener
    {
        [SerializeField] private int _initialInstances;
        public TurretPartProjectileDataModel projectileDataModel;
        
        public ObjectPool MakeObjectPool(Transform originalParent)
        {
            ObjectPool objectPool = new ObjectPool(projectileDataModel.ProjectilePrefab, originalParent, this);
            objectPool.Init(_initialInstances);
            return objectPool;
        }

        public void OnObjectInstantiated(RecyclableObject spawnedObject)
        {
            spawnedObject.GetComponent<ATurretProjectileBehaviour>().InstantiatedInit(projectileDataModel);
        }
    }


    private static ProjectileAttacksFactory instance;

    [SerializeField] private ProjectileTypeAndPool[] projectileAndPoolList;
    private Dictionary<ATurretProjectileBehaviour.Type, Pool> projectileTypeToPool;
    
    private Dictionary<ATurretProjectileBehaviour.Type, ObjectPool> _projectileTypeToPool;

    


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
        PauseMenu.OnGameSurrender += ResetPools;
        TDGameManager.OnEndGameResetPools += ResetPools;
    }
    private void OnDisable()
    {
        PauseMenu.OnGameSurrender -= ResetPools;
        TDGameManager.OnEndGameResetPools -= ResetPools;
    }

    private void Init()
    {
        _projectileTypeToPool = new Dictionary<ATurretProjectileBehaviour.Type, ObjectPool>();

        foreach (ProjectileTypeAndPool projectileTypeAndPool in projectileAndPoolList)
        {
            _projectileTypeToPool.Add(
                projectileTypeAndPool.projectileDataModel.ProjectileType, 
                projectileTypeAndPool.MakeObjectPool(transform));
        }
    }

    public static ProjectileAttacksFactory GetInstance()
    {
        return instance;
    }

    private void ResetPools()
    {
        foreach (var typeToPool in _projectileTypeToPool)
        {
            typeToPool.Value.RecycleAll();
        }
    }

    public ATurretProjectileBehaviour Create(ATurretProjectileBehaviour.Type attackType, 
        Vector3 position, Quaternion rotation)
    {
        return _projectileTypeToPool[attackType].Spawn<ATurretProjectileBehaviour>(position, rotation);
    }

}
