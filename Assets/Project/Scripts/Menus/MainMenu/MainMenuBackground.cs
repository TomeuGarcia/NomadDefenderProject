using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuBackground : MonoBehaviour
{
    [Header("BACKGROUND CAMERA")]
    [SerializeField] private Transform mapCameraParent;
    [SerializeField] private float mapCameraSpeed;

    private void Update()
    {
        //Move background camera
        mapCameraParent.Rotate(new Vector3(0, mapCameraSpeed * Time.deltaTime, 0));
    }
}
