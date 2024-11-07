using UnityEngine;

[CreateAssetMenu(fileName = "BuildingSellingModel_NAME", 
    menuName = SOAssetPaths.CARDS_BUILDINGS + "BuildingSellingModel")]
public class BuildingSellingModel : ScriptableObject
{
    [SerializeField] private BuildingSellingConfig _config;
    public BuildingSellingConfig Config => _config;
}