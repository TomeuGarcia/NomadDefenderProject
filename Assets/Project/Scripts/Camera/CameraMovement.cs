using System;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;
using static UnityEngine.UI.Image;
using Random = UnityEngine.Random;

public class CameraMovement : MonoBehaviour
{
    private Vector3 _cameraStartPosition;

    [Header("ZOOM")]
    [SerializeField, Range(0,1)] private float _startZoomRatio = 0.3f;
    [SerializeField, Min(0)] private float _totalZoomDistance = 30f;
    [SerializeField, Min(0)] private float _zoomSpeed = 4.0f;
    private float _totalZoomInDistance;
    private float _totalZoomOutDistance;
    
    private Vector3 _zoomAxis;
    private float _currentZoomDistance;

    private float ZoomRatio => (_currentZoomDistance - _totalZoomOutDistance) / _totalZoomDistance;
    
    [Header("PANNING")]
    [SerializeField, Min(0)] private float _totalZoomInPanningRadius = 6f;
    [SerializeField, Min(0)] private float _totalZoomOutPanningRadius = 3f;
    [SerializeField, Min(0)] private float _panningSpeed = 3f;
    
    private Vector3 _panningForwardAxis;
    private Vector3 _panningSidewaysAxis;
    
    private Vector2 _currentPannedDistance;
    private Vector2 _previousPanningMousePosition;
    private float PanningRadius => Mathf.LerpUnclamped(_totalZoomOutPanningRadius, _totalZoomInPanningRadius, ZoomRatio);


    private void Awake()
    {
        _cameraStartPosition = transform.position;
        _zoomAxis = transform.forward;

        _panningForwardAxis = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        _panningSidewaysAxis = Vector3.ProjectOnPlane(transform.right, Vector3.up).normalized;

        UpdateVariables();
    }

    private void OnValidate()
    {
        UpdateVariables();
    }

    private void UpdateVariables()
    {
        _totalZoomInDistance = _totalZoomDistance * (1f - _startZoomRatio);
        _totalZoomOutDistance = _totalZoomDistance * (- _startZoomRatio);
        _currentZoomDistance = Mathf.LerpUnclamped(_totalZoomOutDistance, _totalZoomInDistance, _startZoomRatio);
    }
    

    private void OnEnable()
    {
        PathLocation.OnTakeDamage += CameraShakeLocationTakeDamage;
        Enemy.OnTriedToAttackDeadLocation += OnTriedToAttackDeadLocation;
    }
    private void OnDisable()
    {
        PathLocation.OnTakeDamage -= CameraShakeLocationTakeDamage;
        Enemy.OnTriedToAttackDeadLocation -= OnTriedToAttackDeadLocation;
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

    private void OnTriedToAttackDeadLocation(Enemy enemy, PathLocation attackedLocation)
    {
        CameraShakeLocationTakeDamage(attackedLocation);
    }



    
    

    private void LateUpdate()
    {
        UpdateZoomDistance();
        Vector3 zoomOffset = _zoomAxis * _currentZoomDistance;

        UpdatePanningDistance();
        Vector3 panningOffset = (_panningForwardAxis * _currentPannedDistance.y) +
                                (_panningSidewaysAxis * _currentPannedDistance.x);

        transform.position = _cameraStartPosition + zoomOffset + panningOffset;
    }


    private void UpdateZoomDistance()
    {
        float zoomDelta = Input.mouseScrollDelta.y * _zoomSpeed;
        _currentZoomDistance = Mathf.Clamp(_currentZoomDistance + zoomDelta, _totalZoomOutDistance, _totalZoomInDistance);
    }

    private void UpdatePanningDistance()
    {
        Vector2 panningDelta = UpdatePanningDelta();
        
        _currentPannedDistance += panningDelta * _panningSpeed;
        _currentPannedDistance = Vector2.ClampMagnitude(_currentPannedDistance, PanningRadius);
    }

    private Vector2 UpdatePanningDelta()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            _previousPanningMousePosition = Input.mousePosition;
            return Vector2.zero;
        }
        else if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            return Vector2.zero;
        }
        else if (!Input.GetKey(KeyCode.Mouse1))
        {
            return Vector2.zero;
        }
        

        Vector2 currentPanningMousePosition = Input.mousePosition;
        Vector2 panningDelta = currentPanningMousePosition - _previousPanningMousePosition;
        _previousPanningMousePosition = Input.mousePosition;

        return panningDelta;
    }

    private void OnDrawGizmos()
    {
        Color defaultColor = Color.green;
        Color maxZoomInColor = Color.blue;
        Color maxZoomOutColor = Color.red;
        
        Vector3 maxZoomInPosition = _cameraStartPosition + (_zoomAxis * _totalZoomInDistance);
        Vector3 maxZoomOutPosition = _cameraStartPosition + (_zoomAxis * _totalZoomOutDistance);
        Vector3 currentZoomPosition = _cameraStartPosition + (_zoomAxis * _currentZoomDistance);

        Gizmos.color = defaultColor;
        Gizmos.DrawLine(maxZoomInPosition, maxZoomOutPosition);
        
        Gizmos.color = maxZoomInColor;
        Gizmos.DrawSphere(maxZoomInPosition, 0.2f);
        
        Gizmos.color = maxZoomOutColor;
        Gizmos.DrawSphere(maxZoomOutPosition, 0.2f);
        
        Gizmos.color = defaultColor;
        Gizmos.DrawSphere(currentZoomPosition, 0.2f);


        Gizmos.color = Color.LerpUnclamped(maxZoomOutColor, maxZoomInColor, ZoomRatio);
        GizmosUtility.DrawCircle(_cameraStartPosition + (_zoomAxis * _currentZoomDistance), Vector3.up, PanningRadius);
        
    }
}
