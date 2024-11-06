using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.ObjectPooling;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyDisplayUI : RecyclableObject
{
    [SerializeField] private DynamicTextureReference _enemiesPhotoTextureReference;
    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _text;

    private Material _imageMaterial;
    
    private void Awake()
    {
        _imageMaterial = _image.material;
    }

    public void SetEnemyType(EnemyTypeConfig enemyTypeConfig)
    {
        _imageMaterial.SetTexture("_EnemyPhoto", _enemiesPhotoTextureReference.Texture);
        _imageMaterial.SetInt("_EnemyPhotoIndex", enemyTypeConfig.View.PhotoIndex);
    }
    
    public void SetText(string text)
    {
        _text.text = text;
    }


    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    
    
    internal override void RecycledInit() { }
    internal override void RecycledReleased() { }
}
