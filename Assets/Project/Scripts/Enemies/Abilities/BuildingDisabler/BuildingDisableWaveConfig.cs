using UnityEngine;

[CreateAssetMenu(fileName = "BuildingDisableWaveConfig_NAME", 
    menuName = SOAssetPaths.ENEMY_HAZARDS + "BuildingDisableWaveConfig")]
public class BuildingDisableWaveConfig : ScriptableObject
{
    [SerializeField, Min(0)] private float _radius = 1.0f;
    [SerializeField] private LayerMask _buildingsLayerMask;
    
    [SerializeField, Min(0)] private float _disableDuration = 8.0f;
    
    [SerializeField, Min(0)] private float _animationDuration = 2.0f;
    
    public float Radius => _radius;
    public LayerMask BuildingsLayerMask => _buildingsLayerMask;
    public float DisableDuration => _disableDuration;
    public float AnimationDuration => _animationDuration;
}