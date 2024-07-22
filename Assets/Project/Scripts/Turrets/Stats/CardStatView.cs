using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NaughtyAttributes;

public class CardStatView : MonoBehaviour
{
    [Header("CONFIG")]
    [Required] [SerializeField] private CardStatViewConfig _config;
    [SerializeField] private bool _configureOnAwake = true;
    
    [Header("COMPONENS")]
    [SerializeField] private Image _iconImage;
    [SerializeField] private TMP_Text _valueText;



    private void OnValidate()
    {
        if (HasAllReferences())
        {
            ConfigureComponents();
        }
    }

    void Awake()
    {
        if (_configureOnAwake)
        {
            ConfigureComponents();
        }        
    }

    private void ConfigureComponents()
    {
        _iconImage.sprite = _config.Icon;
        _iconImage.color = _config.IconColor;

        gameObject.name = _config.GameObjectName;
        _valueText.gameObject.name = _config.ValueGameObjectName;
        _valueText.color = _config.TextColor;
    }

    private bool HasAllReferences()
    {
        return _config && _iconImage && _valueText;
    }

}
