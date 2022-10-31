using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CurrencyCounter : MonoBehaviour
{
    [Header("Currency Count")]
    [SerializeField, Min(0)] private int currencyCount;

    [Header("Components")]
    [SerializeField] private TextMeshProUGUI currencyCountText;


    private void OnValidate()
    {
        UpdateCurrencyCountText();
    }

    private void Awake()
    {
        UpdateCurrencyCountText();
    }

    private void OnEnable()
    {
        DroppedCurrency.OnCurrencyGathered += AddCurrency;
    }

    private void OnDisable()
    {
        DroppedCurrency.OnCurrencyGathered -= AddCurrency;
    }


    private void UpdateCurrencyCountText()
    {
        currencyCountText.text = currencyCount.ToString();
    }

    private void AddCurrency(int amount)
    {
        currencyCount += amount;
        UpdateCurrencyCountText();
    }

    public void SubtractCurrency(int amount)
    {
        currencyCount -= amount;
        UpdateCurrencyCountText();
    }

    public bool HasEnoughCurrency(int amount)
    {
        return currencyCount >= amount;
    }


}
