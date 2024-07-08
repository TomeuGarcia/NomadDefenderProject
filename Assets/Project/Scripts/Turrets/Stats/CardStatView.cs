using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

public class CardStatView : MonoBehaviour
{
    [Header("CONFIG")]
    [Required] [SerializeField] private CardStatViewConfig _config;
    
    [Header("COMPONENS")]
    [SerializeField] private Image _iconImage;
    [SerializeField] private GameObject _valueText;



    private void OnValidate()
    {
        if (HasAllReferences())
        {
            ConfigureComponents();
        }
    }

    void Awake()
    {
        ConfigureComponents();
    }

    private void ConfigureComponents()
    {
        _iconImage.sprite = _config.Icon;
        _iconImage.color = _config.Color;

        gameObject.name = _config.GameObjectName;
        _valueText.name = _config.ValueGameObjectName;
    }

    private bool HasAllReferences()
    {
        return _config && _iconImage && _valueText;
    }

}
