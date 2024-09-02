using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NaughtyAttributes;

public class CardStatView : MonoBehaviour
{
    [Header("CONFIG")]
    [SerializeField] private CardStatViewMetaType _metaType;
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
        _iconImage.color = new Color(_config.IconColor.r, _config.IconColor.g, _config.IconColor.b, _iconImage.color.a);

        gameObject.name = _metaType.GetGameObjectName(_config.Name);
        _valueText.gameObject.name = _metaType.GetValueGameObjectName(_config.Name);
        _valueText.color = _config.TextColor;
    }

    private bool HasAllReferences()
    {
        return _config && _iconImage && _valueText;
    }

}
