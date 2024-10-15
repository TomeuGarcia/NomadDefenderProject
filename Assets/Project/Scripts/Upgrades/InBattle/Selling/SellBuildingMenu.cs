using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;
using Task = System.Threading.Tasks.Task;

public class SellBuildingMenu : MonoBehaviour
{
    private int _sellValue = 0;
    private const float SELL_VALUE_MULTIPLIER = 0.5f;

    [Header("BUILDING")] 
    [SerializeField] private Building _building;
    
    [Header("COMPONENTS")]
    [SerializeField] private StatUpgradeButton _sellTurretButton;
    [SerializeField] private TMP_Text _sellTurretSellValueText;
    [SerializeField] private Image _currencyIcon;
    [SerializeField] private RectTransform _sellValueHolder;
    
    [Header("VIEW")]
    [SerializeField] private GameObject _content;
    [SerializeField] private Image _backgroundBodyImage;
    [SerializeField] private Image _backgroundBarImage;

    public Action OnSellConfirmed;
    
    public void Init(int cardPlayCost)
    {
        _sellTurretButton.Init(OnButtonClicked, OnButtonHovered, OnButtonUnhovered);
        AddSellValue(cardPlayCost);
    }

    public void AddSellValue(int baseAmount)
    {
        _sellValue += Mathf.CeilToInt(baseAmount * SELL_VALUE_MULTIPLIER);
        UpdateSellValueText();
    }
    
    private void UpdateSellValueText()
    {
        _sellTurretSellValueText.text = '+' + _sellValue.ToString();

        if (_content.activeInHierarchy)
        {
            _sellValueHolder.DOPunchScale(Vector3.one * 0.15f, 0.2f, 4);
        }
    }

    public IEnumerator PlayOpenAnimation()
    {
        _backgroundBarImage.DOComplete();
        _backgroundBodyImage.DOComplete();
        _backgroundBarImage.fillAmount = 0;
        _backgroundBodyImage.fillAmount = 0;

        yield return _backgroundBarImage.DOFillAmount(1, 0.1f);
        yield return _backgroundBodyImage.DOFillAmount(1, 0.1f);
        
        _content.SetActive(true);

        float t1 = 0.075f;
        float t3 = t1 * 3;
        _sellTurretButton.SetupOpenAnimation();
        yield return _sellTurretButton.PlayOpenAnimation(t1, true, t3);
        
        _sellTurretButton.ButtonFadeIn();

    }
    public IEnumerator PlayCloseAnimation()
    {
        _backgroundBarImage.DOComplete();
        _backgroundBodyImage.DOComplete();
        _backgroundBarImage.fillAmount = 1;
        _backgroundBodyImage.fillAmount = 1;
        
        yield return _backgroundBodyImage.DOFillAmount(0, 0.1f);
        yield return _backgroundBarImage.DOFillAmount(0, 0.1f);

        _content.SetActive(false);

        float t1 = 0.075f;
        _sellTurretButton.SetupCloseAnimation();
        yield return _sellTurretButton.PlayCloseAnimation(t1);
        _sellTurretButton.StopButtonFade(false, false);
    }


    private async void OnButtonClicked()
    {
        OnSellConfirmed?.Invoke();

        await Task.Delay(TimeSpan.FromSeconds(0.5f));
        BuildingSellingController.Instance.SellBuilding(_building, _sellValue);
    }
    private void OnButtonHovered()
    {
        _currencyIcon.color = _sellTurretSellValueText.color = Color.cyan;
    }
    private void OnButtonUnhovered()
    {
        _currencyIcon.color = _sellTurretSellValueText.color = Color.white;
        _sellTurretButton.ButtonFadeIn();
    }
}