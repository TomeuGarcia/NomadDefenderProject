using UnityEngine;

[CreateAssetMenu(fileName = "SpeedBoosterConfig_NAME", 
    menuName = SOAssetPaths.ENEMY_SPEEDBOOSTER + "SpeedBoosterConfig")]
public class SpeedBoosterConfig : ScriptableObject
{
    [SerializeField, Min(0)] private float _speedBoosterLifetime = 5.0f;
        
    [SerializeField] private SpeedBooster.Boost _boost;
        
    public float SpeedBoosterLifetime => _speedBoosterLifetime;
    public SpeedBooster.Boost Boost => _boost;
}