using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TutoTurretCard : TurretBuildingCard
{
    [Header("TUTORIAL DRAW ANIMATION")]
    [SerializeField] private CardDrawAnimationPlayer_TutorialCardOverview _tutorialCardDrawAnimation;
    [SerializeField] private Transform _animationTransform;


    public bool ShowTurret { 
        set => _tutorialCardDrawAnimation.ShowTurret = value;
    }
    public bool ShowProjectile { 
        set => _tutorialCardDrawAnimation.ShowProjectile = value;
    }
    public bool ShowPlayCost { 
        set => _tutorialCardDrawAnimation.ShowPlayCost = value;
    }
    public bool ShowDamageStat { 
        set => _tutorialCardDrawAnimation.ShowDamageStat = value;
    }
    public bool ShowRangeStat { 
        set => _tutorialCardDrawAnimation.ShowRangeStat = value;
    }
    public bool ShowShotsPerSecondStat { 
        set => _tutorialCardDrawAnimation.ShowShotsPerSecondStat = value;
    }
    public bool Finish { 
        set => _tutorialCardDrawAnimation.Finish = value;
    }


    
    private float _displacementInDuration = 0.5f;
    private float _displacementOutDuration = 0.75f;
    private Vector3 _positionDisplacement = new Vector3(0f, 1f, 0f);
    private Vector3 _scaleDisplacement = Vector3.one * 1.5f;

    public bool FinishedAnimation => !isPlayingDrawAnimation;


    private CardOverviewPositioner _overviewPositioner;

    public void InitOverviewPositioner(CardOverviewPositioner overviewPositioner)
    {
        _overviewPositioner = overviewPositioner;
    }

    protected override IEnumerator InterfaceDrawAnimation(CardFunctionPtr animationEndCallback)
    {

        canInfoInteract = false;
        isPlayingDrawAnimation = true;
        DisableMouseInteraction();

        Coroutine drawAnimation = StartCoroutine(_tutorialCardDrawAnimation.PlayDrawAnimation());

        yield return new WaitForSeconds(drawAnimWaitDurationBeforeBlink);
        yield return new WaitForSeconds(0.5f);
        
        _overviewPositioner.Init(this);
        yield return StartCoroutine(_overviewPositioner.PositionToSpot());
        /*
        transform.DOComplete();
        transform.DOBlendableMoveBy(_positionDisplacement, _displacementInDuration).SetEase(Ease.InOutSine);
        transform.DOScale(_scaleDisplacement, _displacementInDuration).SetEase(Ease.InOutSine);
        
        yield return new WaitForSeconds(0.4f);
        */
        _tutorialCardDrawAnimation.CanStartShowing = true;
        
        yield return drawAnimation;
        
        
        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(_overviewPositioner.UndoPositioning());
        /*
        DisableMouseInteraction();
        MotionEffectsController.DisableMotion();
        transform.DOComplete();
        transform.DOBlendableMoveBy(-_positionDisplacement, _displacementOutDuration)
            .SetEase(Ease.InOutSine);
        transform.DOScale(Vector3.one, _displacementOutDuration)
            .SetEase(Ease.InOutSine);

        _animationTransform.DOBlendableLocalRotateBy(new Vector3(0, 360f, 0), _displacementOutDuration,
            RotateMode.LocalAxisAdd);
        
        yield return new WaitForSeconds(_displacementOutDuration + 0.25f);

        EnableMouseInteraction();
        MotionEffectsController.EnableMotion();
        isInteractable = true;
        */
        
        

        canInfoInteract = true;
        canBeHovered = true;
        isPlayingDrawAnimation = false;
        cardMaterial.SetFloat("_BorderLoopEnabled", 0f);


        animationEndCallback();
    }

    



    public override void ShowInfo()
    {
        if (!FinishedAnimation)
        {
            return;
        }

        base.ShowInfo();
    }

    public override void HideInfo()
    {
        if (!FinishedAnimation)
        {
            return;
        }

        base.HideInfo();
    }
}
