using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencySpawner : MonoBehaviour
{
    [SerializeField] private Pool currencyPool;

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
        GameObject currency = currencyPool.GetObject(enemy.Position + Vector3.up * 0.2f, Quaternion.identity);
        currency.SetActive(true);
        Debug.Log(enemy.currencyDrop);
        currency.gameObject.GetComponent<DroppedCurrency>().SetValue(enemy.currencyDrop);
    }

    private void ResetPool()
    {
        currencyPool.ResetObjectsList();
    }
}
