using UnityEngine;

[CreateAssetMenu(fileName = "CardViewConfig", 
    menuName = SOAssetPaths.CARDS_BUILDINGS + "CardViewConfig")]
public class BuildingCardViewConfig : ScriptableObject
{
    [SerializeField] private Material _maxCardLevelTextMaterial;
    [SerializeField] private Material _defaultCardLevelTextMaterial;
    
    
    public Material MaxCardLevelTextMaterial => _maxCardLevelTextMaterial;
    public Material DefaultCardLevelTextMaterial => _defaultCardLevelTextMaterial;
    
    
}