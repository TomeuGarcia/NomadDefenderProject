using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressiveRotator : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;
    [SerializeField] private bool xAxis;

    void Update()
    {
        if(xAxis)
        {
            transform.Rotate(new Vector3(rotationSpeed * Time.deltaTime, 0, 0));
        }
        else
        {
            transform.Rotate(new Vector3(0, rotationSpeed * Time.deltaTime, 0));
        }
    }
}
