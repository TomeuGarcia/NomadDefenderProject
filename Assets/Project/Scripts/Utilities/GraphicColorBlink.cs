using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphicColorBlink : MonoBehaviour
{
    [SerializeField] private Graphic[] _graphics;
    [SerializeField] private Color[] _colors;
    [SerializeField, Min(0)] private float _colorChangeDelay = 0.1f;

    private int _currentColorIndex = 0;

    private void OnEnable()
    {
        StartCoroutine(PlayBlink());
    }

    private IEnumerator PlayBlink()
    {
        UpdateGraphicsColor();
        while (gameObject.activeInHierarchy)
        {
            yield return new WaitForSeconds(_colorChangeDelay);

            IncrementColorIndex();
            UpdateGraphicsColor();
        }
    }


    private void UpdateGraphicsColor()
    {
        foreach (Graphic graphic in _graphics)
        {
            graphic.color = _colors[_currentColorIndex];
        }
    }

    private void IncrementColorIndex()
    {
        _currentColorIndex = (_currentColorIndex + 1) % _colors.Length;
    }
}
