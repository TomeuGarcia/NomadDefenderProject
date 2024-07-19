using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;


[CreateAssetMenu(fileName = "TurretCardPlayCostCalculator", 
    menuName = SOAssetPaths.CARDS_COST + "TurretCardPlayCostCalculator")]
public class TurretCardPlayCostCalculator : ScriptableObject
{
    [Header("Enemies")]
    [SerializeField] private AllEnemyTypeConfigsCollection _enemyTypesCollection;

    [Header("Global variables")]
    [SerializeField, Range(0f, 1f)] private float _uptimePerSecond = 1.0f;
    [SerializeField, Range(0f, 200f)] private float _returnOnInvestmentPeriod = 50.0f;
    [SerializeField, Range(0f, 10f)] private float _pricingFactor = 1.0f;

    [Space(50)]
    [Header("TESTING")]
    [SerializeField] private string _testingName = "Test Turret";
    [SerializeField] private InputData _testingInputData;

    [System.Serializable]
    public struct InputData
    {
        [SerializeField, Range(0, 200)] private int _damagePerHit;
        [SerializeField, Range(0, 30)] private float _hitsPerSecond;
        [SerializeField, Range(0, 10)] private float _enemiesHitPerAttack;

        public InputData(int damagePerHit, float hitsPerSecond, float enemiesHitPerAttack)
        {
            _damagePerHit = damagePerHit;
            _hitsPerSecond = hitsPerSecond;
            _enemiesHitPerAttack = enemiesHitPerAttack;
        }

        public float ComputeDamagePerSecond()
        {
            return _damagePerHit * _hitsPerSecond * _enemiesHitPerAttack;
        }
    }


    public int ComputePlayCost(InputData turretData)
    {
        float damagePerSecond = turretData.ComputeDamagePerSecond();
        float uptimePerSecond = _uptimePerSecond;
        float averageEarningPer1Damage = GetEarningsPerHealthAllEnemies();
        float earningsPerSecond = (damagePerSecond * uptimePerSecond) * averageEarningPer1Damage;

        float returnOnInvestmentPeriod = _returnOnInvestmentPeriod;

        float playCost = earningsPerSecond * returnOnInvestmentPeriod * _pricingFactor;

        int roundedPlayCost = Mathf.RoundToInt(playCost / 10f) * 10;

        return roundedPlayCost;
    }


    private float GetEarningsPerHealthAllEnemies()
    {
        float earningsPerHealth = 0f;

        foreach (EnemyTypeConfig enemyType in _enemyTypesCollection.EnemyTypes)
        {
            earningsPerHealth += (float)enemyType.BaseStats.CurrencyDrop / (float)(enemyType.BaseStats.Health + enemyType.BaseStats.Armor);
        }

        earningsPerHealth /= _enemyTypesCollection.EnemyTypes.Length;

        return earningsPerHealth;
    }


    [Button()]
    private void PrintTesting()
    {
        Debug.Log("Cost of " + _testingName + ": [" + ComputePlayCost(_testingInputData) + "]");
    }
}
