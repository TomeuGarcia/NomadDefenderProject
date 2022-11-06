using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformChildrenRotator : MonoBehaviour
{
    [SerializeField] private Transform parent;
    [SerializeField] private Vector3 axis = Vector3.up;
    [SerializeField] private float[] rotations;

    private void Awake()
    {
        for (int i = 0; i< parent.childCount; i++)
        {
            parent.GetChild(i).transform.rotation = Quaternion.AngleAxis(rotations[Random.Range(0, rotations.Length)], axis);
        }
    }



}
