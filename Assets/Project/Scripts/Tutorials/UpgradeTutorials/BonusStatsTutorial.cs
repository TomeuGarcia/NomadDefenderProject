using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusStatsTutorial : MonoBehaviour
{
    [SerializeField] private CardPartReplaceManager cardPartReplaceManager;
    [SerializeField] private ScriptedSequence scriptedSequence;
    private Tutorials tutoType;

    [SerializeField] TurretStatsUpgradeModel[] tutorialBonusStats;


    private void Awake()
    {
        tutoType = Tutorials.BONUS_STATS;
        cardPartReplaceManager.AwakeSetupTutorialStatBonuses(tutorialBonusStats);
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


        //Calibrating BONUS STATS upgrade...
        //[CLEAR]
        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(1.5f);

        cardPartReplaceManager.UpgradeCardHolder.appearAnimationCanStartMoving = true;
        yield return new WaitForSeconds(1.5f);
        scriptedSequence.Clear();

        //Loading new <color=#E36818>Bonus</color> stats card...
        //[BONUS STATS CARD SHOWS UP]
        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(0.5f);
        cardPartReplaceManager.CardPartHolder.appearAnimationCanStartMoving = true;
        yield return new WaitForSeconds(1.5f);

        /*//Loading Body card stats: <color=#FF003E>ATTACK</color> and <color=#FFF345>CADENCY</color>
        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        cardPartReplaceManager.CardPartHolder.PlayBodyTutorialBlinkAnimation();
        yield return new WaitForSeconds(2.0f);
        scriptedSequence.Clear();*/

        //Stats will permanently be added to your card
        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(1.0f);
        
        //Instruction> Effectuate replacement
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
        yield return new WaitForSeconds(1.0f);
        scriptedSequence.Clear();


        //Successful card level up.
        //[CARD IS RETRIEVED BUT STAYS]
        //[WAIT 1 SECOND]
        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(1.0f);
        scriptedSequence.Clear();


        //Upgrade completed
        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(1.0f);
        scriptedSequence.Clear();

        cardPartReplaceManager.canFinalRetrieveCard = true;

        TutorialsSaverLoader.GetInstance().SetTutorialDone(tutoType);
    }

}
