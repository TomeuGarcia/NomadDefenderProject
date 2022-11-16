using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CurrencyTargetConstranins : MonoBehaviour
{
    private Vector3 initialPosition;

    private void Awake()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        transform.position = new Vector3(transform.position.x, initialPosition.y, transform.position.z);
    }
}
