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

    [Header("SLOW BASE")]
    [SerializeField] private List<float> slowSpeedCoefs = new List<float>();
    private float currentSlowSpeedCoef;
    private int currentLvl = 0;

    [SerializeField] private GameObject slowPlane;
    private static Dictionary<Enemy, SlowData> slowedEnemies = new Dictionary<Enemy, SlowData>();

    private Material slowPlaneMaterial;


    private void Awake()
    {
        AwakeInit();
    }

    override public void Init(TurretBuilding turretOwner, float turretRange) 
    {
        base.Init(turretOwner, turretRange);

        turretOwner.OnEnemyEnterRange += SlowEnemy;
        turretOwner.OnEnemyExitRange += StopEnemySlow;

        slowPlane.transform.localScale = Vector3.one * ((float)turretRange / 10.0f);

        currentSlowSpeedCoef = slowSpeedCoefs[currentLvl];
    }
    override public void InitAsSupportBuilding(SupportBuilding supportOwner, float supportRange)
    {
        base.InitAsSupportBuilding(supportOwner, supportRange);

        supportOwner.OnEnemyEnterRange += SlowEnemy;
        supportOwner.OnEnemyExitRange += StopEnemySlow;

        UpdateAreaPlaneSize(supportOwner);

        currentSlowSpeedCoef = slowSpeedCoefs[currentLvl];
    }
    private void UpdateAreaPlaneSize(SupportBuilding supportOwner)
    {
        float planeRange = supportOwner.stats.range * 2 + 1; //only for square
        float range = supportOwner.stats.range;

        slowPlane.transform.localScale = Vector3.one * ((float)planeRange / 10.0f);
        slowPlaneMaterial = slowPlane.GetComponent<MeshRenderer>().materials[0];
        slowPlaneMaterial.SetFloat("_TileNum", planeRange);
    }

    override public void Upgrade(SupportBuilding ownerSupportBuilding, int newStatLevel)
    {
        base.Upgrade(ownerSupportBuilding, newStatLevel);
        currentLvl = newStatLevel;

        currentSlowSpeedCoef = slowSpeedCoefs[currentLvl];

        foreach (KeyValuePair<Enemy, SlowData> slowedEnemy in slowedEnemies)
        {
            if(slowedEnemy.Value.slowCoefApplied > currentSlowSpeedCoef)
            {
                slowedEnemy.Key.SetMoveSpeed(currentSlowSpeedCoef);
                slowedEnemy.Value.slowCoefApplied = currentSlowSpeedCoef;
            }
        }

        // TODO see if we want to make Supports 3rd upgrade always increment range
        /*
        if (newStatLevel == 3)
        {
            ownerSupportBuilding.UpgradeRangeIncrementingLevel();
            UpdateAreaPlaneSize(ownerSupportBuilding);
        }
        */
    }

    private void SlowEnemy(Enemy enemy)
    {
        if(slowedEnemies.ContainsKey(enemy))
        {
            slowedEnemies[enemy].slowQuantity += 1;
        }
        else
        {
            enemy.SetMoveSpeed(currentSlowSpeedCoef);
            slowedEnemies[enemy] = new SlowData { slowQuantity = 1, slowCoefApplied = currentSlowSpeedCoef };
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
