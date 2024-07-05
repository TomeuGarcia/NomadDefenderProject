using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFactory : MonoBehaviour
{
    [System.Serializable]
    private struct EnemyTypeToPool
    {
        public EnemyTypeConfig type;
        public Pool pool;
    }



    private static EnemyFactory instance;

    [SerializeField] private EnemyTypeToPool[] enemiesToPool;
    private Dictionary<EnemyTypeConfig, Pool> sortedEnemies;



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


    public static EnemyFactory GetInstance()
    {
        return instance;
    }



    private void Init()
    {
        sortedEnemies = new Dictionary<EnemyTypeConfig, Pool>();
        foreach (EnemyTypeToPool enemyTypeToPool in enemiesToPool)
        {
            sortedEnemies[enemyTypeToPool.type] = enemyTypeToPool.pool;
        }
    }

    public GameObject GetEnemyGameObject(EnemyTypeConfig enemyType, Vector3 position, Quaternion rotation, Transform spawnTransform)
    {
        return sortedEnemies[enemyType].GetObject(position, rotation, spawnTransform);
    }

    public void ResetPools()
    {
        foreach (EnemyTypeToPool enemyTypeToPool in enemiesToPool)
        {
            enemyTypeToPool.pool.ResetObjectsList();
        }
    }


}
