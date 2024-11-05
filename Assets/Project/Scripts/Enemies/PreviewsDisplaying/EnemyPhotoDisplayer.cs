using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyPhotoDisplayer : MonoBehaviour
{
    [SerializeField] private EnemyTypeConfig _enemyTypeConfig;
    [SerializeField] private Image _photoImage;

    private Material _photoMaterial;

    private void Awake()
    {
        _photoMaterial = _photoImage.material;
    }

    private void Update()
    {
        _photoMaterial.SetInt("_EnemyPhotoIndex", _enemyTypeConfig.PhotoIndex);
    }
    
}
