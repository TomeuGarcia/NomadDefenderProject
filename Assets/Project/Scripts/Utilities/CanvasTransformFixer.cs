using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[RequireComponent(typeof(RectTransform))]
public class CanvasTransformFixer : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private Vector3 _anchoredPosition;


    private void Awake()
    {
        FixPosition();
    }

    [Button]
    private void CopyCurrentPosition()
    {
        _anchoredPosition = _rectTransform.anchoredPosition;
    }

    private void FixPosition()
    {
        _rectTransform.anchoredPosition = _anchoredPosition;
    }
}
