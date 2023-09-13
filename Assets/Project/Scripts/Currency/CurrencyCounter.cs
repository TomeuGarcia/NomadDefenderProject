using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class CurrencyCounter : MonoBehaviour
{
    [Header("Currency Count")]
    [SerializeField, Min(0)] private int currencyCount;

    [Header("Components")]
    [SerializeField] private Transform currencyCountTextHolder;
    [SerializeField] public TextMeshProUGUI currencyCountText;
    [SerializeField] private TextMeshProUGUI addedCurrencyText;
    [SerializeField] private Image currencyImage;

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
    public delegate void CurrencyCounterAction2(int amountSpent);
    public event CurrencyCounterAction2 OnCurrencyAmountSpent;


    private void OnValidate()
    {
        UpdateCurrencyCountText(currencyCount);
    }

    private void Awake()
    {
        UpdateCurrencyCountText(currencyCount);

        addedCurrencyText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) && GameCheats.cheatsEnabled) AddCurrency(50);
    }

    private void OnEnable()
    {
        ServiceLocator.GetInstance().CurrencyCounter = this;

        DroppedCurrency.OnCurrencyGathered += AddCurrency;
    }

    private void OnDisable()
    {       
        DroppedCurrency.OnCurrencyGathered -= AddCurrency;
    }

    public void PlayTextAppearAnimation(float noTextDelay)
    {
        StartCoroutine(TextAppearAnimation(noTextDelay));
    }
    private IEnumerator TextAppearAnimation(float noTextDelay)
    {
        currencyImage.gameObject.SetActive(false);

        currencyCountText.text = "";        
        yield return new WaitForSeconds(noTextDelay);


        float t1 = 0.1f;

        currencyImage.gameObject.SetActive(true);
        GameAudioManager.GetInstance().PlayCardInfoMoveHidden();
        currencyImage.DOFade(0f, t1);
        yield return new WaitForSeconds(t1);
        currencyImage.DOFade(1f, t1);
        GameAudioManager.GetInstance().PlayCardInfoMoveHidden();
        yield return new WaitForSeconds(t1);

        int temp = 0;
        int step = 20;
        while (temp < currencyCount)
        {
            UpdateCurrencyCountText(temp);
            GameAudioManager.GetInstance().PlayCardInfoMoveShown();
            yield return new WaitForSeconds(t1);

            temp += step;
            t1 -= 0.05f;
        }

        UpdateCurrencyCountText(currencyCount);
        GameAudioManager.GetInstance().PlayCardInfoMoveShown();
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
        if (OnCurrencyAmountSpent != null) OnCurrencyAmountSpent(amount);
    }

    private IEnumerator SubtractCurrencyAnimation(int initialCurrencyCount, int amount)
    {
        isSubtracting = true;
        remainingAmountToSubtract = amount;

        currencyCountText.DOComplete();
        currencyCountText.transform.DOComplete();
        currencyCountText.color = colorSpentCurrencyText;
        currencyCountText.DOBlendableColor(startColorAddedCurrencyText, amount * Time.deltaTime * 0.9f);
        currencyCountText.transform.DOPunchPosition(Vector3.down * 40f, 0.5f, 6);

        int subtractAmountPerTick = 5;

        for (int i = 0; i < amount; i += subtractAmountPerTick)
        {
            initialCurrencyCount -= subtractAmountPerTick;
            remainingAmountToSubtract -= subtractAmountPerTick;
            UpdateCurrencyCountText(initialCurrencyCount);
            yield return null;
        }
        UpdateCurrencyCountText(currencyCount);

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

        addedCurrencyText.DOBlendableColor(endColorAddedCurrencyText, 1f);
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
