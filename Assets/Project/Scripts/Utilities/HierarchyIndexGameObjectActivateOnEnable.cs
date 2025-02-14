using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class HierarchyIndexGameObjectActivateOnEnable : MonoBehaviour
{
    [SerializeField] private Transform _hiererchyObjectToTrack;
    [SerializeField] private GameObject[] _possibleObjects;

    private void OnEnable()
    {
        ActivateObject();
    }

    private async void ActivateObject()
    {
        await Task.Yield();
        
        int activateObjectIndex = _hiererchyObjectToTrack.GetSiblingIndex() % _possibleObjects.Length;

        for (int i = 0; i < _possibleObjects.Length; ++i)
        {
            _possibleObjects[i].SetActive(i == activateObjectIndex);
        }
    }

}
