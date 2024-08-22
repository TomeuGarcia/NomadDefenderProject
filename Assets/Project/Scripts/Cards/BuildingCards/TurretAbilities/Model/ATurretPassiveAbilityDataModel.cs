using UnityEngine;


public abstract class ATurretPassiveAbilityDataModel : ScriptableObject
{
    [Header("DEFAULTS")]
    [Header("View")] 
    [SerializeField] private ViewConfig _viewConfig;
    
    [Header("Description")] 
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
    
    
    public ViewConfig View => _viewConfig;
    public string Name => _name;
    public string Description => _description;


    public abstract ATurretPassiveAbility MakePassiveAbility();
}