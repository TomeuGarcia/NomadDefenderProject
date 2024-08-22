
public class TurretPassiveAbility_ExtraDamagePerCardInHand : ATurretPassiveAbility
{
    private readonly float _damageMultiplierPerCard;

    
    public TurretPassiveAbility_ExtraDamagePerCardInHand(TPADataModel_ExtraDamagePerCardInHand originalModel) 
        : base(originalModel)
    {
        _damageMultiplierPerCard = originalModel.DamagePercentBonusPerCard.Value / 100f;
        
        ApplyDescriptionCorrection(originalModel.DamagePercentBonusPerCard);
    }


    public override void OnBeforeDamagingEnemy(TurretDamageAttack damageAttack)
    {
        int numberOfCardsInHand = ServiceLocator.GetInstance().CardDrawer.GetCardsInHand().Length;

        int baseDamage = damageAttack.Damage;
        int extraDamage = (int)(baseDamage * (_damageMultiplierPerCard * numberOfCardsInHand));
        damageAttack.UpdateDamage(baseDamage + extraDamage);
    }
    
}