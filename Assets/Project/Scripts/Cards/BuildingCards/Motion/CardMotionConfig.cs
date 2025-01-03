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
    [Header("Idle")]
    [SerializeField] private RotationEffect _defaultIdleRotationEffect;
    [SerializeField] private RotationEffect _cardCollectionIdleRotationEffect;
    [Header("Hover")]
    [SerializeField] private RotationEffect _hoveredMouseRotationEffect;
    [SerializeField] private RotationEffect _hoveredGatherNewCardRotationEffect;
    public RotationEffect IdleRotationEffect { get; private set; }
    public RotationEffect HoveredMouseRotationEffect { get; private set; }




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
    [SerializeField] private CardStateDisplacements _resultsScreenDisplacements;
    [SerializeField] private CardStateDisplacements _cardCollectionDisplacements;
    public CardStateDisplacements CurrentDisplacements { get; private set; }


    public void SetTDGameplayHandMode()
    {
        IdleRotationEffect = _defaultIdleRotationEffect;
        HoveredMouseRotationEffect = _hoveredMouseRotationEffect;
        CurrentDisplacements = _gameplayHandDisplacements;
    }
    public void SetUpgradeSceneMode()
    {
        IdleRotationEffect = _defaultIdleRotationEffect;
        HoveredMouseRotationEffect = _hoveredMouseRotationEffect;
        CurrentDisplacements = _upgradesDisplacements;
    }
    public void SetTutorialDisplayMode()
    {
        IdleRotationEffect = _defaultIdleRotationEffect;
        HoveredMouseRotationEffect = _hoveredMouseRotationEffect;
        CurrentDisplacements = _tutorialDisplayDisplacements;
    }
    public void SetResultsScreenDisplayMode()
    {
        IdleRotationEffect = _defaultIdleRotationEffect;
        HoveredMouseRotationEffect = _hoveredMouseRotationEffect;
        CurrentDisplacements = _resultsScreenDisplacements;
    }
    public void SetCardCollectionDisplayMode()
    {
        IdleRotationEffect = _cardCollectionIdleRotationEffect;
        HoveredMouseRotationEffect = _hoveredMouseRotationEffect;
        CurrentDisplacements = _cardCollectionDisplacements;
    }
    public void SetGatherCardDisplayMode()
    {
        IdleRotationEffect = _defaultIdleRotationEffect;
        HoveredMouseRotationEffect = _hoveredGatherNewCardRotationEffect;
        CurrentDisplacements = _cardCollectionDisplacements;
    }

}
