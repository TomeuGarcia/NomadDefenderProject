using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathStart : MonoBehaviour
{
    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private HomingProjectile projectilePrefab;
    [SerializeField] private PathNode startNode;

    private List<Enemy> enemies = new List<Enemy>();

    //ENEMY POOL
    private Pool enemyPool;

    private void Awake()
    {
        enemyPool = GameObject.Find("BasicEnemyPool").GetComponent<Pool>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SpawnEnemy();
        }
        
        if (Input.GetKeyDown(KeyCode.W))
        {
            //ShootProjectile();
        }
    }

    private void SpawnEnemy()
    {
        float totalDistance = 0f;
        PathNode itNode = startNode;
        while (!itNode.IsLastNode)
        {
            totalDistance += itNode.GetDistanceToNextNode();
            itNode = itNode.GetNextNode();
        }


        Enemy enemy = enemyPool.GetObject(startNode.Position, Quaternion.identity).gameObject.GetComponent<Enemy>();
        enemy.gameObject.SetActive(true);
        enemy.pathFollower.Init(startNode.GetNextNode(), startNode.GetDirectionToNextNode(), Vector3.zero, totalDistance, enemy.transformToMove);

        enemies.Add(enemy);
    }

    private void ShootProjectile()
    {
        if (enemies.Count == 0) return;

        Enemy topEnemy = enemies.First();
        while (topEnemy == null && enemies.Count > 0)
        {
            enemies.RemoveAt(0);
            if (enemies.Count == 0) return; 
            topEnemy = enemies.First();
        }

        HomingProjectile projectile = Instantiate(projectilePrefab, startNode.Position, Quaternion.identity);
        projectile.Init(topEnemy, null);
    }

}
