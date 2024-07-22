using UnityEngine;


[CreateAssetMenu(fileName = "SelfHurtUpgradeStat", 
    menuName = SOAssetPaths.TURRET_PARTS_BASEPASSIVES + "SelfHurtUpgradeStat")]
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
