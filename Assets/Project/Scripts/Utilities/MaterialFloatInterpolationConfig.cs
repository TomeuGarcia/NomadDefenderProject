using System;
using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "MF_InterpolationConfig_NAME",
        menuName = "VFX/MF_InterpolationConfig", order = 1)]
public class MaterialFloatInterpolationConfig : ScriptableObject
{
    [SerializeField] private float _delay;

    [SerializeField] private string _name;
    [SerializeField] private float _duration;
    [SerializeField] private float _endValue = 1.0f;
    [SerializeField] private Ease _ease;

    [SerializeField] private bool _waitForCompletion;

    private int _property;

    public float Delay => _delay;
    public string Name => _name;
    public int ID => _property;
    public float Duration => _duration;
    public float EndValue => _endValue;
    public Ease Ease => _ease;
    public bool WaitForCompletion => _waitForCompletion;

    private void Awake()
    {
        _property = Shader.PropertyToID(_name);
    }
}