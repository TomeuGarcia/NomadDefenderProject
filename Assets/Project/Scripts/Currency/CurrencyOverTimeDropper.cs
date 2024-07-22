using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyOverTimeDropper
{
    private readonly CurrencyDropOverTimeConfig _config;
    private readonly ITDCurrencySpawnService _currencySpawner;
    private readonly Timer _dropTimer;

    public CurrencyOverTimeDropper(CurrencyDropOverTimeConfig config, ITDCurrencySpawnService currencySpawner)
    {
        _config = config;
        _currencySpawner = currencySpawner;
        _dropTimer = new Timer(_config.DropPerioid);

        _config.OnValuesUpdated += OnConfigValuesUpdated;
    }

    ~CurrencyOverTimeDropper()
    {
        _config.OnValuesUpdated -= OnConfigValuesUpdated;
    }


    public void Update()
    {
        _dropTimer.Update(GameTime.DeltaTime);
        if (_dropTimer.HasFinished())
        {
            _dropTimer.Reset();
            DropCurrency();
        }
    }

    private void DropCurrency()
    {
        _currencySpawner.SpawnCurrency(_config.DropValue, Vector3.zero);
    }

    private void OnConfigValuesUpdated()
    {
        _dropTimer.Duration = _config.DropPerioid;
    }
}
