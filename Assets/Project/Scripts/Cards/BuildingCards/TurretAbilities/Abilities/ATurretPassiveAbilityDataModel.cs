using UnityEngine;


public abstract class ATurretPassiveAbilityDataModel : ScriptableObject
{
    [Header("DEFAULTS")]
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
    
    
    public ViewConfig View => _viewConfig;
    public string Name => _descriptionModel.AbilityName;


    public EditableCardAbilityDescription MakeDescription()
    {
        return _descriptionModel.MakeEditableDescription();
    }
    
    public abstract ATurretPassiveAbility MakePassiveAbility();
}