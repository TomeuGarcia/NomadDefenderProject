using System;
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
    private static Dictionary<Enemy, SlowData> s_slowedEnemies = new Dictionary<Enemy, SlowData>();
    private List<Enemy> _slowedEnemiesByThis = new List<Enemy>();
    [SerializeField] private ParticleTypes _slowBreakParticleType;

    private float _particleLookAtHeightOffset = 0f;

    private Material slowPlaneMaterial;

    private IParticleFactory _particleFactory;
    private RangeBuilding _ownerBuilding;


    private void Awake()
    {
        AwakeInit();

        _particleFactory = ServiceLocator.GetInstance().ParticleFactory;
    }

    private void OnDestroy()
    {
        _ownerBuilding.OnEnemyEnterRange -= SlowEnemy;
        _ownerBuilding.OnEnemyExitRange -= StopEnemySlow;
    }

    override public void Init(TurretBuilding turretOwner, float turretRange) 
    {
        base.Init(turretOwner, turretRange);
        _ownerBuilding = turretOwner;

        _ownerBuilding.OnEnemyEnterRange += SlowEnemy;
        _ownerBuilding.OnEnemyExitRange += StopEnemySlow;

        slowPlane.transform.localScale = Vector3.one * ((float)turretRange / 10.0f);

        currentSlowSpeedCoef = slowSpeedCoefs[currentLvl];
    }
    override public void InitAsSupportBuilding(SupportBuilding supportOwner, float supportRange)
    {
        base.InitAsSupportBuilding(supportOwner, supportRange);
        _ownerBuilding = supportOwner;

        _ownerBuilding.OnEnemyEnterRange += SlowEnemy;
        _ownerBuilding.OnEnemyExitRange += StopEnemySlow;

        UpdateAreaPlaneSize(supportOwner);

        currentSlowSpeedCoef = slowSpeedCoefs[currentLvl];
    }
    private void UpdateAreaPlaneSize(SupportBuilding supportOwner)
    {
        float planeRange = supportOwner.Stats.RadiusRange * 2 + 1; //only for square
        float range = supportOwner.Stats.RadiusRange;

        slowPlane.transform.localScale = Vector3.one * ((float)planeRange / 10.0f);
        slowPlaneMaterial = slowPlane.GetComponent<MeshRenderer>().materials[0];
        slowPlaneMaterial.SetFloat("_TileNum", planeRange);
    }

    override public void Upgrade(SupportBuilding ownerSupportBuilding, int newStatLevel)
    {
        base.Upgrade(ownerSupportBuilding, newStatLevel);
        currentLvl = newStatLevel;

        currentSlowSpeedCoef = slowSpeedCoefs[currentLvl];

        foreach (KeyValuePair<Enemy, SlowData> slowedEnemy in s_slowedEnemies)
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
        if(s_slowedEnemies.ContainsKey(enemy))
        {
            s_slowedEnemies[enemy].slowQuantity += 1;
        }
        else
        {
            enemy.SetMoveSpeed(currentSlowSpeedCoef);
            s_slowedEnemies[enemy] = new SlowData { slowQuantity = 1, slowCoefApplied = currentSlowSpeedCoef };
            SpawnSlowBreakParticles(enemy);
        }
        
        _slowedEnemiesByThis.Add(enemy);
    }

    private void StopEnemySlow(Enemy enemy)
    {
        if (!s_slowedEnemies.ContainsKey(enemy)) return;

        s_slowedEnemies[enemy].slowQuantity -= 1;
        
        if(s_slowedEnemies[enemy].slowQuantity == 0)
        {
            s_slowedEnemies.Remove(enemy);
            SpawnSlowBreakParticles(enemy);

            enemy.SetMoveSpeed(1.0f);
        }
        
        _slowedEnemiesByThis.Remove(enemy);
    }

    private void SpawnSlowBreakParticles(Enemy enemy)
    {
        Transform particle = _particleFactory
            .Create(_slowBreakParticleType, transform.position + Vector3.up * _particleLookAtHeightOffset, Quaternion.identity);
        particle.LookAt(enemy.Position);
        particle.position = enemy.Position;
    }

    public override void DoOnBuildingDisableStart()
    {
        base.DoOnBuildingDisableStart();
        baseCollider.GetCollider().enabled = false;

        StopSlowingEnemiesByThis();
    }

    public override void DoOnBuildingDisableFinish()
    {
        base.DoOnBuildingDisableFinish();
        baseCollider.GetCollider().enabled = true;
    }
    
    
    public override void OnGetUnplaced()
    {
        StopSlowingEnemiesByThis();
    }

    private void StopSlowingEnemiesByThis()
    {
        Enemy[] tempSlowedEnemiesByThis = _slowedEnemiesByThis.ToArray();
        foreach (Enemy enemy in tempSlowedEnemiesByThis)
        {
            StopEnemySlow(enemy);
        }
    }
}
