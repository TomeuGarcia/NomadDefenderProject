using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TurretPartAttack_Berserker", 
    menuName = SOAssetPaths.TURRET_PARTS_ATTACKS + "TurretPartAttack_Berserker")]
public class TurretPartAttack_Berserker : TurretPartAttack
{
    [Space(40)]
    [SerializeField] private BerserkerTurretBuildingVisuals berserkerVisualsPrefab;

    public override void OnTurretPlaced(TurretBuilding owner, Material turretMaterial)
    {
        BerserkerTurretBuildingVisuals berserkerVisuals = GameObject.Instantiate(berserkerVisualsPrefab, owner.transform);

        berserkerVisuals.TurretPlacedInit(owner, turretMaterial);
    }

}
