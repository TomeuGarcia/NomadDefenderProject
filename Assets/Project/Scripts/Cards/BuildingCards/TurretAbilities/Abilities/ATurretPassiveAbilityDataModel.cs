using UnityEngine;


public abstract class ATurretPassiveAbilityDataModel : ScriptableObject
{
    [Header("DEFAULTS")]
    [Header("View Add-Ons")] 
    [SerializeField] private ViewAddOnsConfig _optionalViewAddOns;
    
    [Space(10)]
    [Header("View")] 
    [SerializeField] private ViewConfig _viewConfig;

    [Header("Description")] 
    [SerializeField] private CardAbilityDescriptionModel _descriptionModel;
    

    [System.Serializable]
    public class ViewConfig
    {
        [SerializeField] private Sprite _sprite;
        [SerializeField] private Texture2D _spriteAsTexture;
        [SerializeField] private Color32 _color = UnityEngine.Color.white;
        
        public Sprite Sprite => _sprite;
        public Texture2D SpriteAsTexture => _spriteAsTexture;
        public Color32 Color => _color;
    }

    [System.Serializable]
    public class ViewAddOnsConfig
    {
        [SerializeField] private TurretViewAddOnConfig _turretAddOn = null;
        [SerializeField] private ProjectileViewAddOnConfig _projectileAddOn = null;
        
        public TurretViewAddOnConfig TurretAddOn => _turretAddOn;
        public ProjectileViewAddOnConfig ProjectileAddOn => _projectileAddOn;
    }
    
    
    public ViewConfig View => _viewConfig;
    public string Name => _descriptionModel.AbilityName;
    public ViewAddOnsConfig OptionalViewAddOns => _optionalViewAddOns;
    

    public EditableCardAbilityDescription MakeDescription()
    {
        return _descriptionModel.MakeEditableDescription();
    }
    
    public abstract ATurretPassiveAbility MakePassiveAbility();
}