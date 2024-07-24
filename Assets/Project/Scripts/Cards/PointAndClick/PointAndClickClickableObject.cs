using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class PointAndClickClickableObject : MonoBehaviour
{
    private Outline _outline;
    private bool _highlighted = false;

    private CursorChanger _cursorChanger;

    [SerializeField] private AFacilityInteractable _interactable;

    private void Awake()
    {
        _cursorChanger = ServiceLocator.GetInstance().CursorChanger;
    }

    private void OnMouseEnter()
    {
        if(!gameObject.activeInHierarchy) return;

        if (_outline != null)
        {
            _highlighted = true;
            //_outline.enabled = true;
        }
        else
        {
            //_outline = gameObject.AddComponent<Outline>();
            _highlighted = true;
        }

        MouseHoverCheck(true);
    }

    private void OnMouseOver()
    {
        if (!gameObject.activeInHierarchy) return;

        MouseHoverCheck(false);
    }

    private void OnMouseExit()
    {
        if (!gameObject.activeInHierarchy) return;

        //if (_outline != null)
        //{
        //  _highlighted = false;
        //  _outline.enabled = false;
        //}

        _cursorChanger.RegularCursor();
        if(_interactable.CanInteract())
        {
            _interactable.Unhovered();
        }
    }

    private void OnMouseDown()
    {
        if (!gameObject.activeInHierarchy) return;

        if (_highlighted)
        {
            //_onClick.Invoke();
            _interactable.Interact();
        }
    }

    private void MouseHoverCheck(bool recent)
    {
        if (_interactable.CanInteract())
        {
            _cursorChanger.HoverCursor();
            if(recent)
            {
                _interactable.Hovered();
            }
            return;
        }
        else if (_interactable.WaitingToInteract())
        {
            _cursorChanger.ForbiddenCursor();
            return;
        }
    }

    private void OnDestroy()
    {
        _cursorChanger.RegularCursor();
    }
}
