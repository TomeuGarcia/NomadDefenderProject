using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEngine.UI.Image;

public class CameraMovement : MonoBehaviour
{
    private Vector3 difference;
    private Vector3 lastOrigin;

    private Vector3 moveX;
    private Vector3 moveZ;

    private Vector3 zoomAxis;
    private float maxZoomIn;
    private float maxZoomOut;
    private float acumulatedZoom;
    private float zoomStep;

    private float dragSpeed;

    private void Awake()
    {
        dragSpeed = 0.25f;

        moveX = (Quaternion.AngleAxis(transform.rotation.x, Vector3.right) * transform.right).normalized;
        moveZ = (Quaternion.AngleAxis(transform.rotation.x, Vector3.right) * transform.up).normalized;
        zoomAxis = (Quaternion.AngleAxis(transform.rotation.x, Vector3.right) * transform.forward).normalized;

        maxZoomIn = 50.0f;
        maxZoomOut = -20.0f;
        acumulatedZoom = 0.0f;
        zoomStep = 2.0f;
    }

    void LateUpdate()
    {
        float zoomIncrement = Input.mouseScrollDelta.y * zoomStep;
        acumulatedZoom = Mathf.Clamp(acumulatedZoom + zoomIncrement, maxZoomOut, maxZoomIn);
        if(acumulatedZoom >= maxZoomIn || acumulatedZoom <= maxZoomOut)
        {
            zoomIncrement = 0.0f;
        }
        gameObject.transform.position += zoomAxis * zoomIncrement;

        if (Input.GetMouseButtonDown(1))
        {
            lastOrigin = Input.mousePosition;
        }

        if (Input.GetMouseButton(1))
        {
            difference = Input.mousePosition - lastOrigin;
            float differenceMag = difference.magnitude / 25.0f;
            difference.Normalize();
            gameObject.transform.position += (moveX * difference.x + moveZ * difference.y + zoomAxis * zoomIncrement).normalized * -dragSpeed * differenceMag;
            lastOrigin = Input.mousePosition;
        }
    }
}
