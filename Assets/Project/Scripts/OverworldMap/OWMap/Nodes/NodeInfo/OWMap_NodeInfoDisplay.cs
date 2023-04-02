using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public abstract class OWMap_NodeInfoDisplay : MonoBehaviour
{
    private bool isInteractable = true;
    private MouseOverNotifier mouseOverNotifier;
    protected OWMap_Node attachedNode;

    protected Coroutine showInfoCoroutine = null;
    protected Coroutine hideInfoCoroutine = null;


    [Header("DISPLAY")]
    [SerializeField] protected RectTransform infoCanvasTransform;
    [SerializeField] protected Image backgroundImage;
    [SerializeField] protected Image barImage;


    protected void BaseInit(OWMap_Node attachedNode, MouseOverNotifier mouseOverNotifier, bool positionedAtRight)
    {
        this.attachedNode = attachedNode;
        SubscriveToAttachedNodeEvents();

        this.mouseOverNotifier = mouseOverNotifier;
        SubscriveToMouseOverEvents();

        float posX = positionedAtRight ? transform.localPosition.x : -transform.localPosition.x;
        transform.localPosition = new Vector3(posX, 0f, 0f);

        barImage.transform.localRotation = positionedAtRight ? Quaternion.identity : Quaternion.AngleAxis(180f, Vector3.up);
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
  
    public void SetIsInteractableFalse()
    {
        this.isInteractable = false;
        HideNodeInfo();
    }
    public void SetIsInteractableTrue()
    {
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

    protected virtual void SubscriveToAttachedNodeEvents()
    {        
        this.attachedNode.OnNodeInfoInteractionEnabled += SetIsInteractableTrue;
        //this.attachedNode.OnNodeInfoInteractionDisabled += SetIsInteractableFalse;
    }
    protected virtual void UnsubscriveToAttachedNodeEvents()
    {
        this.attachedNode.OnNodeInfoInteractionEnabled -= SetIsInteractableTrue;
        //this.attachedNode.OnNodeInfoInteractionDisabled -= SetIsInteractableFalse;
    }


    // Functionality    
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

    protected abstract IEnumerator ShowNodeInfoAnimation();


    private void HideNodeInfo()
    {
        hideInfoCoroutine = StartCoroutine(HideNodeInfoAnimation());
    }

    protected abstract IEnumerator HideNodeInfoAnimation();
}
