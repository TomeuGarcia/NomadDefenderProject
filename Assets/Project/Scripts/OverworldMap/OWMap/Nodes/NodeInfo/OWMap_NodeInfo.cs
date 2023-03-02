using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.RuleTile.TilingRuleOutput;


public class OWMap_NodeInfo : MonoBehaviour
{
    private bool isInteractable = true;
    private MouseOverNotifier mouseOverNotifier;
    private OWMap_Node attachedNode;

    [Header("DISPLAY")]
    [SerializeField] private RectTransform infoCanvasTransform;
    [SerializeField] private Image barImage;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TextMeshProUGUI nodeInfoText;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI rewardsText;

    private static string[] nodeHealthToText = { "<color=#1DFF5F>SAFE</color>",
                                                 "<color=#FFF345>PLUNDERED</color>",
                                                 "<color=#F5550C>DEMOLISHED</color>",
                                                 "<color=#FF003E>DESTROYED</color>" };
    private static string[] nodeHealthToRewardText = { "<color=#1DFF5F>GREAT</color>",
                                                       "<color=#FFF345>MEDIUM</color>",
                                                       "<color=#F5550C>POOR</color>",
                                                       "<color=#FF003E>ERROR</color>" };
    private const string perfectNodeHealthText = "<color=#439bee>PERFECT</color>";
    private const string perfectRewardText = "<color=#439bee>GREAT+</color>";

    private const string statusStr = "\\status: ";
    private const string rewardsStr = "\\rewards: ";
    private const string unknownStr = "???";

    private Coroutine showInfoCoroutine = null;
    private Coroutine hideInfoCoroutine = null;


    private void Awake()
    {
        infoCanvasTransform.gameObject.SetActive(false);

        statusText.text = statusStr + unknownStr;
        rewardsText.text = rewardsStr + unknownStr;
    }

    private void OnEnable()
    {
        if (mouseOverNotifier != null)
        {
            SubscriveToMouseOverEvents();
        }

        if (attachedNode != null)
        {
            SubscriveToAttachedNodeEvents();
        }
        
    }

    private void OnDisable()
    {
        if (mouseOverNotifier != null)
        {
            UnsubscriveToMouseOverEvents();
        }

        if (attachedNode != null)
        {
            UnsubscriveToAttachedNodeEvents();
        }
    }

    public void Init(OWMap_Node attachedNode, MouseOverNotifier mouseOverNotifier, bool positionedAtRight)
    {
        this.attachedNode = attachedNode;
        SubscriveToAttachedNodeEvents();

        this.mouseOverNotifier = mouseOverNotifier;
        SubscriveToMouseOverEvents();

        float posX = positionedAtRight ? transform.localPosition.x : -transform.localPosition.x;
        transform.localPosition = new Vector3(posX, 0f, 0f);

        barImage.transform.localRotation = positionedAtRight? Quaternion.identity : Quaternion.AngleAxis(180f, Vector3.up);
    }   
    public void SetIsInteractableFalse()
    {
        Debug.Log("isInteractable FALSE");
        this.isInteractable = false;
    }
    public void SetIsInteractableTrue()
    {
        Debug.Log("isInteractable TRUE");
        this.isInteractable = true;
    }


    // Events
    private void SubscriveToMouseOverEvents()
    {
        mouseOverNotifier.OnMouseEntered += CheckShowInfo;
        mouseOverNotifier.OnMouseExited += CheckHideInfo;
    }
    private void UnsubscriveToMouseOverEvents()
    {
        mouseOverNotifier.OnMouseEntered -= CheckShowInfo;
        mouseOverNotifier.OnMouseExited -= CheckHideInfo;
    }

    private void SubscriveToAttachedNodeEvents()
    {
        this.attachedNode.OnNodeHealthStateSet += SetupTexts;
        this.attachedNode.OnNodeInfoInteractionEnabled += SetIsInteractableTrue;
    }
    private void UnsubscriveToAttachedNodeEvents()
    {
        this.attachedNode.OnNodeHealthStateSet -= SetupTexts;
        this.attachedNode.OnNodeInfoInteractionEnabled -= SetIsInteractableTrue;
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


    private void CheckShowInfo()
    {
        if (!isInteractable) return;

        if (hideInfoCoroutine != null) StopCoroutine(hideInfoCoroutine);
        ShowNodeInfo();
    }

    private void CheckHideInfo()
    {
        if (!isInteractable) return;

        if (showInfoCoroutine != null) StopCoroutine(showInfoCoroutine);
        HideNodeInfo();        
    }


    private void ShowNodeInfo()
    {
        showInfoCoroutine = StartCoroutine(ShowNodeInfoAnimation());
    }

    private IEnumerator ShowNodeInfoAnimation()
    {
        transform.DOComplete();
        barImage.DOComplete();
        backgroundImage.DOComplete();
        nodeInfoText.DOComplete();
        statusText.DOComplete();
        rewardsText.DOComplete();

        float t0 = 0.001f;
        float t1 = 0.1f;
        float t2 = 0.2f;

        infoCanvasTransform.gameObject.SetActive(true);
        barImage.fillAmount = 0f;
        backgroundImage.fillAmount = 0f;
        nodeInfoText.DOFade(0f, t0);
        statusText.DOFade(0f, t0);
        rewardsText.DOFade(0f, t0);
        yield return new WaitForSeconds(t0);


        transform.DORotate(Vector3.right * -20f, 0.75f);

        barImage.DOFillAmount(1f, t2);
        backgroundImage.DOFillAmount(1f, t2*2f);
        GameAudioManager.GetInstance().PlayCardInfoMoveShown();
        yield return new WaitForSeconds(t1);

        nodeInfoText.DOFade(1f, t1);
        GameAudioManager.GetInstance().PlayCardInfoShown();
        yield return new WaitForSeconds(t1);


        yield return new WaitForSeconds(t1);


        statusText.DOFade(1f, t1);
        GameAudioManager.GetInstance().PlayCardInfoShown();
        yield return new WaitForSeconds(t1);


        rewardsText.DOFade(1f, t1);
        GameAudioManager.GetInstance().PlayCardInfoShown();
        yield return new WaitForSeconds(t1);

        showInfoCoroutine = null;
    }


    private void HideNodeInfo()
    {
        hideInfoCoroutine = StartCoroutine(HideNodeInfoAnimation());
    }

    private IEnumerator HideNodeInfoAnimation()
    {
        transform.DOComplete();
        barImage.DOComplete();
        backgroundImage.DOComplete();
        nodeInfoText.DOComplete();
        statusText.DOComplete();
        rewardsText.DOComplete();

        float t1 = 0.1f;

        transform.DORotate(Vector3.zero, 1f);

        rewardsText.DOFade(0f, t1);
        GameAudioManager.GetInstance().PlayCardInfoHidden();
        yield return new WaitForSeconds(t1);

        statusText.DOFade(0f, t1);
        GameAudioManager.GetInstance().PlayCardInfoHidden();
        yield return new WaitForSeconds(t1);

        nodeInfoText.DOFade(0f, t1);
        GameAudioManager.GetInstance().PlayCardInfoHidden();
        yield return new WaitForSeconds(t1);

        barImage.DOFillAmount(0f, t1);
        backgroundImage.DOFillAmount(0f, t1);
        GameAudioManager.GetInstance().PlayCardInfoMoveHidden();
        yield return new WaitForSeconds(t1);

        infoCanvasTransform.gameObject.SetActive(false);

        hideInfoCoroutine = null;
    }
}
