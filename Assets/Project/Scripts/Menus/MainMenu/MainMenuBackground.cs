using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuBackground : MonoBehaviour
{
    [Header("BACKGROUND CAMERA")]
    [SerializeField] private Transform mapCamera;
    [SerializeField] private float mapCameraSpeed;
    [SerializeField] private float mapLength;
    private Vector3 camMoveVect => Vector3.forward * mapCameraSpeed;
    private float camDistDiff;

    private void Awake()
    {
        camDistDiff = mapLength - mapCamera.position.z; 
    }

    private void Update()
    {
        //Move background camera
        mapCamera.position += camMoveVect * Time.deltaTime;

        //Teleport when needed
        if (mapCamera.position.z >= mapLength)
        {
            mapCamera.position += Vector3.back * camDistDiff;
        }
    }
}
