using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyReplaceTutorial : MonoBehaviour
{
    [SerializeField] private CardPartReplaceManager cardPartReplaceManager;
    [SerializeField] private ScriptedSequence scriptedSequence;
    private Tutorials tutoType;

    [SerializeField] TurretPartBody[] tutorialBodies;


    private void Awake()
    {
        tutoType = Tutorials.BODY_FUSION_UPG;

        cardPartReplaceManager.InitTutorialBodies(tutorialBodies);
    }

    void Start()
    {
        if (!TutorialsSaverLoader.GetInstance().IsTutorialDone(tutoType))
        {
            StartCoroutine(Tutorial());

            cardPartReplaceManager.UpgradeCardHolder.appearAnimationCanStartMoving = false;
            cardPartReplaceManager.CardPartHolder.appearAnimationCanStartMoving = false;

            cardPartReplaceManager.CardPartHolder.canSelectCard = false;
            cardPartReplaceManager.UpgradeCardHolder.canSelectCard = false;

            cardPartReplaceManager.canFinalRetrieveCard = false;
        }
    }




    IEnumerator Tutorial()
    {
        yield return new WaitForSeconds(1.0f);


        //Calibrating body replacement upgrade…
        //[CLEAR]
        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(2.0f);
        scriptedSequence.Clear();


        //Loading deck cards…
        //[CARDS SHOW UP]
        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(0.5f);
        cardPartReplaceManager.UpgradeCardHolder.appearAnimationCanStartMoving = true;
        yield return new WaitForSeconds(1.5f);


        //Loading Spammer body card…
        //[SPAMMER PART SHOWS UP]
        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(0.5f);
        cardPartReplaceManager.CardPartHolder.appearAnimationCanStartMoving = true;
        yield return new WaitForSeconds(1.5f);

        // Loading Body card stats: <color=#FF003E>ATTACK</color> and <color=#FFF345>CADENCY</color>
        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        cardPartReplaceManager.CardPartHolder.PlayBodyTutorialBlinkAnimation();
        yield return new WaitForSeconds(2.0f);


        //Instruction> Effectuate replacement
        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(1.0f);

        //Card stats will be<color=#FF003E>OVERWRITTEN</color>
        //[WAIT FOR PLAYER SELECT BOTH CARD AND PART]
        //[CLEAR]
        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        cardPartReplaceManager.CardPartHolder.canSelectCard = true;
        cardPartReplaceManager.UpgradeCardHolder.canSelectCard = true;

        yield return new WaitUntil(() => cardPartReplaceManager.ReplacementDone);
        yield return new WaitForSeconds(0.5f);
        scriptedSequence.Clear();


        //Successful replacement status.
        //[WAIT 1 SECOND THEN CLEAR]
        scriptedSequence.NextLine();
        yield return new WaitForSeconds(1.5f);
        scriptedSequence.Clear();


        //Successful card level up.
        //[CARD IS RETRIEVED BUT STAYS]
        //[WAIT 1 SECOND]
        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(1.5f);
        scriptedSequence.Clear();


        //Upgrade completed
        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(1.5f);
        scriptedSequence.Clear();

        cardPartReplaceManager.canFinalRetrieveCard = true;

        TutorialsSaverLoader.GetInstance().SetTutorialDone(tutoType);
    }

}
