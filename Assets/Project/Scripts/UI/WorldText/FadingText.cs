using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.ObjectPooling;
using DG.Tweening;
using System.Threading.Tasks;

public class FadingText : RecyclableObject
{
    [Header("COMPONENTS")]
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private Transform _charactersHolder;

    private List<FadingTextCharacter> _fadingTextCharacters;
    private Vector3 _moveVelocity;


    internal override void RecycledInit() { }
    internal override void RecycledReleased() { }

    private void Awake()
    {
        _fadingTextCharacters = new List<FadingTextCharacter>(3);
    }

    private void Update()
    {
        transform.position = transform.position + (_moveVelocity * Time.deltaTime);
    }

    public void Init(IFadingTextsFactory.TextSpawnData textSpawnData,
        IFadingTextCharactersFactory charactersFactory, FadingTextConfig config)
    {
        _fadingTextCharacters.Clear();
        string Content = textSpawnData.Content;
        foreach (char character in Content)
        {
            WithCharacter(charactersFactory, character);
        }

        transform.position = textSpawnData.WorldPosition + config.RandomPositionOffset;
        _backgroundImage.color = textSpawnData.BackgroundImageColor;
        _backgroundImage.transform.localScale = Vector3.zero;
        _canvasGroup.alpha = 1;

        PlayAppearAnimation(config);
    }

    private void WithCharacter(IFadingTextCharactersFactory charactersFactory, char character)
    {
        FadingTextCharacter fadingTextCharacter = charactersFactory.SpawnFadingTextCharacter(_charactersHolder, character);
        _fadingTextCharacters.Add(fadingTextCharacter);
    }

    private async void PlayAppearAnimation(FadingTextConfig config)
    {
        FadingTextConfig.TextAnimation _textAppearAnimation = config.TextAppearAnimation;

        _moveVelocity = _textAppearAnimation.MoveVelocity;
        
        _backgroundImage.transform.Scale(_textAppearAnimation.BackgroundImageAppearScale);       
        foreach (FadingTextCharacter fadingTextCharacter in _fadingTextCharacters)
        {
            await fadingTextCharacter.PlayAppearAnimation(config.CharacterAppearAnimation);
        }

        await Task.Delay(TimeSpan.FromSeconds(_textAppearAnimation.DelayBeforeFadeOut));
        
        _canvasGroup.Fade(config.TextAppearAnimation.GroupFadeOut)
            .OnComplete(AppearAnimationCompleted);
    }

    private void AppearAnimationCompleted()
    {
        _backgroundImage.transform.DOComplete();
        _canvasGroup.DOComplete();

        foreach(FadingTextCharacter fadingTextCharacter in _fadingTextCharacters)
        {
            fadingTextCharacter.Recycle();
        }

        Recycle();
    }

}
