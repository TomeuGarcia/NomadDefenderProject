using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencySpawner : MonoBehaviour, ITDCurrencySpawnService
{
    [SerializeField] private Pool currencyPool;
    [SerializeField] private Transform spawnTransform;

    private void OnEnable()
    {
        ServiceLocator.GetInstance().CurrencySpawnService = this;

        TDGameManager.OnEndGameResetPools += ResetPool;
    }

    private void OnDisable()
    {
        ServiceLocator.GetInstance().CurrencySpawnService = null;

        TDGameManager.OnEndGameResetPools -= ResetPool;
    }

    private void ResetPool()
    {
        currencyPool.ResetObjectsList();
    }

    public void SpawnCurrency(int currencyValue, Vector3 position)
    {
        GameObject currency = currencyPool.GetObject(position + (Vector3.up * 0.2f), Quaternion.identity, spawnTransform);
        currency.transform.SetParent(spawnTransform);

        currency.gameObject.GetComponent<DroppedCurrency>().SetValue(currencyValue);
        currency.SetActive(true);
    }
}
