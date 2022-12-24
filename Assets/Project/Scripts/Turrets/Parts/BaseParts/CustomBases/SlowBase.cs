using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowBase : TurretPartBase_Prefab
{
    [SerializeField] private float slowSpeedCoef;
    [SerializeField] private GameObject slowPlane;
    private static Dictionary<Enemy, int> slowedEnemies = new Dictionary<Enemy, int>();

    override public void Init(TurretBuilding turretOwner, float turretRange) 
    {
        base.Init(turretOwner, turretRange);

        turretOwner.OnEnemyEnterRange += SlowEnemy;
        turretOwner.OnEnemyExitRange += StopEnemySlow;

        slowPlane.transform.localScale = Vector3.one * ((float)turretRange / 10.0f);
    }
    override public void InitAsSupportBuilding(SupportBuilding turretOwner, float supportRange)
    {
        base.InitAsSupportBuilding(turretOwner, supportRange);

        turretOwner.OnEnemyEnterRange += SlowEnemy;
        turretOwner.OnEnemyExitRange += StopEnemySlow;

        slowPlane.transform.localScale = Vector3.one * ((float)supportRange / 10.0f);
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
        if (!slowedEnemies.ContainsKey(enemy)) return;

        slowedEnemies[enemy] -= 1;
        
        if(slowedEnemies[enemy] == 0)
        {
            slowedEnemies.Remove(enemy);

            enemy.SetMoveSpeed(1.0f);
        }
    }
}
