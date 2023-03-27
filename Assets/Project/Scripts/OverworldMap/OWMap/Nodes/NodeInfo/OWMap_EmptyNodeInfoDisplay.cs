using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OWMap_EmptyNodeInfoDisplay : OWMap_NodeInfoDisplay
{

    [SerializeField] private TextMeshProUGUI nodeTitleText;


    private const string startNodeStr = ">ORIGIN";
    private const string endNodeStr = ">ENDPOINT";
    private const string unknownStr = ">???";


    private void Awake()
    {
        infoCanvasTransform.gameObject.SetActive(false);

        nodeTitleText.text = startNodeStr;
    }

    public void Init(OWMap_Node attachedNode, MouseOverNotifier mouseOverNotifier, bool positionedAtRight, NodeEnums.EmptyType battleType)
    {
        BaseInit(attachedNode, mouseOverNotifier, positionedAtRight);
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
    private void SetupTexts(NodeEnums.EmptyType battleType)
    {
        if (battleType == NodeEnums.EmptyType.FIRST_LEVEL)
        {
            nodeTitleText.text = startNodeStr;
        }
        else if (battleType == NodeEnums.EmptyType.LAST_LEVEL)
        {
            nodeTitleText.text = endNodeStr;
        }
        else
        {
            nodeTitleText.text = unknownStr;
        }
    }


    protected override IEnumerator ShowNodeInfoAnimation()
    {
        transform.DOComplete();
        barImage.DOComplete();
        backgroundImage.DOComplete();
        nodeTitleText.DOComplete();

        float t0 = 0.001f;
        float t1 = 0.1f;
        float t2 = 0.2f;

        infoCanvasTransform.gameObject.SetActive(true);
        barImage.fillAmount = 0f;
        backgroundImage.fillAmount = 0f;
        nodeTitleText.DOFade(0f, t0);
        yield return new WaitForSeconds(t0);


        transform.DORotate(Vector3.right * -25f, 0.75f);

        barImage.DOFillAmount(1f, t2);
        GameAudioManager.GetInstance().PlayCardInfoMoveShown();
        yield return new WaitForSeconds(t1);
        backgroundImage.DOFillAmount(1f, t2);

        nodeTitleText.DOFade(1f, t1);
        yield return new WaitForSeconds(t1);



        showInfoCoroutine = null;
    }

    protected override IEnumerator HideNodeInfoAnimation()
    {
        transform.DOComplete();
        barImage.DOComplete();
        backgroundImage.DOComplete();
        nodeTitleText.DOComplete();

        float t1 = 0.1f;
        float t2 = 0.2f;

        transform.DORotate(Vector3.zero, 1f);


        backgroundImage.DOFillAmount(0f, t2);
        nodeTitleText.DOFade(0f, t1);
        yield return new WaitForSeconds(t1);

        barImage.DOFillAmount(0f, t1);
        yield return new WaitForSeconds(t1);

        infoCanvasTransform.gameObject.SetActive(false);

        hideInfoCoroutine = null;
    }
}
