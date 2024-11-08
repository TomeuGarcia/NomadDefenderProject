using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseOverlapNotifier : MonoBehaviour
{
    public event Action OnMouseEntered;
    public event Action OnMouseExited;

    private Camera _camera;

    private bool _isBeingOverlapped;

    private void Start()
    {
        _camera = MouseOverlapCamera.Camera;
    }

    private void Update()
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, float.PositiveInfinity, 1 << gameObject.layer, QueryTriggerInteraction.Collide))
        {
            bool hitThis = hit.transform == transform;

            if (hitThis)
            {
                if (_isBeingOverlapped)
                {
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
