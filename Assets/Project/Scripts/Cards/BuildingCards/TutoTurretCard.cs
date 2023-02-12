using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutoTurretCard : TurretBuildingCard
{
    [Header("TUTORIAL DRAW ANIMATION")]
    [SerializeField] private CanvasGroup rangeBarCg;
    [SerializeField] private CanvasGroup cadenceBarCg;
    [SerializeField] private CanvasGroup attackBarCg;
    [SerializeField] private CanvasGroup attackIconCg;
    [SerializeField] private CanvasGroup basePassiveIconCg;

    [HideInInspector] public bool showLevel = false;
    [HideInInspector] public bool showRangeStat = false;
    [HideInInspector] public bool showCadenceStat = false;
    [HideInInspector] public bool showAttackStat = false;
    [HideInInspector] public bool showPlayCost = false;
    [HideInInspector] public bool showAttackIcon = false;
    [HideInInspector] public bool showBasePassiveIcon = false;




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


        float tBlink = drawAnimBlinkDuration / drawAnimNumBlinks;
        for (int i = 0; i < drawAnimNumBlinks; ++i)
        {
            GameAudioManager.GetInstance().PlayCardInfoMoveHidden();
            yield return new WaitForSeconds(tBlink);
        }


        float t1 = 0.1f;

        yield return new WaitUntil(() => showLevel);
        cardLevelText.DOFade(1f, t1);
        GameAudioManager.GetInstance().PlayCardInfoShown();

        yield return new WaitUntil(() => showRangeStat);
        rangeBarCg.DOFade(1f, t1);
        GameAudioManager.GetInstance().PlayCardInfoShown();

        yield return new WaitUntil(() => showCadenceStat);
        cadenceBarCg.DOFade(1f, t1);
        GameAudioManager.GetInstance().PlayCardInfoShown();

        yield return new WaitUntil(() => showAttackStat);
        attackBarCg.DOFade(1f, t1);
        GameAudioManager.GetInstance().PlayCardInfoShown();

        yield return new WaitUntil(() => showPlayCost);
        playCostText.DOFade(1f, t1);
        GameAudioManager.GetInstance().PlayCardInfoShown();

        yield return new WaitUntil(() => showAttackIcon);
        attackIconCg.DOFade(1f, t1);
        GameAudioManager.GetInstance().PlayCardInfoShown();

        yield return new WaitUntil(() => showBasePassiveIcon);
        basePassiveIconCg.DOFade(1f, t1);
        GameAudioManager.GetInstance().PlayCardInfoShown();



        canInfoInteract = true;
        isPlayingDrawAnimation = false;
        cardMaterial.SetFloat("_BorderLoopEnabled", 0f);


        animationEndCallback();
    }
}
