using UnityEngine;


[CreateAssetMenu(fileName = "NewTurretCardDataModel", 
    menuName = SOAssetPaths.CARDS_BUILDINGS + "TurretCardDataModel")]
public class TurretCardDataModel : ScriptableObject
{
    [Header("COST")]
    [SerializeField, Min(0)] public int cardCost;

    [Header("PARTS")] 
    [SerializeField] private TurretCardPartsGroup _partsGroup;
    public TurretCardPartsGroup SharedPartsGroup => _partsGroup;
    public TurretCardPartsGroup MakePartsGroup() => new TurretCardPartsGroup(_partsGroup);
    
    
    [Header("LEVEL")]
    [Range(1, MAX_CARD_LEVEL)] public int cardLevel = 1;
    
    
    public const int MAX_CARD_LEVEL = 3;
}