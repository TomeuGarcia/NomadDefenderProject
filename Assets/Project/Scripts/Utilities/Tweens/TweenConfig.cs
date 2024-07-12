using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[System.Serializable]
public class TweenConfig
{
    [SerializeField] private Vector3 _value = new Vector3(0.5f, 0.5f, 0);
    [SerializeField, Range(0.01f, 5.0f)] private float _duration = 0.3f;
    [SerializeField] private Ease _ease = Ease.InOutSine;

    public Vector3 Value => _value;
    public float Duration => _duration;
    public Ease Ease => _ease;

    public TweenConfig(Vector3 value, float duration, Ease ease)
    {
        _value = value;
        _duration = duration;
        _ease = ease;
    }

    public void SetDuration(float duration)
    {
        _duration = duration;
    }

    public TweenConfig Undo()
    {
        return new TweenConfig(-_value, _duration, _ease);
    }

}