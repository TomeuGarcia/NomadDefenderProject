using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFactory : MonoBehaviour, IActiveEnemiesTracker
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

    [SerializeField] private Enemy _nullEnemy;

    private List<Enemy> _activeEnemies;


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
        TDGameManager.OnSceneFinish += ResetPools;
    }
    private void OnDisable()
    {
        TDGameManager.OnSceneFinish -= ResetPools;
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
        
        _activeEnemies = new List<Enemy>(100);
    }

    public Enemy CreateEnemy(EnemyTypeConfig enemyType, Vector3 position, Quaternion rotation, Transform spawnTransform)
    {
        Enemy enemy = sortedEnemies[enemyType].GetObject(position, rotation, spawnTransform).GetComponent<Enemy>();
        enemy.gameObject.SetActive(true);
        
        enemy.SetActiveEnemiesTracker(this);
        
        return enemy;
    }

    public Enemy GetNullEnemy()
    {
        return _nullEnemy;
    }
    
    public void ResetPools()
    {
        foreach (EnemyTypeToPool enemyTypeToPool in enemiesToPool)
        {
            enemyTypeToPool.pool.ResetObjectsList();
        }
        
        _activeEnemies.Clear();
    }

    public void AddActiveEnemy(Enemy enemy)
    {
        _activeEnemies.Add(enemy);
    }

    public void RemoveActiveEnemy(Enemy enemy)
    {
        _activeEnemies.Remove(enemy);
    }

    public IReadOnlyCollection<Enemy> GetActiveEnemies()
    {
        return _activeEnemies;
    }
}
