using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyOverTimeDropManager : MonoBehaviour
{
    [Header("TEMP. CURRENCY DROP")]
    [SerializeField] private CurrencyDropOverTimeConfig _currencyDropOverTime;
    private CurrencyOverTimeDropper _currencyOverTimeDropper;

    private bool _canGenerateCurrency;



    private void Start()
    {
        _currencyOverTimeDropper = new CurrencyOverTimeDropper(_currencyDropOverTime, ServiceLocator.GetInstance().CurrencySpawnService);
        _canGenerateCurrency = false;

        StartCoroutine(ControlCanGenerateCurrency());        
    }


    private void Update()
    {
        if (!_canGenerateCurrency)
        {
            return;
        }

        _currencyOverTimeDropper.Update();
    }


    private IEnumerator ControlCanGenerateCurrency()
    {
        ITDGameState tdGameState = ServiceLocator.GetInstance().TDGameState;

        yield return new WaitUntil(() => tdGameState.FirstCardWasPlayed);
        _canGenerateCurrency = true;
        yield return new WaitUntil(() => tdGameState.GameHasFinished);
        _canGenerateCurrency = false;
    }

}
