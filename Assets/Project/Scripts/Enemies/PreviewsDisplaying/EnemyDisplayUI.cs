using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Scripts.ObjectPooling;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyDisplayUI : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private CanvasGroup _canvasGroup;

    private Material _imageMaterial;
    

    public void Init(EnemyTypeConfig enemyTypeConfig, bool withArmor)
    {
        CheckInitMaterial();
        _imageMaterial.SetInt("_EnemyPhotoIndex", enemyTypeConfig.View.PhotoIndex);
        _imageMaterial.SetInt("_WithArmor", withArmor ? 1 : 0);
    }

    private void CheckInitMaterial()
    {
        if (_imageMaterial == null)
        {
            _imageMaterial = new Material(_image.material);
            _image.material = _imageMaterial;
        }
    }
    
    public void SetText(string text)
    {
        _text.text = text;
    }


    public void Show()
    {
        _canvasGroup.alpha = 1;
    }
    public void Hide()
    {
        _canvasGroup.alpha = 0;
    }

    public void Activate()
    {
        gameObject.SetActive(true);
    }
    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
    
    public void PlayResetAnimation()
    {
        _text.transform.DOPunchScale(Vector3.one * 0.5f, 0.4f);
        _image.transform.DOPunchScale(Vector3.one * 0.5f, 0.4f);
    }
    public void PlayTextUpdateAnimation()
    {
        _text.transform.DOPunchScale(Vector3.one * 0.4f, 0.3f, 1, 1f);
        _text.DOColor(new Color(1, 0.5f, 0, 1), 0.15f)
            .OnComplete(() => _text.DOColor(Color.white, 0.15f));
    }
}
