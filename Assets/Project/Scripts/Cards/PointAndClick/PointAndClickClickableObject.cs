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

        if (_interactable.CanInteract())
        {
            _cursorChanger.HoverCursor();
        }
    }

    private void OnMouseOver()
    {
        if (_interactable.CanInteract())
        {
            _cursorChanger.HoverCursor();
        }
    }

    private void OnMouseExit()
    {
        //if (_outline != null)
        //{
        //  _highlighted = false;
        //  _outline.enabled = false;
        //}

        _cursorChanger.RegularCursor();
    }

    private void OnMouseDown()
    {
        if (_highlighted)
        {
            //_onClick.Invoke();
            _interactable.Interact();
        }
    }
}
