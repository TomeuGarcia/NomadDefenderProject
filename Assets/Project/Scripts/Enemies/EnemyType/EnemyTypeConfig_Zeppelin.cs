using UnityEngine;

[CreateAssetMenu(fileName = "EnemyType_ZeppelinOfNAME", 
    menuName = SOAssetPaths.ENEMY_TYPES + "EnemyTypeConfig_Zeppelin")]
public class EnemyTypeConfig_Zeppelin : EnemyTypeConfig
{
    [Space(30)]
    [Header("DEATH SPAWNS")]
    [SerializeField] private EnemyTypeConfig _spawnEnemyType;
    [SerializeField, Min(1)] private int _numberOfSpawns = 6;
    [SerializeField, Min(0)] private float _delayBetweenSpawns = 0.1f;
    [SerializeField, Min(0)] private float _randomSpawnOffsetRadius = 0.5f;
    
    public EnemyTypeConfig SpawnEnemyType => _spawnEnemyType;
    public int NumberOfSpawns => _numberOfSpawns;
    public float DelayBetweenSpawns => _delayBetweenSpawns;


    public Vector2 RandomSpawnOffset()
    {
        return new Vector2(
            Random.Range(-_randomSpawnOffsetRadius, _randomSpawnOffsetRadius), 
            Random.Range(-_randomSpawnOffsetRadius, _randomSpawnOffsetRadius));
    }
}