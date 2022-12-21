using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyBase : TurretPartBase_Prefab
{

    [SerializeField] private int quantityToIncreaseCurrencyDrop;
    public override void Init(TurretBuilding turretOwner, float turretRange)
    {
        base.Init(turretOwner, turretRange);

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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
