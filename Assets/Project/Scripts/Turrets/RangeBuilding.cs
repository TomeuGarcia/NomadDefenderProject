using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RangeBuilding : Building
{
    protected List<Enemy> enemies = new List<Enemy>();


    public delegate void RangeBuildingAction(Enemy enemy);
    public RangeBuildingAction OnEnemyEnterRange;
    public RangeBuildingAction OnEnemyExitRange;


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            OnEnemyEnters(other.gameObject.GetComponent<Enemy>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Enemy")
        {
            OnEnemyExits(other.gameObject.GetComponent<Enemy>());
        }
    }


    private void OnEnemyEnters(Enemy enemy)
    {
        if (!enemy.DiesFromQueuedDamage())
        {
            AddEnemy(enemy);
        }
    }
    private void OnEnemyExits(Enemy enemy)
    {
        if (enemies.Contains(enemy))
        {
            RemoveEnemy(enemy);
        }            
    }


    private void AddEnemy(Enemy enemy)
    {
        if (OnEnemyEnterRange != null) OnEnemyEnterRange(enemy);
        enemy.OnEnemyDeactivated += DeleteEnemyFromList;
        enemies.Add(enemy);
        enemies.Sort(mySort);
    }

    protected void RemoveEnemy(Enemy enemy)
    {
        enemy.OnEnemyDeactivated -= DeleteEnemyFromList;
        DeleteEnemyFromList(enemy);
    }

    private void DeleteEnemyFromList(Enemy enemyToDelete)
    {
        if (OnEnemyExitRange != null) OnEnemyExitRange(enemyToDelete);
        enemyToDelete.ChangeToBaseMat();
        enemies.Remove(enemyToDelete);

        enemies.Sort(mySort);
    }

    private int mySort(Enemy e1, Enemy e2)
    {
        return e1.pathFollower.DistanceLeftToEnd.CompareTo(e2.pathFollower.DistanceLeftToEnd);
    }



    /// <summary>
    /// Returns a List with size up to "amount", assumes there is at least 1 enemy in the turret's queue
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
    public Enemy[] GetNearestEnemies(int amount, float thresholdDistance)
    {
        List<Enemy> enemyList = new List<Enemy>();

        enemyList.Add(enemies[0]);

        int currentEnemyI = 1;
        while (currentEnemyI < enemies.Count && enemyList.Count < amount)
        {
            if (!enemyList.Contains(enemies[currentEnemyI]) && !enemies[currentEnemyI].DiesFromQueuedDamage())
            {
                float distance = Vector3.Distance(enemyList[enemyList.Count - 1].transformToMove.position, enemies[currentEnemyI].transformToMove.position);
                if (distance < thresholdDistance)
                {
                    enemyList.Add(enemies[currentEnemyI]);
                    currentEnemyI = 0;
                }
            }

            ++currentEnemyI;
        }

        return enemyList.ToArray();
    }


}
