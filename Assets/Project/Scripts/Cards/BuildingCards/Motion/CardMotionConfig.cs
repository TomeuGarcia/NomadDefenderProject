using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


[CreateAssetMenu(fileName = "CardMotionConfig",
    menuName = SOAssetPaths.CARDS_VIEW + "CardMotionConfig")]
public class CardMotionConfig : ScriptableObject
{
    [Header("EASINGS")]
    [Header("Repositioning")]
    [SerializeField] private Ease _repositioning_Move_Ease = Ease.InOutSine;
    public Ease Repositioning_Move_Ease => _repositioning_Move_Ease;


    [Header("Standard")]
    [SerializeField] private Ease _toStandard_Move_Ease = Ease.InOutSine;
    [SerializeField] private Ease _toStandard_Rot_Ease = Ease.InOutSine;
    public Ease ToStandard_Move_Ease => _toStandard_Move_Ease;
    public Ease ToStandard_Rot_Ease => _toStandard_Rot_Ease;


    [Header("Hovered")]
    [SerializeField] private Ease _hovered_Move_Ease = Ease.InOutSine;
    [SerializeField] private Ease _hovered_Rot_Ease = Ease.InOutSine;
    public Ease Hovered_Move_Ease => _hovered_Move_Ease;
    public Ease Hovered_Rot_Ease => _hovered_Rot_Ease;


    [Header("Selected")]
    [SerializeField] private Ease _selected_Move_Ease = Ease.InOutSine;
    public Ease Selected_Move_Ease => _selected_Move_Ease;


    [System.Serializable]
    public class RotationEffect
    {
        [SerializeField] private Vector2 _maxRotationAngles = Vector2.one * 5.0f;
        [SerializeField, Min(0)] private float _rotationSpeed = 1.0f;
        [SerializeField, Min(0)] private float _transitionDuration = 0.2f;
        public Vector2 MaxRotationAngles => _maxRotationAngles;
        public float RotationSpeed => _rotationSpeed;
        public float TransitionDuration => _transitionDuration;
    }


    [Space(20)]
    [Header("ROTATION EFFECT")]
    [SerializeField] private RotationEffect _idleRotationEffect;
    [SerializeField] private RotationEffect _hoveredMouseRotationEffect;
    public RotationEffect IdleRotationEffect => _idleRotationEffect;
    public RotationEffect HoveredMouseRotationEffect => _hoveredMouseRotationEffect;




    [System.Serializable]
    public class CardStateDisplacements
    {
        [SerializeField] private Vector3 _hovered;
        public Vector3 Hovered => _hovered;       
    }

    [Space(20)]
    [Header("INTERACTION DISPLACEMENTS")]
    [SerializeField] private CardStateDisplacements _gameplayHandDisplacements;
    [SerializeField] private CardStateDisplacements _upgradesDisplacements;
    [SerializeField] private CardStateDisplacements _tutorialDisplayDisplacements;
    public CardStateDisplacements CurrentDisplacements { get; private set; }


    public void SetTDGameplayHandMode()
    {
        CurrentDisplacements = _gameplayHandDisplacements;
    }
    public void SetUpgradeSceneMode()
    {
        CurrentDisplacements = _upgradesDisplacements;
    }
    public void SetTutorialDisplayMode()
    {
        CurrentDisplacements = _tutorialDisplayDisplacements;
    }

}
