using UnityEngine;

[CreateAssetMenu(fileName = "TurretAbility_ElectricWire", 
    menuName = SOAssetPaths.CARDS_ABILITIES + "ElectricWire")]
public class TPADataModel_ElectricWire : ATurretPassiveAbilityDataModel
{
    [Header("ABILITY CONFIG")] 
    [SerializeField]    private AbilityDescriptionVariable _numberOfPassives;
    [Space(10)]
    [SerializeField] private AbilityDescriptionVariable _wireDamage;
    [SerializeField] private AbilityDescriptionVariable _wireStunDuration;
    [SerializeField] private Gradient _wireColorGradient;
    [Space(10)]
    [SerializeField] private AbilityDescriptionVariable _stackedWireDamage;
    [SerializeField] private AbilityDescriptionVariable _stackedWireStunDuration;
    [SerializeField] private Gradient _stackedWireColorGradient;
    
    
    
    
    public AbilityDescriptionVariable NumberOfPassives => _numberOfPassives;
    public AbilityDescriptionVariable WireStunDuration => _wireStunDuration;
    public AbilityDescriptionVariable WireDamage => _wireDamage;
    public Gradient WireColorGradient => _wireColorGradient;
    public AbilityDescriptionVariable StackedWireStunDuration => _stackedWireStunDuration;
    public AbilityDescriptionVariable StackedWireDamage => _stackedWireDamage;
    public Gradient StackedWireColorGradient => _stackedWireColorGradient;
    
    
    public override ATurretPassiveAbility MakePassiveAbility()
    {
        return new TurretPassiveAbility_ElectricWire(this);
    }
    
    
}