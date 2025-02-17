using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomGameObjectActivateOnEnable : MonoBehaviour
{
    [SerializeField] private GameObject[] _possibleObjects;

    private void OnEnable()
    {
        ActivateRandomObject();
    }

    private void ActivateRandomObject()
    {
        int activateObjectIndex = Random.Range(0, _possibleObjects.Length);

        for (int i = 0; i < _possibleObjects.Length; ++i)
        {
            _possibleObjects[i].SetActive(i == activateObjectIndex);
        }
    }
}
