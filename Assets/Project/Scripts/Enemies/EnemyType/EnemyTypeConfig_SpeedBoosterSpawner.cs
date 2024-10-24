using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyType_SpeedBoosterSpawner", 
    menuName = SOAssetPaths.ENEMY_TYPES + "EnemyTypeConfig_SpeedBoosterSpawner")]
public class EnemyTypeConfig_SpeedBoosterSpawner : EnemyTypeConfig
{
    [Space(30)]
    [Header("SPEED BOOSTER")]
    [SerializeField] private Vector2 _startSpawnRandomInterval = new Vector2(2.0f, 3.0f);
    [SerializeField, Min(0)] private float _spawnTravelDistance = 5.0f;
    [Expandable] [SerializeField] private SpeedBoosterConfig _speedBoosterConfig;
    
    public float StartSpawningWaitDuration => Random.Range(_startSpawnRandomInterval.x, _startSpawnRandomInterval.y);
    public float SpawnTravelDistance => _spawnTravelDistance;
    public SpeedBoosterConfig SpeedBoosterConfig => _speedBoosterConfig;
}