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

    [Header("Colors")]
    [SerializeField] private Color startColorAddedCurrencyText;
    [SerializeField] private Color endColorAddedCurrencyText;
    [SerializeField] private Color colorSpentCurrencyText;

    bool isSubtracting = false;
    int remainingAmountToSubtract = 0;
    Coroutine lastSubtractCoroutine;

    public delegate void CurrencyCounterAction();
    public event CurrencyCounterAction OnCurrencyAdded;
    public event CurrencyCounterAction OnCurrencySpent;


    private void OnValidate()
    {
        UpdateCurrencyCountText(currencyCount);
    }

    private void Awake()
    {
        UpdateCurrencyCountText(currencyCount);

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


    private void UpdateCurrencyCountText(int count)
    {
        currencyCountText.text = count.ToString();
    }

    private void AddCurrency(int amount)
    {
        StartCoroutine("ShowAddedCurrency", amount);
        currencyCount += amount;
        UpdateCurrencyCountText(currencyCount);

        if (OnCurrencyAdded != null) OnCurrencyAdded();
    }

    public void SubtractCurrency(int amount)
    {
        if (isSubtracting) StopCoroutine(lastSubtractCoroutine);

        lastSubtractCoroutine = StartCoroutine(SubtractCurrencyAnimation(currencyCount + remainingAmountToSubtract, amount + remainingAmountToSubtract));
        currencyCount -= amount;

        if (OnCurrencySpent != null) OnCurrencySpent();
    }

    private IEnumerator SubtractCurrencyAnimation(int initialCurrencyCount, int amount)
    {
        isSubtracting = true;
        remainingAmountToSubtract = amount;

        float delay = 0.05f;

        currencyCountText.DOKill();
        currencyCountText.transform.DOKill();
        currencyCountText.color = colorSpentCurrencyText;
        currencyCountText.DOColor(startColorAddedCurrencyText, delay * amount + 1.5f);
        currencyCountText.transform.DOPunchPosition(Vector3.down * 40f, 0.5f, 6);

        for (int i = 0; i < amount; ++i)
        {
            --initialCurrencyCount;
            --remainingAmountToSubtract;
            UpdateCurrencyCountText(initialCurrencyCount);
            yield return new WaitForSeconds(delay);
        }

        isSubtracting = false;
    }


    public bool HasEnoughCurrency(int amount)
    {
        return currencyCount >= amount;
    }


    private IEnumerator ShowAddedCurrency(int amount)
    {
        addedCurrencyText.gameObject.SetActive(true);
        addedCurrencyText.text = "+" + amount.ToString();
        addedCurrencyText.color = startColorAddedCurrencyText;

        // Reset
        addedCurrencyText.transform.DOKill();
        addedCurrencyText.DOKill();
        addedCurrencyText.transform.rotation = Quaternion.identity;
        //addedCurrencyText.transform.localPosition = Vector3.zero;
        currencyCountText.transform.DOKill();
        currencyCountText.transform.localScale = Vector3.one;


        // Animations
        addedCurrencyText.transform.rotation = Quaternion.identity;
        addedCurrencyText.transform.DOPunchRotation(Vector3.forward * 25f, 0.5f);
        //addedCurrencyText.transform.DOLocalMove(Vector3.up, 1.5f);

        currencyCountText.transform.DOPunchScale(Vector3.one * 0.8f, 1f, 4);
        yield return new WaitForSeconds(0.5f);

        addedCurrencyText.DOColor(endColorAddedCurrencyText, 1f);
        yield return new WaitForSeconds(1.0f);

        addedCurrencyText.gameObject.SetActive(false);
    }

}
