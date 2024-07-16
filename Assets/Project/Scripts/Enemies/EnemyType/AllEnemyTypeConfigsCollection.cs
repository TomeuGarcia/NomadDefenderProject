using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;


[CreateAssetMenu(fileName = "EnemyTypesCollection", 
    menuName = SOAssetPaths.ENEMY_TYPES + "EnemyTypesCollection")]
public class AllEnemyTypeConfigsCollection : ScriptableObject
{
    [Expandable] [SerializeField] private EnemyTypeConfig[] _enemyTypes;
    public EnemyTypeConfig[] EnemyTypes => _enemyTypes;

    public void UpdateEnemyTypes(EnemyTypeConfig[] enemyTypes)
    {
        _enemyTypes = enemyTypes;
    }
}
