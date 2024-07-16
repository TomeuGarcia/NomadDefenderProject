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
    
    [SerializeField] private UnityEvent _onClick = new UnityEvent();

    private void OnMouseEnter()
    {
        if (_outline != null)
        {
            _highlighted = true;
            _outline.enabled = true;
        }
        else
        {
            _outline = gameObject.AddComponent<Outline>();
            _highlighted = true;
        }
    }

    private void OnMouseExit()
    {
        if (_outline != null)
        {
            _highlighted = false;
            _outline.enabled = false;
        }
    }

    private void OnMouseDown()
    {
        if (_highlighted)
        {
            _onClick.Invoke();
        }
    }
}
