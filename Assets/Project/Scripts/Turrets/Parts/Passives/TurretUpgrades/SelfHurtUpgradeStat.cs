using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;


[CreateAssetMenu(fileName = "SelfHurtUpgradeStat", menuName = "TurretPassives/SelfHurtUpgradeStat")]
public class SelfHurtUpgradeStat : BasePassive
{
    [SerializeField] private SelfHurtUpgradeStatLogic logicPrefab;
    [SerializeField, Range(1, 5)] private int damageAmount = 1;

    public override void ApplyEffects(TurretBuilding owner)
    {
        SelfHurtUpgradeStatLogic spawnedLogic = Instantiate(logicPrefab, owner.transform);
        spawnedLogic.Init(owner, damageAmount);
    }

}
