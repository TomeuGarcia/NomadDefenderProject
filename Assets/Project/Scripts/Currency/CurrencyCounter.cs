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
    [SerializeField] private Transform currencyCountTextHolder;
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

        currencyCountText.DOComplete();
        currencyCountText.transform.DOComplete();
        currencyCountText.color = colorSpentCurrencyText;
        currencyCountText.DOColor(startColorAddedCurrencyText, delay * amount + 1.5f);
        currencyCountText.transform.DOPunchPosition(Vector3.down * 40f, 0.5f, 6);

        for (int i = 0; i < amount; ++i)
        {
            --initialCurrencyCount;
            --remainingAmountToSubtract;
            UpdateCurrencyCountText(initialCurrencyCount);
            yield return null;
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
        addedCurrencyText.transform.DOComplete();
        addedCurrencyText.DOComplete();
        addedCurrencyText.transform.rotation = Quaternion.identity;
        //addedCurrencyText.transform.localPosition = Vector3.zero;
        currencyCountTextHolder.DOComplete();
        currencyCountTextHolder.localScale = Vector3.one;


        // Animations
        addedCurrencyText.transform.rotation = Quaternion.identity;
        addedCurrencyText.transform.DOPunchRotation(Vector3.forward * 25f, 0.15f);
        //addedCurrencyText.transform.DOLocalMove(Vector3.up, 1.5f);

        currencyCountTextHolder.DOPunchScale(Vector3.one * 0.8f, 0.25f, 4);
        yield return new WaitForSeconds(0.25f);

        addedCurrencyText.DOColor(endColorAddedCurrencyText, 1f);
        yield return new WaitForSeconds(0.5f);

        addedCurrencyText.gameObject.SetActive(false);
    }

    public void PlayNotEnoughCurrencyAnimation()
    {
        currencyCountText.DOComplete();
        currencyCountText.transform.DOComplete();

        currencyCountText.DOBlendableColor(colorSpentCurrencyText, 0.25f)
            .OnComplete(() => currencyCountText.DOBlendableColor(startColorAddedCurrencyText, 0.25f));

        currencyCountText.transform.DOPunchScale(Vector3.one * 0.6f, 0.5f, 6);
    }

}
