using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementEnabler : MonoBehaviour
{
    [SerializeField] private MonoBehaviour _script;

    private Vector3 _lastPosition;

    private void Update()
    {
        _script.enabled = (_lastPosition != transform.position);

        _lastPosition = transform.position;
    }
}
