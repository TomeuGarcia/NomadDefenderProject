using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

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

    [System.Serializable]
    public class ViewConfig
    {
        [SerializeField] private string _displayName;
        
        [Space(10)] 
        [SerializeField, Min(0)] private float _photoScale = 1;
        [SerializeField] private Vector3 _photoRotation = new Vector3(20, 45, 20);
        [SerializeField] private Vector2 _photoOffset = Vector2.zero;

        [Space(10)] 
        [SerializeField] private ParticleTypes _particlesSpawn = ParticleTypes.EnemySpawnSmall;
        [SerializeField] private ParticleTypes _particlesDeath = ParticleTypes.EnemyDeathSmall;
        [SerializeField] private ParticleTypes _particlesAttack = ParticleTypes.EnemyAttack;
        
        public string DisplayName => '/' + _displayName;
        public float PhotoScale => _photoScale;
        public Quaternion PhotoRotation => Quaternion.Euler(_photoRotation);
        public Vector3 PhotoOffset => _photoOffset;
        public int PhotoIndex { get; set; }

        public ParticleTypes ParticlesSpawn => _particlesSpawn;
        public ParticleTypes ParticlesDeath => _particlesDeath;
        public ParticleTypes ParticlesAttack => _particlesAttack;
    }

    [SerializeField] private Stats _baseStats;
    [SerializeField] private ViewConfig _view;
    
    [Space(10)]
    [Required, ShowIf("IsArmored"), SerializeField] private EnemyTypeConfig _nonArmoredVersion;


    public Stats BaseStats => _baseStats;
    public ViewConfig View => _view;

    public EnemyTypeConfig NonArmored => IsArmored ? _nonArmoredVersion : this;

    public bool IsArmored => _baseStats.Armor > 0;
}
