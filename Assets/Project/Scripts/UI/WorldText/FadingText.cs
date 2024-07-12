using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.ObjectPooling;
using DG.Tweening;
using System.Threading.Tasks;
using System.Collections;

public class FadingText : RecyclableObject
{
    [Header("COMPONENTS")]
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Transform _backgroundImageHolder;
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
            WithCharacter(charactersFactory, character, textSpawnData);
        }

        transform.position = textSpawnData.WorldPosition + config.RandomPositionOffset;
        _backgroundImageHolder.localScale = Vector3.zero;
        _canvasGroup.alpha = 1;

        StartCoroutine(PlayAppearAnimation(config));
    }

    private void WithCharacter(IFadingTextCharactersFactory charactersFactory, char character, IFadingTextsFactory.TextSpawnData textSpawnData)
    {
        FadingTextCharacter fadingTextCharacter = charactersFactory.SpawnFadingTextCharacter(_charactersHolder, character, textSpawnData.TextColor);
        _fadingTextCharacters.Add(fadingTextCharacter);
    }

    private IEnumerator PlayAppearAnimation(FadingTextConfig config)
    {
        FadingTextConfig.TextAnimation textAppearAnimation = config.TextAppearAnimation;

        _moveVelocity = textAppearAnimation.MoveVelocity;

        _canvasGroup.transform.PunchScale(textAppearAnimation.AppearScale);

        _backgroundImageHolder.Scale(textAppearAnimation.BackgroundImageAppearScale);
        _backgroundImageHolder.LocalRotateBy(textAppearAnimation.BackgroundRotationTween);

        foreach (FadingTextCharacter fadingTextCharacter in _fadingTextCharacters)
        {
            yield return fadingTextCharacter.PlayAppearAnimation(config.CharacterAppearAnimation);
        }

        yield return new WaitForSeconds(textAppearAnimation.DelayBeforeFadeOut);
        
        _canvasGroup.Fade(config.TextAppearAnimation.GroupFadeOut)
            .OnComplete(AppearAnimationCompleted);
    }

    private void AppearAnimationCompleted()
    {
        _backgroundImageHolder.DOComplete();
        _canvasGroup.DOComplete();

        foreach(FadingTextCharacter fadingTextCharacter in _fadingTextCharacters)
        {
            fadingTextCharacter.Recycle();
        }

        Recycle();
    }

}
