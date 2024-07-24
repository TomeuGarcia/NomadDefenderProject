using System;
using UnityEngine;


[CreateAssetMenu(fileName = "CurrencyDropOverTimeConfig_NAME",
    menuName = SOAssetPaths.TD_CURRENCYDROP + "CurrencyDropOverTimeConfig")]
public class CurrencyDropOverTimeConfig : ScriptableObject
{
    [SerializeField, Min(0)] private float _dropPerioid = 10f;
    [SerializeField, Min(0)] private int _dropValue = 20;
    
    public float DropPerioid => _dropPerioid;
    public int DropValue => _dropValue;



    public Action OnValuesUpdated;

    private void OnValidate()
    {
        OnValuesUpdated?.Invoke();
    }
}
