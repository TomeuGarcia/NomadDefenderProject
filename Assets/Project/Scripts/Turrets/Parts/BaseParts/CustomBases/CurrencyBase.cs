using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyBase : TurretPartBase_Prefab
{

    [SerializeField] private int quantityToIncreaseCurrencyDrop;
    [SerializeField] private Transform topCube;
    private float rot = 0;
    public override void Init(TurretBuilding turretOwner, float turretRange)
    {
        base.Init(turretOwner, turretRange);
        //owner = turretOwner;
        turretOwner.OnEnemyEnterRange += increaseCurrencyDrop;
        turretOwner.OnEnemyExitRange += decreaseCurrencyDrop;
    }

    public override void InitAsSupportBuilding(SupportBuilding supportBuilding, float supportRange)
    {
        base.InitAsSupportBuilding(supportBuilding, supportRange);
        supportBuilding.OnEnemyEnterRange += increaseCurrencyDrop;   
        supportBuilding.OnEnemyExitRange += decreaseCurrencyDrop;
    }

    private void increaseCurrencyDrop(Enemy enemy)
    {
        enemy.currencyDrop += quantityToIncreaseCurrencyDrop;
    }

    private void decreaseCurrencyDrop(Enemy enemy)
    {
        enemy.currencyDrop -= quantityToIncreaseCurrencyDrop;
    }

    private void onEnemyDead()
    {
        //Change emissiveness of topObject
        //PlaySound(?)
    }

    // Start is called before the first frame update
    void Start()
    {
        rot = topCube.localRotation.y;
    }

    // Update is called once per frame
    void Update()
    {
        rot += 0.1f;
        topCube.localRotation.Set(topCube.localRotation.x, rot, topCube.localRotation.z, topCube.localRotation.w);
        //Do animation up-down
    }
}
