

using System.Collections.Generic;
using UnityEngine;

public class FurthestEnemyProjectileTargetingController : IProjectileTargetingController
{
    private readonly RangeBuilding _ownerBuilding;
    public Enemy TargetedEnemy { get; private set; }
    
    
    public FurthestEnemyProjectileTargetingController(RangeBuilding ownerBuilding)
    {
        _ownerBuilding = ownerBuilding;
        TargetedEnemy = null;
    }
    

    public bool TargetEnemyExists()
    {
        return TargetedEnemy != null;
    }

    public void ComputeNextTargetedEnemy()
    {
        List<Enemy> enemies = _ownerBuilding.Enemies;
        if (enemies.Count < 1)
        {
            TargetedEnemy = null;
            return;
        }

        TargetedEnemy = GetFurthestEnemy(enemies);  
    }
    
    private Enemy GetFurthestEnemy(List<Enemy> enemies)
    {
        Vector3 turretPosition = _ownerBuilding.Position;
        Enemy furthestEnemy = enemies[0];
        float furthestEnemyDistance = Vector3.Distance(furthestEnemy.Position, turretPosition);

        for (int i = 1; i < enemies.Count; ++i)
        {
            Enemy enemy = enemies[i];
            float distanceToEnemy = Vector3.Distance(enemy.Position, turretPosition);
            if (distanceToEnemy > furthestEnemyDistance)
            {
                furthestEnemy = enemy;
                furthestEnemyDistance = distanceToEnemy;
            }
        }

        return furthestEnemy;
    }

}