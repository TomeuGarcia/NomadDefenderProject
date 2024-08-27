
public class TurretPassiveAbility_HitStun : ATurretPassiveAbility
{
    private readonly float _stunDuration;
    
    public TurretPassiveAbility_HitStun(TPADataModel_HitStun originalModel) 
        : base(originalModel)
    {
        _stunDuration = originalModel.StunDuration.FloatValue;

        ApplyDescriptionCorrection(originalModel.StunDuration);
    }

    public override void OnAfterDamagingEnemy(TurretDamageAttackResult damageAttackResult)
    {
        if (!damageAttackResult.Target.IsDead())
        {
            damageAttackResult.Target.GetStunned(_stunDuration);
        }
    }
    
}