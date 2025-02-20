
using UnityEngine;

[CreateAssetMenu(fileName = "DisableMineConfig_NAME", 
    menuName = SOAssetPaths.ENEMY_HAZARDS + "DisableMineConfig")]
public class DisableMineConfig : ScriptableObject
{
    [SerializeField] private DisableMine.LogicConfig _logicConfig;
    
    [Space(20)]
    [SerializeField] private DisableMineView.Config _viewConfig;
    
    public DisableMine.LogicConfig LogicConfig => _logicConfig;
    public DisableMineView.Config ViewConfig => _viewConfig;
}