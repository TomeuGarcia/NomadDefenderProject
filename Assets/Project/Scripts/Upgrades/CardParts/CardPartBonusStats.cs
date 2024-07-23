using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPartBonusStats : CardPart
{
    [SerializeField] private CardPartBonusStatsItem _damageItem;
    [SerializeField] private CardPartBonusStatsItem _shotsPerSecondItem;
    [SerializeField] private CardPartBonusStatsItem _radiusRangeItem;

    private TurretStatsMultiplicationSnapshot _statsSnapshotUpgrade;

    public void Configure(TurretStatsUpgradeModel model)
    {
        _statsSnapshotUpgrade = model.MakeStatMultiplicationSnapshot();

        model.MakeStatStrings(
            out TurretStatsUpgradeModel.StatString damageStatString,
            out TurretStatsUpgradeModel.StatString shotsPerSecondStatString,
            out TurretStatsUpgradeModel.StatString radiusRangeStatString
        );
        _damageItem.Init(damageStatString);
        _shotsPerSecondItem.Init(shotsPerSecondStatString);
        _radiusRangeItem.Init(radiusRangeStatString);
    }

    public override void Init()
    {
        //attackImage.sprite = turretPartAttack.abilitySprite;
        //attackImage.color = turretPartAttack.materialColor;
    }

    protected override void DoShowInfo() { }


    public void ApplyStatsModification(ITurretStatsBonusController turretCardStatsController)
    {
        turretCardStatsController.AddBonusStatsMultiplication(_statsSnapshotUpgrade);
    }

}
