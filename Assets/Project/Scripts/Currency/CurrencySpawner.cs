using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencySpawner : MonoBehaviour
{
    [SerializeField] private Pool currencyPool;
    [SerializeField] private CurrencyCounter currencyCounter; ////////////

    private void OnEnable()
    {
         Enemy.OnEnemyDeathDropCurrency += SpawnCurrency;
    }

    private void SpawnCurrency(Enemy enemy)
    {
        GameObject currency = currencyPool.GetObject(enemy.Position + Vector3.up * 0.2f, Quaternion.identity);
        currency.SetActive(true);
        currency.gameObject.GetComponent<DroppedCurrency>().SetValue(enemy.currencyDrop);
        currency.gameObject.GetComponent<DroppedCurrency>().SetTarget(Vector3.up * 100.0f);
    }
}
