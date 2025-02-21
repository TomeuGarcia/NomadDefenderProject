using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseOverlapNotifier : MonoBehaviour
{
    public static bool GlobalDisabled = false;
    
    public event Action OnMouseEntered;
    public event Action OnMouseExited;
    public event Action OnMousePressed;
    
    

    private Camera _camera;

    private bool _isBeingOverlapped;

    private void Start()
    {
        _camera = MouseOverlapCamera.Camera;
    }

    private void Update()
    {
        if (GlobalDisabled)
        {
            if (_isBeingOverlapped)
            {
                _isBeingOverlapped = false;
                OnMouseExited?.Invoke();
            }
            return;
        }
        
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, float.PositiveInfinity, 1 << gameObject.layer, QueryTriggerInteraction.Collide))
        {
            bool hitThis = hit.transform == transform;

            if (hitThis)
            {
                if (_isBeingOverlapped)
                {
                    if (OnMousePressed != null && Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        OnMousePressed.Invoke();
                    }
                    return;
                }
                
                OnMouseEntered?.Invoke();
                _isBeingOverlapped = true;
                return;
            }
        }
        
        if (_isBeingOverlapped)
        {
            _isBeingOverlapped = false;
            OnMouseExited?.Invoke();
        }
        
    }

}
