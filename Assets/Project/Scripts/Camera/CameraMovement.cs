using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;
using static UnityEngine.UI.Image;

public class CameraMovement : MonoBehaviour
{
    const float MAX_DRAG_DISTANCE = 3f;
    Vector3 cameraOriginPos;
    Vector3 cameraDragPos;
    Vector3 cameraZoomPos = Vector3.zero;


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

        maxZoomIn = 40.0f;
        maxZoomOut = -20.0f;
        acumulatedZoom = 0.0f;
        zoomStep = 2.0f;

        cameraOriginPos = transform.position;
        cameraDragPos = cameraOriginPos;
        cameraZoomPos = Vector3.zero;
    }

    private void OnEnable()
    {
        PathLocation.OnTakeDamage += CameraShakeLocationTakeDamage;
    }
    private void OnDisable()
    {
        PathLocation.OnTakeDamage -= CameraShakeLocationTakeDamage;
    }

    void LateUpdate()
    {
        float zoomIncrement = Input.mouseScrollDelta.y * zoomStep;
        acumulatedZoom = Mathf.Clamp(acumulatedZoom + zoomIncrement, maxZoomOut, maxZoomIn);
        if(acumulatedZoom >= maxZoomIn || acumulatedZoom <= maxZoomOut)
        {
            zoomIncrement = 0.0f;
        }
        MoveCameraByDisplacement(Vector3.zero, zoomAxis* zoomIncrement);

        if (Input.GetMouseButtonDown(1))
        {
            lastOrigin = Input.mousePosition;
        }

        if (Input.GetMouseButton(1))
        {
            difference = Input.mousePosition - lastOrigin;
            float differenceMag = difference.magnitude / 25.0f;

            //accumulatedDifferenceX = Mathf.Clamp(accumulatedDifferenceX + difference.x, -maxDifferenceX, maxDifferenceX);
            //if (accumulatedDifferenceX >= maxDifferenceX || accumulatedDifferenceX <= -maxDifferenceX)
            //    difference.x = 0f;


            difference.Normalize();
            //Vector3 displacement = (moveX * difference.x + moveZ * difference.y + zoomAxis * zoomIncrement).normalized * -dragSpeed * differenceMag;
            Vector3 displacementDrag = (moveX * difference.x + moveZ * difference.y).normalized * -dragSpeed * differenceMag;
            Vector3 displacementZoom = (zoomAxis * zoomIncrement).normalized * -dragSpeed * differenceMag;
            MoveCameraByDisplacement(displacementDrag, displacementZoom);
            lastOrigin = Input.mousePosition;

            
        }
    }

    private void MoveCameraByDisplacement(Vector3 displacementDrag, Vector3 displacementZoom)
    {
        cameraDragPos += displacementDrag;
        cameraZoomPos += displacementZoom;

        if (Vector3.Distance(cameraOriginPos, cameraDragPos) > MAX_DRAG_DISTANCE)
        {
            cameraDragPos = (cameraDragPos - cameraOriginPos).normalized * MAX_DRAG_DISTANCE + cameraOriginPos;
        }

        transform.position = cameraDragPos + cameraZoomPos;
    }



    public void CameraShake(float duration, int vibrato)
    {
        transform.DOComplete();

        float randomY = Random.Range(0, 2) > 0 ? Random.Range(0.3f, 0.5f) : Random.Range(-0.5f, -0.3f);
        Vector3 shakePunch = new Vector3(Random.Range(0.1f, 0.2f), randomY, Random.Range(0.1f, 0.2f));
        transform.DOPunchRotation(shakePunch, 0.5f, vibrato, 0.0f);
        //transform.DOShakePosition(1.0f, 100.0f, 10, 90, false, true, ShakeRandomnessMode.Full);
    }

    private void CameraShakeLocationTakeDamage(PathLocation pathLocationThatTookDamage)
    {
        transform.DOComplete();

        float randomY = Random.Range(0, 2) > 0 ? Random.Range(0.3f, 0.5f) : Random.Range(-0.5f, -0.3f);
        Vector3 shakePunch = new Vector3(Random.Range(0.1f, 0.2f), randomY, Random.Range(0.1f, 0.2f)) * 1.5f;
        transform.DOPunchRotation(shakePunch, 0.5f, 10);
    }

    private void CameraShakeBuildingPlaced()
    {
        transform.DOComplete();

        Vector3 shakePunch = new Vector3(Random.Range(-0.25f, -0.1f), 0f, 0f) * 1.5f;
        transform.DOPunchRotation(shakePunch, 0.4f, 9);
    }

}
