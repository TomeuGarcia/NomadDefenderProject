using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyOverTimeDropManager : MonoBehaviour
{
    [Header("TEMP. CURRENCY DROP")]
    [SerializeField] private CurrencyDropOverTimeConfig _currencyDropOverTime;
    [SerializeField] private CurrencyDropOverTimeCanvasView _view;
    private CurrencyOverTimeDropper _currencyOverTimeDropper;

    private bool _canGenerateCurrency;



    private void Start()
    {
        _currencyOverTimeDropper = new CurrencyOverTimeDropper(_currencyDropOverTime, ServiceLocator.GetInstance().CurrencySpawnService, _view);
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
        _currencyOverTimeDropper.Start();
        _canGenerateCurrency = true;
        yield return new WaitUntil(() => tdGameState.GameHasFinished);
        _canGenerateCurrency = false;
        _currencyOverTimeDropper.Finish();
    }

}
