using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyType_NAME", 
    menuName = SOAssetPaths.ENEMY_TYPES + "EnemyTypeConfig")]
public class EnemyTypeConfig : ScriptableObject
{
    [System.Serializable]
    public class Stats
    {
        [Header("STATS")]
        [SerializeField, Min(1)] private int _damage = 1;
        [SerializeField, Min(1)] private int _health = 40;
        [SerializeField, Min(0)] private int _armor = 0;
        [SerializeField, Min(0)] private int _currencyDrop = 20;
        [SerializeField, Min(0)] private float _moveSpeed = 1f;

        public int Damage => _damage;
        public int Health => _health;
        public int Armor => _armor;
        public int CurrencyDrop => _currencyDrop;
        public float MoveSpeed => _moveSpeed;
    }


    [SerializeField] private Stats _baseStats;
    [SerializeField] public Enemy.EnemyType type;
    public Stats BaseStats => _baseStats;
}
