using System;
using System.Collections.Generic;
using UnityEngine;
using Scripts.ObjectPooling;
using TMPro;
using System.Threading.Tasks;
using DG.Tweening;

public class FadingTextCharacter : RecyclableObject
{
    [SerializeField] private Transform _textsHolder;
    [SerializeField] private TextMeshProUGUI _frontText;
    [SerializeField] private TextMeshProUGUI _backgroundText;
    private Transform _defaultParent;

    internal override void RecycledInit() { }
    internal override void RecycledReleased() 
    {
        transform.SetParent(_defaultParent);
    }

    private void Awake()
    {
        _defaultParent = transform.parent;
    }

    public void Init(Transform parentFadingText, char character)
    {
        transform.SetParent(parentFadingText);
        transform.localScale = Vector3.one;
        _frontText.text = _backgroundText.text = character.ToString();
    }

    public async Task PlayAppearAnimation(FadingTextConfig.CharacterAnimation appearAnimationConfig)
    {
        _textsHolder.PunchScale(appearAnimationConfig.ScalePunch);
        _textsHolder.PunchRotation(appearAnimationConfig.RotationPunch);

        await Task.Delay(TimeSpan.FromSeconds(appearAnimationConfig.AppearDuration));
    }
}
