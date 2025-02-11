using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[System.Serializable]
public class MachineTweenConfig
{
    [SerializeField] private float _value;
    [SerializeField, Range(0.01f, 5.0f)] private float _duration;
    [SerializeField] private Ease _ease = Ease.InOutSine;
    [SerializeField, Range(0.01f, 5.0f)] private float _delay;

    public float Value => _value;
    public float Duration => _duration;
    public Ease Ease => _ease;
    public float Delay => _delay;

    public MachineTweenConfig(float value, float duration, Ease ease, float delay)
    {
        _value = value;
        _duration = duration;
        _ease = ease;
        _delay = delay;
    }

    public void SetDuration(float duration)
    {
        _duration = duration;
    }

    public MachineTweenConfig Undo()
    {
        return new MachineTweenConfig(-_value, _duration, _ease, _delay);
    }
}