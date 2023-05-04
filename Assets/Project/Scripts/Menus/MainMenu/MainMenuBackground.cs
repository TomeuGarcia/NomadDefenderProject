using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuBackground : MonoBehaviour
{
    [Header("BACKGROUND CAMERA")]
    [SerializeField] private Transform mapCameraParent;
    [SerializeField] private Vector3 mapCameraRotationGoal;
    [SerializeField] private float cycleTime;
    [SerializeField] private float cyclePauseTime;

    private void Start()
    {
        StartCoroutine(MapCameraRotation(0.5f));
    }

    IEnumerator MapCameraRotation(float cycleCoef)
    {
        mapCameraParent.DORotate(mapCameraRotationGoal, cycleTime);
        yield return new WaitForSeconds(cycleTime * cycleCoef);
        yield return new WaitForSeconds(cyclePauseTime);

        mapCameraRotationGoal *= -1.0f;
        StartCoroutine(MapCameraRotation(1.0f));
    }
}
