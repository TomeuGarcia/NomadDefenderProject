using UnityEngine;

[CreateAssetMenu(fileName = "BuildingDisableWaveConfig_NAME", 
    menuName = SOAssetPaths.ENEMY_HAZARDS + "BuildingDisableWaveConfig")]
public class BuildingDisableWaveConfig : ScriptableObject
{
    [System.Serializable]
    public class AnimationConfig
    {
        [SerializeField, Min(0)] private float _duration = 1.0f;
        [SerializeField] private AnimationCurve _fadeEase;
        public float Duration => _duration;
        public AnimationCurve FadeEase => _fadeEase;
    }
    
    
    [SerializeField, Min(0)] private float _radius = 1.0f;
    [SerializeField] private LayerMask _buildingsLayerMask;
    
    [SerializeField, Min(0)] private float _disableDuration = 8.0f;
    [SerializeField, Min(0)] private float _delayBeforeApplying = 1.0f;
    
    
    [SerializeField] private AnimationConfig _animation;
    
    
    public float Radius => _radius;
    public LayerMask BuildingsLayerMask => _buildingsLayerMask;
    public float DelayBeforeApplying => _delayBeforeApplying;
    public float DisableDuration => _disableDuration;
    public AnimationConfig Animation => _animation;
}