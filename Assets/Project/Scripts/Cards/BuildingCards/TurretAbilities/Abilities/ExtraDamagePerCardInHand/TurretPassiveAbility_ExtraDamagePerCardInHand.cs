
public class TurretPassiveAbility_ExtraDamagePerCardInHand : ATurretPassiveAbility,
    ProjectileViewAddOn_StackedPower.IConfigurationSource
{
    private readonly float _damageMultiplierPerCard;
    private int _numberOfCardsInHand;

    private TurretViewAddOn_StackedPower _turretViewAddOn;

    
    public TurretPassiveAbility_ExtraDamagePerCardInHand(TPADataModel_ExtraDamagePerCardInHand originalModel) 
        : base(originalModel)
    {
        _damageMultiplierPerCard = originalModel.DamagePercentBonusPerCard.Value / 100f;

        ApplyDescriptionCorrection(originalModel.DamagePercentBonusPerCard);
    }

    protected override void DoOnBeforeShootingEnemyStart()
    {
        _numberOfCardsInHand = ServiceLocator.GetInstance().CardDrawer.GetCardsInHand().Length;
        ProjectileViewAddOn_StackedPower.ConfigurationSource = this;
    } 

    public override void OnBeforeDamagingEnemy(TurretDamageAttack damageAttack)
    {
        int baseDamage = damageAttack.Damage;
        int extraDamage = (int)(baseDamage * (_damageMultiplierPerCard * _numberOfCardsInHand));
        damageAttack.UpdateDamage(baseDamage + extraDamage);
    }

    int ProjectileViewAddOn_StackedPower.IConfigurationSource.GetNumberOfCards()
    {
        _numberOfCardsInHand = ServiceLocator.GetInstance().CardDrawer.GetCardsInHand().Length;
        return _numberOfCardsInHand;
    }

    protected override void OnTurretViewAddOnAdded(ATurretViewAddOn turretViewAddOn)
    {
        if(turretViewAddOn is TurretViewAddOn_StackedPower)
        {
            _turretViewAddOn = turretViewAddOn as TurretViewAddOn_StackedPower;
            _turretViewAddOn.ConfigurationSource = this;
        }
    }
}