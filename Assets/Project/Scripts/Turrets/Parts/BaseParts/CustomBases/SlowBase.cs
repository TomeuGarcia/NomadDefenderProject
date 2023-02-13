using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowBase : TurretPartBase_Prefab
{
    public class SlowData
    {
        public int slowQuantity = 0;
        public float slowCoefApplied = 0;
    }

    [SerializeField] private List<float> slowSpeedCoefs = new List<float>();
    private int currentLvl = 0;

    [SerializeField] private GameObject slowPlane;
    private static Dictionary<Enemy, SlowData> slowedEnemies = new Dictionary<Enemy, SlowData>();

    private Material slowPlaneMaterial;

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

        float planeRange = turretOwner.stats.range * 2 + 1; //only for square
        float range = turretOwner.stats.range;

        slowPlane.transform.localScale = Vector3.one * ((float)planeRange / 10.0f);
        slowPlaneMaterial = slowPlane.GetComponent<MeshRenderer>().materials[0];
        slowPlaneMaterial.SetFloat("_TileNum", planeRange);
    }

    override public void Upgrade(int newStatnewStatLevel)
    {
        base.Upgrade(newStatnewStatLevel);
        currentLvl = newStatnewStatLevel;

        foreach(KeyValuePair<Enemy, SlowData> slowedEnemy in slowedEnemies)
        {
            if(slowedEnemy.Value.slowCoefApplied > slowSpeedCoefs[currentLvl])
            {
                slowedEnemy.Key.SetMoveSpeed(slowSpeedCoefs[currentLvl]);
                slowedEnemy.Value.slowCoefApplied = slowSpeedCoefs[currentLvl];
            }
        }
    }

    private void SlowEnemy(Enemy enemy)
    {
        if(slowedEnemies.ContainsKey(enemy))
        {
            slowedEnemies[enemy].slowQuantity += 1;
        }
        else
        {
            enemy.SetMoveSpeed(slowSpeedCoefs[currentLvl]);
            slowedEnemies[enemy] = new SlowData { slowQuantity = 1, slowCoefApplied = slowSpeedCoefs[currentLvl] };
        }
    }

    private void StopEnemySlow(Enemy enemy)
    {
        if (!slowedEnemies.ContainsKey(enemy)) return;

        slowedEnemies[enemy].slowQuantity -= 1;
        
        if(slowedEnemies[enemy].slowQuantity == 0)
        {
            slowedEnemies.Remove(enemy);

            enemy.SetMoveSpeed(1.0f);
        }
    }
}
