using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[System.Serializable]
public class TweenColorConfig
{
    [SerializeField] private Color _value = Color.white;
    [SerializeField, Range(0.01f, 5.0f)] private float _duration = 0.3f;
    [SerializeField] private Ease _ease = Ease.InOutSine;

    public Color Value => _value;
    public float Duration => _duration;
    public Ease Ease => _ease;
}