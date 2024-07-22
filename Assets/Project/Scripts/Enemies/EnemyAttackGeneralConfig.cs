using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyAttackGeneralConfig", menuName = "Enemies/EnemyAttackGeneralConfig")]
public class EnemyAttackGeneralConfig : ScriptableObject
{
    [SerializeField] private LayerMask _pathLocationAttackLayer;

    public LayerMask PathLocationAttackLayer => _pathLocationAttackLayer;
}
