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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SpawnEnemy();
        }
        
        if (Input.GetKeyDown(KeyCode.W))
        {
            ShootProjectile();
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


        Enemy enemy = Instantiate(enemyPrefab, startNode.Position, Quaternion.identity);
        enemy.pathFollower.Init(startNode.GetNextNode(), startNode.GetDirectionToNextNode(), totalDistance, enemy.transformToMove);

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
        projectile.Init(topEnemy, 1);
    }

}
