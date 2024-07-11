using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FadingTextConfig_NAME", menuName = "UI/FadingTexts/FadingTextConfig")]
public class FadingTextConfig : ScriptableObject
{
    [Header("DEFAULTS")]
    [SerializeField] private Color _defaultImageBackgroundColor = Color.red;
    [SerializeField] private Vector3 _randomPositionOffsetBounds = new Vector3(1, 1, 1);
    [SerializeField] private Vector3 _fixedPositionOffset = new Vector3(0, 1, 0);

    [Space(20)]
    [Header("ANIMATIONS")]
    [SerializeField] private CharacterAnimation _characterAppearAnimation;
    [Space(20)]
    [SerializeField] private TextAnimation _textAppearAnimation;


    public Color DefaultImageBackgroundColor => _defaultImageBackgroundColor;
    public Vector3 RandomPositionOffset => VectorExtensions.RandomVector(_randomPositionOffsetBounds) + _fixedPositionOffset;

    public CharacterAnimation CharacterAppearAnimation => _characterAppearAnimation;
    public TextAnimation TextAppearAnimation => _textAppearAnimation;



    [System.Serializable]
    public class CharacterAnimation
    {
        [SerializeField, Range(0.0f, 10.0f)] private float _appearDuration = 0.2f;
        [SerializeField] private TweenPunchConfig _rotationPunch;
        [SerializeField] private TweenPunchConfig _scalePunch;
        
        public float AppearDuration => _appearDuration;
        public TweenPunchConfig RotationPunch => _rotationPunch;
        public TweenPunchConfig ScalePunch => _scalePunch;
    }

    [System.Serializable]
    public class TextAnimation
    {
        [SerializeField] private Vector3 _moveVelocity = new Vector3(0, 5, 0);
        [SerializeField, Range(0.0f, 10.0f)] private float _delayBeforeFadeOut = 0.5f;
        [SerializeField] private TweenConfig _backgroundImageAppearScale;
        [SerializeField] private TweenFadeConfig _groupFadeOut;

        public Vector3 MoveVelocity => _moveVelocity;
        public float DelayBeforeFadeOut => _delayBeforeFadeOut;
        public TweenConfig BackgroundImageAppearScale => _backgroundImageAppearScale;
        public TweenFadeConfig GroupFadeOut => _groupFadeOut;
    }

}
