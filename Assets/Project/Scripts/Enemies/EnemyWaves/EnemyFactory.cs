using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFactory : MonoBehaviour
{
    [System.Serializable]
    private struct EnemyTypeToPool
    {
        public Enemy.EnemyType type;
        public Pool pool;
    }



    private static EnemyFactory instance;

    [SerializeField] private EnemyTypeToPool[] enemiesToPool;
    private Dictionary<Enemy.EnemyType, Pool> sortedEnemies;



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
        sortedEnemies = new Dictionary<Enemy.EnemyType, Pool>();
        foreach (EnemyTypeToPool enemyTypeToPool in enemiesToPool)
        {
            sortedEnemies[enemyTypeToPool.type] = enemyTypeToPool.pool;
        }
    }

    public GameObject GetEnemyGameObject(Enemy.EnemyType enemyType, Vector3 position, Quaternion rotation)
    {
        return sortedEnemies[enemyType].GetObject(position, rotation);
    }

    private void ResetPools()
    {
        foreach (EnemyTypeToPool enemyTypeToPool in enemiesToPool)
        {
            enemyTypeToPool.pool.ResetObjectsList();
        }
    }


}
