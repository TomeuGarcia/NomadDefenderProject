using UnityEngine;

[CreateAssetMenu(fileName = "BuildingDisableBombConfig_NAME",
    menuName = SOAssetPaths.ENEMY_HAZARDS + "BuildingDisableBombConfig")]
public class BuildingDisableBombConfig : ScriptableObject
{
    [System.Serializable]
    public class AnimationConfig
    {
        [SerializeField, Min(0)] private float _duration = 1.0f;
        [SerializeField] private AnimationCurve _fadeEase;
        public float Duration => _duration;
        public AnimationCurve FadeEase => _fadeEase;
    }
    [System.Serializable]
    public class DropAnimationConfig
    {
        [SerializeField, Min(0)] private float _duration = 1.0f;
        [SerializeField] private AnimationCurve _ease;
        [SerializeField] private Vector3 _EMPEndPosition;
        [SerializeField] private Vector3 _EMPEndRotation;

        public float Duration => _duration;
        public AnimationCurve Ease => _ease;
        public Vector3 EMPEndPosition => _EMPEndPosition;
        public Vector3 EMPEndRotation => _EMPEndRotation;
    }

    [SerializeField, Min(0)] private float _delayBeforeApplying = 1.0f;

    [SerializeField] private AnimationConfig _animation;
    [SerializeField] private DropAnimationConfig _dropAnimation;

    [SerializeField] private BuildingDisableWaveConfig _waveConfig;

    public AnimationConfig Animation => _animation;
    public DropAnimationConfig DropAnimation => _dropAnimation;
    public float DelayBeforeApplying => _delayBeforeApplying;

    public BuildingDisableWaveConfig WaveConfig => _waveConfig;
}
