using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowBase : TurretPartBase_Prefab
{
    [SerializeField] private float slowSpeedCoef;
    [SerializeField] private GameObject slowPlane;
    private static Dictionary<Enemy, int> slowedEnemies = new Dictionary<Enemy, int>();

    override public void Init(Turret turretOwner, int turretRange) 
    {
        turretOwner.OnEnemyEnterRange += SlowEnemy;
        turretOwner.OnEnemyExitRange += StopEnemySlow;

        slowPlane.transform.localScale = Vector3.one * ((float)turretRange / 10.0f);
    }

    private void SlowEnemy(Enemy enemy)
    {
        if(slowedEnemies.ContainsKey(enemy))
        {
            slowedEnemies[enemy] += 1;
        }
        else
        {
            enemy.SetMoveSpeed(slowSpeedCoef);
            slowedEnemies[enemy] = 1;
        }
    }

    private void StopEnemySlow(Enemy enemy)
    {
        slowedEnemies[enemy] -= 1;
        
        if(slowedEnemies[enemy] == 0)
        {
            //slowedEnemies.Remove(enemy);
            enemy.SetMoveSpeed(1.0f);
        }
    }
}
