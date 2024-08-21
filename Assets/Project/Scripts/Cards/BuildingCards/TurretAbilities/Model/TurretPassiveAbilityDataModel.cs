using UnityEngine;

[CreateAssetMenu(fileName = "TurretAbility_NAME", 
    menuName = SOAssetPaths.CARDS_ABILITIES + "TurretPassiveAbilityDataModel")]
public class TurretPassiveAbilityDataModel : ScriptableObject
{
    [Header("TYPE")]
    [SerializeField] private TurretPassiveAbilityType _abilityType;

    [Header("VIEW")] 
    [SerializeField] private ViewConfig _viewConfig;
    
    [Header("DESCRIPTION")] 
    [SerializeField] private string _name;
    [SerializeField, TextArea(2, 5)] private string _description;
    
    
    
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
    
    
    public TurretPassiveAbilityType AbilityType => _abilityType;
    public ViewConfig View => _viewConfig;
    public string Name => _name;
    public string Description => _description;
    
}