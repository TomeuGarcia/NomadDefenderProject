using UnityEngine;

[CreateAssetMenu(fileName = "TurretAbility_ChargedBlast", 
    menuName = SOAssetPaths.CARDS_ABILITIES + "ChargedBlast")]
public class TPADataModel_ExtraDamageSlowShooter : ATurretPassiveAbilityDataModel
{
    [Header("ABILITY CONFIG")]
    [SerializeField] private SlowFireRateTurretBuildingVisuals _slowFireRateVisualsPrefab;

    [SerializeField] private AnimationCurve _damageMultiplierOverTime;
    
    public SlowFireRateTurretBuildingVisuals VisualsPrefab => _slowFireRateVisualsPrefab;
    public float MaxTimeBetweenShots => _damageMultiplierOverTime.keys[^1].time;
    public AnimationCurve DamageMultiplierOverTimePer1 => _damageMultiplierOverTime;
    
    
    public override ATurretPassiveAbility MakePassiveAbility()
    {
        return new TurretPassiveAbility_ExtraDamageSlowShooter(this);
    }
}