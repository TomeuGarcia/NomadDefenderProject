using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class CurrencyCounter : MonoBehaviour
{
    [Header("Currency Count")]
    [SerializeField, Min(0)] private int currencyCount;

    [Header("Components")]
    [SerializeField] public TextMeshProUGUI currencyCountText;
    [SerializeField] private TextMeshProUGUI addedCurrencyText;

    private void OnValidate()
    {
        UpdateCurrencyCountText();
    }

    private void Awake()
    {
        UpdateCurrencyCountText();

        addedCurrencyText.gameObject.SetActive(false);
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
        StartCoroutine("ShowAddedCurrency", amount);
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


    private IEnumerator ShowAddedCurrency(int amount)
    {
        addedCurrencyText.gameObject.SetActive(true);
        addedCurrencyText.text = "+" + amount.ToString();

        // rotate
        addedCurrencyText.transform.DOKill();
        addedCurrencyText.transform.rotation = Quaternion.identity;
        addedCurrencyText.transform.DOPunchRotation(Vector3.forward * 20f, 0.5f);

        yield return new WaitForSeconds(1.5f);
        addedCurrencyText.gameObject.SetActive(false);
    }

}
