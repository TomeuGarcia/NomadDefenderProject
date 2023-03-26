using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OWMap_UpgradeNodeInfoDisplay : OWMap_NodeInfoDisplay
{

    [SerializeField] private TextMeshProUGUI nodeTitleText;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI rewardsText;

    private static string[] upgardeTypesToText =
    {
        "newCard", "projectile", "body", "base"
    };

    private static string[] nodeHealthToText = { "<color=#1DFF5F>INTACT</color>",
                                                 "<color=#FFF345>PLUNDERED</color>",
                                                 "<color=#F5550C>RUINED</color>",
                                                 "<color=#FF003E>DESTROYED</color>" };
    private static string[] nodeHealthToRewardText = { "<color=#1DFF5F>GREAT</color>",
                                                       "<color=#FFF345>MEDIUM</color>",
                                                       "<color=#F5550C>POOR</color>",
                                                       "<color=#FF003E>ERROR</color>" };
    private const string perfectNodeHealthText = "<color=#439bee>PERFECT</color>";
    private const string perfectRewardText = "<color=#439bee>GREAT+</color>";

    private const string statusStr = "\\status: ";
    private const string rewardsStr = "\\rewards: ";
    private const string unknownStr = "<color=#8C8C8C>???</color>";


    private void Awake()
    {
        infoCanvasTransform.gameObject.SetActive(false);

        statusText.text = statusStr + unknownStr;
        rewardsText.text = rewardsStr + unknownStr;
    }


    public void Init(OWMap_Node attachedNode, MouseOverNotifier mouseOverNotifier, bool positionedAtRight)
    {
        BaseInit(attachedNode, mouseOverNotifier, positionedAtRight);
    }
    public void InitUpgradeType(NodeEnums.UpgradeType upgradeType)
    {
        nodeTitleText.text = ">UPG-" + upgardeTypesToText[(int)upgradeType];
    }

    protected override void SubscriveToAttachedNodeEvents()
    {
        base.SubscriveToAttachedNodeEvents();
        this.attachedNode.OnNodeHealthStateSet += SetupTexts;
    }
    protected override void UnsubscriveToAttachedNodeEvents()
    {
        base.UnsubscriveToAttachedNodeEvents();
        this.attachedNode.OnNodeHealthStateSet -= SetupTexts;
    }


    // Functionality
    private void SetupTexts(NodeEnums.HealthState nodeHealth, bool wonWithPerfectDefense)
    {
        if (wonWithPerfectDefense)
        {
            statusText.text = statusStr + perfectNodeHealthText;
            rewardsText.text = rewardsStr + perfectRewardText;
        }
        else
        {
            statusText.text = statusStr + nodeHealthToText[(int)nodeHealth];
            rewardsText.text = rewardsStr + nodeHealthToRewardText[(int)nodeHealth];
        }
    }


    protected override IEnumerator ShowNodeInfoAnimation()
    {
        transform.DOComplete();
        barImage.DOComplete();
        backgroundImage.DOComplete();
        nodeTitleText.DOComplete();
        statusText.DOComplete();
        rewardsText.DOComplete();

        float t0 = 0.001f;
        float t1 = 0.1f;
        float t2 = 0.2f;

        infoCanvasTransform.gameObject.SetActive(true);
        barImage.fillAmount = 0f;
        backgroundImage.fillAmount = 0f;
        nodeTitleText.DOFade(0f, t0);
        statusText.DOFade(0f, t0);
        rewardsText.DOFade(0f, t0);
        yield return new WaitForSeconds(t0);


        transform.DORotate(Vector3.right * -25f, 0.75f);

        barImage.DOFillAmount(1f, t2);
        GameAudioManager.GetInstance().PlayCardInfoMoveShown();
        yield return new WaitForSeconds(t1);
        

        backgroundImage.DOFillAmount(1f, t2);
        nodeTitleText.DOFade(1f, t1);
        yield return new WaitForSeconds(t1);


        yield return new WaitForSeconds(t1);


        statusText.DOFade(1f, t1);
        yield return new WaitForSeconds(t1);


        rewardsText.DOFade(1f, t1);
        yield return new WaitForSeconds(t1);

        showInfoCoroutine = null;
    }

    protected override IEnumerator HideNodeInfoAnimation()
    {
        transform.DOComplete();
        barImage.DOComplete();
        backgroundImage.DOComplete();
        nodeTitleText.DOComplete();
        statusText.DOComplete();
        rewardsText.DOComplete();

        float t1 = 0.1f;
        float t2 = 0.2f;

        transform.DORotate(Vector3.zero, 1f);

        rewardsText.DOFade(0f, t1);
        //GameAudioManager.GetInstance().PlayCardInfoHidden();
        yield return new WaitForSeconds(t1);
        //GameAudioManager.GetInstance().PlayCardInfoHidden();

        statusText.DOFade(0f, t1);
        yield return new WaitForSeconds(t1);

        backgroundImage.DOFillAmount(0f, t2);
        nodeTitleText.DOFade(0f, t1);
        yield return new WaitForSeconds(t1);

        barImage.DOFillAmount(0f, t1);
        //GameAudioManager.GetInstance().PlayCardInfoMoveHidden();
        yield return new WaitForSeconds(t1);

        infoCanvasTransform.gameObject.SetActive(false);

        hideInfoCoroutine = null;
    }

}
