using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencySpawner : MonoBehaviour
{
    [SerializeField] private Pool currencyPool;
    [SerializeField] private Transform spawnTransform;

    private void OnEnable()
    {
         Enemy.OnEnemyDeathDropCurrency += SpawnCurrency;
         TDGameManager.OnEndGameResetPools += ResetPool;
    }

    private void OnDisable()
    {
        Enemy.OnEnemyDeathDropCurrency -= SpawnCurrency;
        TDGameManager.OnEndGameResetPools -= ResetPool;
    }

    private void SpawnCurrency(Enemy enemy)
    {
        GameObject currency = currencyPool.GetObject(enemy.Position + Vector3.up * 0.2f, Quaternion.identity, spawnTransform);
        currency.transform.SetParent(spawnTransform);

        currency.gameObject.GetComponent<DroppedCurrency>().SetValue(enemy.currencyDrop);
        currency.SetActive(true);
    }

    private void ResetPool()
    {
        currencyPool.ResetObjectsList();
    }
}
