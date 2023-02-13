using DG.Tweening;
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


        yield return new WaitForSeconds(0.4f);


        float t1 = 0.065f;


        // Show Turret Body and Base
        baseAndBodyCg.DOFade(1f, t1);
        GameAudioManager.GetInstance().PlayCardInfoMoveShown();
        yield return new WaitForSeconds(t1);

        baseAndBodyCg.DOFade(0f, t1);
        yield return new WaitForSeconds(t1);

        baseAndBodyCg.DOFade(1f, t1);
        GameAudioManager.GetInstance().PlayCardInfoMoveShown();
        yield return new WaitForSeconds(t1);



        // Show Level
        yield return new WaitUntil(() => showLevel);

        cardLevelTextCg.DOFade(1f, t1);
        GameAudioManager.GetInstance().PlayCardInfoShown();
        yield return new WaitForSeconds(t1);

        cardLevelTextCg.DOFade(0f, t1);
        yield return new WaitForSeconds(t1);

        cardLevelTextCg.DOFade(1f, t1);
        GameAudioManager.GetInstance().PlayCardInfoShown();
        yield return new WaitForSeconds(t1);


        // Show Range Stat
        yield return new WaitUntil(() => showRangeStat);

        rangeBarCg.DOFade(1f, t1);
        GameAudioManager.GetInstance().PlayCardInfoShown();
        yield return new WaitForSeconds(t1);

        rangeBarCg.DOFade(0f, t1);
        yield return new WaitForSeconds(t1);

        rangeBarCg.DOFade(1f, t1);
        GameAudioManager.GetInstance().PlayCardInfoShown();
        yield return new WaitForSeconds(t1);


        // Show Cadence Stat
        yield return new WaitUntil(() => showCadenceStat);

        cadenceBarCg.DOFade(1f, t1);
        GameAudioManager.GetInstance().PlayCardInfoShown();
        yield return new WaitForSeconds(t1);

        cadenceBarCg.DOFade(0f, t1);
        yield return new WaitForSeconds(t1);

        cadenceBarCg.DOFade(1f, t1);
        GameAudioManager.GetInstance().PlayCardInfoShown();
        yield return new WaitForSeconds(t1);


        // Show Attack Stat
        yield return new WaitUntil(() => showAttackStat);

        attackBarCg.DOFade(1f, t1);
        GameAudioManager.GetInstance().PlayCardInfoShown();
        yield return new WaitForSeconds(t1);

        attackBarCg.DOFade(0f, t1);
        yield return new WaitForSeconds(t1);

        attackBarCg.DOFade(1f, t1);
        GameAudioManager.GetInstance().PlayCardInfoShown();
        yield return new WaitForSeconds(t1);



        // Show Play Cost
        yield return new WaitUntil(() => showPlayCost);

        playCostTextCg.DOFade(1f, t1);
        GameAudioManager.GetInstance().PlayCardInfoShown();
        yield return new WaitForSeconds(t1);

        playCostTextCg.DOFade(0f, t1);
        yield return new WaitForSeconds(t1);

        playCostTextCg.DOFade(1f, t1);
        GameAudioManager.GetInstance().PlayCardInfoShown();
        yield return new WaitForSeconds(t1);


        // Show Attack Icon
        yield return new WaitUntil(() => showAttackIcon);

        attackIconCg.DOFade(1f, t1);
        GameAudioManager.GetInstance().PlayCardInfoShown();
        yield return new WaitForSeconds(t1);

        attackIconCg.DOFade(0f, t1);
        yield return new WaitForSeconds(t1);

        attackIconCg.DOFade(1f, t1);
        GameAudioManager.GetInstance().PlayCardInfoShown();
        yield return new WaitForSeconds(t1);


        // Show base Passive Icon
        yield return new WaitUntil(() => showBasePassiveIcon);
        
        basePassiveIconCg.DOFade(1f, t1);
        GameAudioManager.GetInstance().PlayCardInfoShown();
        yield return new WaitForSeconds(t1);

        basePassiveIconCg.DOFade(0f, t1);
        yield return new WaitForSeconds(t1);

        basePassiveIconCg.DOFade(1f, t1);
        GameAudioManager.GetInstance().PlayCardInfoShown();
        yield return new WaitForSeconds(t1);



        canInfoInteract = true;
        isPlayingDrawAnimation = false;
        cardMaterial.SetFloat("_BorderLoopEnabled", 0f);


        animationEndCallback();
    }
}
