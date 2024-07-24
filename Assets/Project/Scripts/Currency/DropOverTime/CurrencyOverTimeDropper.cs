using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyOverTimeDropper
{
    private readonly CurrencyDropOverTimeConfig _config;
    private readonly ITDCurrencySpawnService _currencySpawner;
    private readonly ICurrencyDropOverTimeView _view;
    private readonly Timer _dropTimer;

    private static Vector3 TEMP_CURRENCY_SPAWN_POSITION = new Vector3(0,-1000,0);

    public CurrencyOverTimeDropper(CurrencyDropOverTimeConfig config, ITDCurrencySpawnService currencySpawner,
        ICurrencyDropOverTimeView view)
    {
        _config = config;
        _currencySpawner = currencySpawner;
        _view = view;
        _dropTimer = new Timer(_config.DropPerioid);

        _config.OnValuesUpdated += OnConfigValuesUpdated;
    }

    ~CurrencyOverTimeDropper()
    {
        _config.OnValuesUpdated -= OnConfigValuesUpdated;
    }

    public void Start()
    {
        _view.Show();
    }

    public void Finish()
    {
        _view.Hide();
    }

    public void Update()
    {
        _dropTimer.Update(GameTime.DeltaTime);
        _view.Update(_dropTimer.Ratio01);
        if (_dropTimer.HasFinished())
        {
            _dropTimer.Reset();
            _view.PlayDropAnimation();
            DropCurrency();
        }
    }

    private void DropCurrency()
    {
        _currencySpawner.SpawnCurrency(_config.DropValue, TEMP_CURRENCY_SPAWN_POSITION);
    }

    private void OnConfigValuesUpdated()
    {
        _dropTimer.Duration = _config.DropPerioid;
    }
}
