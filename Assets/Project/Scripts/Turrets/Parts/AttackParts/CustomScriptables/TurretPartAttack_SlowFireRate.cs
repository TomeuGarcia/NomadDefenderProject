using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "TurretPartAttack_SlowFireRate", 
    menuName = SOAssetPaths.TURRET_PARTS_ATTACKS + "TurretPartAttack_SlowFireRate")]
public class TurretPartAttack_SlowFireRate : TurretPartAttack
{
    [Space(40)]
    [SerializeField] private SlowFireRateTurretBuildingVisuals slowFireRateVisualsPrefab;

    public override void OnTurretPlaced(TurretBuilding owner, Material turretMaterial)
    {
        SlowFireRateTurretBuildingVisuals slowFireRateVisuals = GameObject.Instantiate(slowFireRateVisualsPrefab, owner.transform);

        slowFireRateVisuals.TurretPlacedInit(owner);
    }

}