using System.Collections;
using UnityEngine;

public class AttackReplaceTutorial : MonoBehaviour
{
    [SerializeField] private CardPartReplaceManager cardPartReplaceManager;
    [SerializeField] private ScriptedSequence scriptedSequence;
    private Tutorials tutoType;
    private bool cardShowedInfo;
    private bool buttonPressed;

    [SerializeField] TurretPartAttack[] tutorialAttacks;


    private void Awake()
    {
        tutoType = Tutorials.PROJECTILE_FUSION_UPG;

        cardPartReplaceManager.InitTutorialAttacks(tutorialAttacks);
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

            cardShowedInfo = false;
            CardPart.OnInfoShown += SetCardShowedInfoTrue;
            BuildingCard.OnInfoShown += SetCardShowedInfoTrue;
        }
    }

    private void SetCardShowedInfoTrue()
    {
        CardPart.OnInfoShown -= SetCardShowedInfoTrue;
        BuildingCard.OnInfoShown -= SetCardShowedInfoTrue;
        cardShowedInfo = true;
    }



    IEnumerator Tutorial()
    {
        yield return new WaitForSeconds(1.0f);


        //Calibrating turret projectile replacement upgrade…
        //[CLEAR]
        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(1.0f);
        scriptedSequence.Clear();


        //Loading deck cards…
        //[CARDS SHOW UP]
        //[CLEAR]
        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(0.5f);
        cardPartReplaceManager.UpgradeCardHolder.appearAnimationCanStartMoving = true;
        yield return new WaitForSeconds(2.0f);
        scriptedSequence.Clear();


        //Loading High Voltage projectile card…
        //[HIGH VOLTAGE CARD PART SHOWS UP]
        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(0.5f);
        cardPartReplaceManager.CardPartHolder.appearAnimationCanStartMoving = true;
        yield return new WaitForSeconds(2.0f);
        scriptedSequence.Clear();


        //Display/Conceal a card’s ability using <color=#47B13A>Input</color>.<color=#F5550C>RightClick</color>
        //[WAIT FOR PLAYER TO VIEW ABILITY]
        //[CLEAR]
        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitUntil(() => cardShowedInfo);
        yield return new WaitForSeconds(1.0f);
        scriptedSequence.Clear();
        cardPartReplaceManager.CardPartHolder.canSelectCard = true;
        cardPartReplaceManager.UpgradeCardHolder.canSelectCard = true;


        //Instruction> Effectuate replacement /REPLACE
        //[WAIT FOR PLAYER TO PRESS REPLACE BUTTON]
        //[CLEAR]
        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitUntil(() => cardPartReplaceManager.ReplacementDone);
        yield return new WaitForSeconds(1.0f);
        scriptedSequence.Clear();


        //Successful replacement status.
        //[WAIT 1 SECOND THEN CLEAR]
        scriptedSequence.NextLine();
        yield return new WaitForSeconds(2.0f);
        scriptedSequence.Clear();


        //Successful card level up.
        //[CARD IS RETRIEVED BUT STAYS]
        //[WAIT 1 SECOND]
        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(2.0f);

        //Card now supports 2 battle upgrades.
        //[WAIT X SECOND]
        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(3.0f);
        scriptedSequence.Clear();

        scriptedSequence.NextLine();
        yield return new WaitUntil(() => scriptedSequence.IsLinePrinted());
        yield return new WaitForSeconds(0.3f);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        yield return new WaitForSeconds(0.2f);
        scriptedSequence.Clear();

        cardPartReplaceManager.canFinalRetrieveCard = true;

        TutorialsSaverLoader.GetInstance().SetTutorialDone(tutoType);
    }
}
