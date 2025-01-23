using UnityEngine;

[CreateAssetMenu(fileName = "EnemyType_Dasher", 
    menuName = SOAssetPaths.ENEMY_TYPES + "EnemyTypeConfig_Dasher")]
public class EnemyTypeConfig_Dasher : EnemyTypeConfig
{
    [Space(30)]
    [Header("DASH")]
    [SerializeField, Min(0)] private float _stopDuration = 1.0f;
    [SerializeField, Min(0)] private float _dashTravelDistance = 5.0f;
    public float StopDuration => _stopDuration;
    public float DashTravelDistance => _dashTravelDistance;
    
    
    
}