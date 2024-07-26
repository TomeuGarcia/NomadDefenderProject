using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutoTurretCard : TurretBuildingCard
{
    [Header("TUTORIAL DRAW ANIMATION")]
    [SerializeField] private CanvasGroup cardLevelTextCg;
    [SerializeField] private CanvasGroup baseAndBodyCg;
    [SerializeField] private CanvasGroup playCostTextCg;
    [SerializeField] private CanvasGroup rangeBarCg;
    [SerializeField] private CanvasGroup cadenceBarCg;
    [SerializeField] private CanvasGroup attackBarCg;
    [SerializeField] private CanvasGroup attackIconCg;
    [SerializeField] private CanvasGroup basePassiveIconCg;


    [HideInInspector] public bool showTurret = false;
    //[HideInInspector] public bool showLevel = false;
    [HideInInspector] public bool showRangeStat = false;
    [HideInInspector] public bool showCadenceStat = false;
    [HideInInspector] public bool showAttackStat = false;
    [HideInInspector] public bool showPlayCost = false;
    //[HideInInspector] public bool showAttackIcon = false;
    //[HideInInspector] public bool showBasePassiveIcon = false;

    private delegate void PlayAudioFunction();

    private float _displacementInDuration = 0.5f;
    private float _displacementOutDuration = 0.75f;
    private Vector3 _positionDisplacement = new Vector3(0f, 1f, 0f);
    private Vector3 _scaleDisplacement = Vector3.one * 1.5f;

    public bool FinishedAnimation => !isPlayingDrawAnimation;

    [SerializeField] private Transform _animationTransform;



    protected override IEnumerator InterfaceDrawAnimation(CardFunctionPtr animationEndCallback)
    {

        canInfoInteract = false;
        isPlayingDrawAnimation = true;
        

        for (int i = 0; i < cgsInfoHide.Length; ++i)
        {
            cgsInfoHide[i].alpha = 0f;
        }
        for (int i = 0; i < otherCfDrawAnimation.Length; ++i)
        {
            otherCfDrawAnimation[i].alpha = 0f;
        }

        DisableMouseInteraction();



        float startStep = 10f;
        float step = startStep;
        float stepDec = startStep * 0.02f;
        for (float t = 0f; t < drawAnimLoopDuration; t += Time.deltaTime * step)
        {
            GameAudioManager.GetInstance().PlayCardInfoMoveShown();
            yield return new WaitForSeconds(Time.deltaTime * step);

            step -= stepDec;
        }


        yield return new WaitForSeconds(drawAnimWaitDurationBeforeBlink);
        transform.DOComplete();
        transform.DOBlendableMoveBy(_positionDisplacement, _displacementInDuration).SetEase(Ease.InOutSine);
        transform.DOScale(_scaleDisplacement, _displacementInDuration).SetEase(Ease.InOutSine);

        float tBlink = drawAnimBlinkDuration / drawAnimNumBlinks;
        for (int i = 0; i < drawAnimNumBlinks; ++i)
        {
            GameAudioManager.GetInstance().PlayCardInfoMoveHidden();
            yield return new WaitForSeconds(tBlink);
        }


        yield return new WaitForSeconds(0.4f);


        float t1 = 0.065f;


        // Show Turret Body and Base
        yield return new WaitUntil(() => showTurret);
        CanvasGroup[] turretCgs = { baseAndBodyCg, attackIconCg, basePassiveIconCg, cardLevelTextCg };
        yield return StartCoroutine(ShowCanvasGroupElements(turretCgs, t1, GameAudioManager.GetInstance().PlayCardInfoMoveShown));

        //// Show Level
        //yield return new WaitUntil(() => showLevel);
        //yield return StartCoroutine(ShowCanvasGroupElement(cardLevelTextCg, t1, GameAudioManager.GetInstance().PlayCardInfoShown));

        // Show Play Cost
        yield return new WaitUntil(() => showPlayCost);
        yield return StartCoroutine(ShowCanvasGroupElement(playCostTextCg, t1, GameAudioManager.GetInstance().PlayCardInfoShown));

        // Show Attack Stat
        yield return new WaitUntil(() => showAttackStat);
        yield return StartCoroutine(ShowCanvasGroupElement(attackBarCg, t1, GameAudioManager.GetInstance().PlayCardInfoShown));

        // Show Cadence Stat
        yield return new WaitUntil(() => showCadenceStat);
        yield return StartCoroutine(ShowCanvasGroupElement(cadenceBarCg, t1, GameAudioManager.GetInstance().PlayCardInfoShown));

        // Show Range Stat
        yield return new WaitUntil(() => showRangeStat);
        yield return StartCoroutine(ShowCanvasGroupElement(rangeBarCg, t1, GameAudioManager.GetInstance().PlayCardInfoShown));

        //// Show Attack Icon
        //yield return new WaitUntil(() => showAttackIcon);
        //yield return StartCoroutine(ShowCanvasGroupElement(attackIconCg, t1, GameAudioManager.GetInstance().PlayCardInfoShown));

        //// Show base Passive Icon
        //yield return new WaitUntil(() => showBasePassiveIcon);
        //yield return StartCoroutine(ShowCanvasGroupElement(basePassiveIconCg, t1, GameAudioManager.GetInstance().PlayCardInfoShown));


        yield return new WaitForSeconds(0.5f);


        DisableMouseInteraction();
        MotionEffectsController.DisableMotion();
        transform.DOComplete();
        transform.DOBlendableMoveBy(-_positionDisplacement, _displacementOutDuration)
            .SetEase(Ease.InOutSine);
        transform.DOScale(Vector3.one, _displacementOutDuration)
            .SetEase(Ease.InOutSine);

        Quaternion startRotation = transform.localRotation;
        float spinRotation = 0f;
        DOTween.To(
            () => spinRotation,
            (spinRotationValue) => {
                spinRotation = spinRotationValue;
                _animationTransform.localRotation = startRotation * Quaternion.Euler(0, spinRotation, 0); 
            },
            360f,
            _displacementOutDuration
            )
            .SetEase(Ease.InOutQuad);

        yield return new WaitForSeconds(_displacementOutDuration + 0.25f);

        EnableMouseInteraction();
        isInteractable = true;
        MotionEffectsController.EnableMotion();
        

        canInfoInteract = true;
        canBeHovered = true;
        isPlayingDrawAnimation = false;
        cardMaterial.SetFloat("_BorderLoopEnabled", 0f);


        animationEndCallback();
    }

    private IEnumerator ShowCanvasGroupElement(CanvasGroup cgElement, float t1, PlayAudioFunction playAudioFunction)
    {
        cgElement.DOFade(1f, t1);
        playAudioFunction();
        yield return new WaitForSeconds(t1);

        cgElement.DOFade(0f, t1);
        yield return new WaitForSeconds(t1);

        cgElement.DOFade(1f, t1);
        playAudioFunction();
        yield return new WaitForSeconds(t1);
    }

    private IEnumerator ShowCanvasGroupElements(CanvasGroup[] cgElements, float t1, PlayAudioFunction playAudioFunction)
    {
        foreach (CanvasGroup cgElement in cgElements)
        {
            cgElement.DOFade(1f, t1);
        }
        playAudioFunction();
        yield return new WaitForSeconds(t1);

        foreach (CanvasGroup cgElement in cgElements)
        {
            cgElement.DOFade(0f, t1);
        }
        yield return new WaitForSeconds(t1);

        foreach (CanvasGroup cgElement in cgElements)
        {
            cgElement.DOFade(1f, t1);
        }
        playAudioFunction();
        yield return new WaitForSeconds(t1);
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
