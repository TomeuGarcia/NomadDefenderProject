using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OWMap_BattleNodeInfoDisplay : OWMap_NodeInfoDisplay
{
    [SerializeField] private TextMeshProUGUI nodeTitleText;
    [SerializeField] private TextMeshProUGUI difficultyText;

    private static string[] nodeDifficultyToText = { "<color=#1DFF5F>EARLY</color>",
                                                 "<color=#FFF345>MID</color>",
                                                 "<color=#F5550C>LATE</color>",
                                                 "<color=#FF003E>BOSS</color>" };

    private const string difficultyStr = "stage: ";
    private const string unknownStr = "???";

    private int nextLevelNodes;


    private void Awake()
    {
        infoCanvasTransform.gameObject.SetActive(false);

        difficultyText.text = difficultyStr + unknownStr;
    }

    public void Init(OWMap_Node attachedNode, MouseOverNotifier mouseOverNotifier, bool positionedAtRight, NodeEnums.BattleType battleType, int newNextLevelNodes)
    {
        BaseInit(attachedNode, mouseOverNotifier, positionedAtRight);
        nextLevelNodes = newNextLevelNodes;
        SetupTexts(battleType);
    }

    protected override void SubscriveToAttachedNodeEvents()
    {
        base.SubscriveToAttachedNodeEvents();
        //this.attachedNode.OnNodeHealthStateSet += SetupTexts;
    }
    protected override void UnsubscriveToAttachedNodeEvents()
    {
        base.UnsubscriveToAttachedNodeEvents();
        //this.attachedNode.OnNodeHealthStateSet -= SetupTexts;
    }


    // Functionality
    private void SetupTexts(NodeEnums.BattleType battleType)
    {
        nodeTitleText.text = "\\BATTLE> " + nextLevelNodes;
        nodeTitleText.text += (nextLevelNodes > 1 ? " nodes" : " node");

        difficultyText.text = difficultyStr + nodeDifficultyToText[(int)battleType];
    }


    protected override IEnumerator ShowNodeInfoAnimation()
    {
        transform.DOComplete();
        barImage.DOComplete();
        backgroundImage.DOComplete();
        nodeTitleText.DOComplete();
        difficultyText.DOComplete();

        float t0 = 0.001f;
        float t1 = 0.1f;
        float t2 = 0.2f;

        infoCanvasTransform.gameObject.SetActive(true);
        barImage.fillAmount = 0f;
        backgroundImage.fillAmount = 0f;
        nodeTitleText.DOFade(0f, t0);
        difficultyText.DOFade(0f, t0);
        yield return new WaitForSeconds(t0);


        transform.DORotate(Vector3.right * -25f, 0.75f);

        barImage.DOFillAmount(1f, t2);
        GameAudioManager.GetInstance().PlayCardInfoMoveShown();
        yield return new WaitForSeconds(t1);
        backgroundImage.DOFillAmount(1f, t2);

        nodeTitleText.DOFade(1f, t1);
        yield return new WaitForSeconds(t1);


        yield return new WaitForSeconds(t1);


        difficultyText.DOFade(1f, t1);
        yield return new WaitForSeconds(t1);


        showInfoCoroutine = null;
    }

    protected override IEnumerator HideNodeInfoAnimation()
    {
        transform.DOComplete();
        barImage.DOComplete();
        backgroundImage.DOComplete();
        nodeTitleText.DOComplete();
        difficultyText.DOComplete();

        float t1 = 0.1f;
        float t2 = 0.2f;

        transform.DORotate(Vector3.zero, 1f);


        difficultyText.DOFade(0f, t1);
        yield return new WaitForSeconds(t1);

        backgroundImage.DOFillAmount(0f, t2);
        nodeTitleText.DOFade(0f, t1);
        yield return new WaitForSeconds(t1);

        barImage.DOFillAmount(0f, t1);
        yield return new WaitForSeconds(t1);

        infoCanvasTransform.gameObject.SetActive(false);

        hideInfoCoroutine = null;
    }
}
