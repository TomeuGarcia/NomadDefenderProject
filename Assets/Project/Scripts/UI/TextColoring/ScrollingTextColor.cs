using System;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class ScrollingTextColor : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    [SerializeField] private float _speed = 0.5f;
    
    
    private float _time = 0;

    private VertexGradient _textColorGradient;

    [SerializeField] private bool _useHandMadeGradient;
    [ShowIf("_useHandMadeGradient")] [SerializeField] private Gradient _colorGradient;

    private void Awake()
    {
        if (!_useHandMadeGradient)
        {
            _colorGradient = new Gradient();

            GradientColorKey[] colorKeys = new[]
            {
                new GradientColorKey(_text.colorGradient.topLeft, 0),
                new GradientColorKey(_text.colorGradient.topRight, 0.25f),
                new GradientColorKey(_text.colorGradient.bottomRight, 0.5f),
                new GradientColorKey(_text.colorGradient.bottomLeft, 0.75f),
                new GradientColorKey(_text.colorGradient.topLeft, 1),
            };

            GradientAlphaKey[] alphaKeys = new[]
            {
                new GradientAlphaKey(_text.colorGradient.topLeft.a, 0),
                new GradientAlphaKey(_text.colorGradient.topRight.a, 0.25f),
                new GradientAlphaKey(_text.colorGradient.bottomRight.a, 0.5f),
                new GradientAlphaKey(_text.colorGradient.bottomLeft.a, 0.75f),
                new GradientAlphaKey(_text.colorGradient.topLeft.a, 1),
            };
        
            _colorGradient.SetKeys(colorKeys, alphaKeys);
        }

        _textColorGradient = _text.colorGradient;
    }

    private void Update()
    {
        _textColorGradient.topLeft = _colorGradient.Evaluate((_time) % 1f);
        _textColorGradient.topRight = _colorGradient.Evaluate((_time + 0.25f) % 1f);
        _textColorGradient.bottomRight = _colorGradient.Evaluate((_time + 0.5f) % 1f);
        _textColorGradient.bottomLeft = _colorGradient.Evaluate((_time + 0.75f) % 1f);
        _text.colorGradient = _textColorGradient;
        
        _time += Time.deltaTime * _speed;
    }
}