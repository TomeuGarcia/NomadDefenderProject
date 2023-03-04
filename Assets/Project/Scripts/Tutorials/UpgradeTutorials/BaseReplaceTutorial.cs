using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseReplaceTutorial : MonoBehaviour
{
    [SerializeField] private CardPartReplaceManager cardPartReplaceManager;
    [SerializeField] private ScriptedSequence scriptedSequence;
    private Tutorials tutoType;

    [SerializeField] PartsLibrary.BaseAndPassive[] tutorialBaseAndPassive;


    private void Awake()
    {
        tutoType = Tutorials.BASE_FUSION_UPG;
        cardPartReplaceManager.SetPartsCreatedByTutorialTrue();
    }

    void Start()
    {
        cardPartReplaceManager.InitTutorialBases(tutorialBaseAndPassive);
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


        //Calibrating base replacement upgrade…
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


        //Loading new Clean Strike base card…
        //[Clean Strike PART SHOWS UP]
        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(0.5f);
        cardPartReplaceManager.CardPartHolder.appearAnimationCanStartMoving = true;
        yield return new WaitForSeconds(1.5f);

        ///Info> Base cards contain the <color=#1DFF5F>RANGE</color> stat 
        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        cardPartReplaceManager.CardPartHolder.PlayBaseTutorialBlinkAnimation();
        yield return new WaitForSeconds(2.0f);

        ///Info> Base cards can have a <color=#E36818>PASSIVE ABILITY</color>
        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(2.0f);


        //Instruction> Effectuate replacement
        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        
        cardPartReplaceManager.CardPartHolder.canSelectCard = true;
        cardPartReplaceManager.UpgradeCardHolder.canSelectCard = true;
        yield return new WaitUntil(() => cardPartReplaceManager.ReplacementDone);
        bool cardLevelWasMax = cardPartReplaceManager.UpgradeCardHolder.selectedCard.GetComponent<TurretBuildingCard>().IsCardLevelMaxed();


        //Successful replacement status.
        //[WAIT 1 SECOND THEN CLEAR]
        scriptedSequence.NextLine();
        yield return new WaitForSeconds(1.5f);
        scriptedSequence.Clear();


        // CEHCK IF LEVEL WAS ALREADY MAXED        
        if (cardLevelWasMax)
        {
            //Card already max level
            //[CARD IS RETRIEVED BUT STAYS]
            //[WAIT 1 SECOND]
            scriptedSequence.SkipLine();
            scriptedSequence.NextLine();
            yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
            yield return new WaitForSeconds(1.5f);
            scriptedSequence.Clear();
        }
        else
        {
            //Successful card level up
            //[CARD IS RETRIEVED BUT STAYS]
            //[WAIT 1 SECOND]
            scriptedSequence.NextLine();
            yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
            yield return new WaitForSeconds(1.5f);
            scriptedSequence.Clear();
            scriptedSequence.SkipLine();
        }


        //Upgrade completed
        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(1.5f);
        scriptedSequence.Clear();

        cardPartReplaceManager.canFinalRetrieveCard = true;

        TutorialsSaverLoader.GetInstance().SetTutorialDone(tutoType);
    }
}
